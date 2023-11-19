using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace STYLY.MaintenanceTool.Utility
{
    /// <summary>
    /// ADBでのAndroid端末操作用Utilityクラス
    /// </summary>
    public class AndroidVRUtility : IPlatformUtility
    {
        /// <summary>AssetBundleのキャッシュフォルダ</summary>
        private const string OutputPath = "_Output";

        /// <summary>テストモード時に端末へ送るXMLファイル名</summary>
        private const string TestModeSceneXml = "TestModeScene.xml";

        public static string PackageBasePath;

        static string ADB_COMMAND = "";

        public string SerialNo = null;
        public string PackageName = null;
        public string IPAddress = null;
        private string propertyString = null;

        public AndroidVRUtility()
        {
            if (!File.Exists(ADB_COMMAND))
            {
                SetADBCommandByRuntimePlatform();
            }
            if (!File.Exists(ADB_COMMAND))
            {
                Debug.LogError(ADB_COMMAND + " is not found.");
            }
            Debug.Log(ADB_COMMAND);
        }

        /// <summary>
        /// adb.exeのファイルパスを設定する
        /// </summary>
        void SetADBCommandByRuntimePlatform()
        {
            if (PackageBasePath == null)
            {
                // Windowsなら\\, Macなら/で結合
                PackageBasePath = Path.Combine(Application.dataPath, "STYLY_Plugin", "STYLY_MaintenanceTool");
            }

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                ADB_COMMAND = PackageBasePath + "\\Tools\\ADB\\Windows\\adb.exe";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                ADB_COMMAND = PackageBasePath + "/Tools/ADB/OSX/adb";
            }
        }

        public AndroidVRUtility(string serialNo)
        {
            this.SerialNo = serialNo;
        }

        /// <summary>
        /// 接続しているデバイス分インスタンスを返す。
        /// 同一端末がUSB接続/WiFi接続両方で検出した場合、WiFi接続のみ返却する。
        /// </summary>
        /// <returns>AndroidVRUtilityのリスト</returns>
        public static List<IPlatformUtility> GetInstances()
        {
            var androidUtil = new AndroidVRUtility();
            var dic = androidUtil.GetDeviceModelName();

            var utils = new List<IPlatformUtility>();
            foreach (var serialNo in dic.Keys)
            {
                var util = new AndroidVRUtility(serialNo);
                util.propertyString = util.GetProperty();

                utils.Add(util);
            }

            foreach (AndroidVRUtility util in utils)
            {
                util.IPAddress = util.GetIPAddress();
                util.PackageName = util.GetPackageName();
            }
            return utils;
        }

        public int RunExternalProcessSync(string command, string[] argsArray, out string outputString, out string errorString)
        {
            var commonUtility = new CommonUtility();
            return commonUtility.RunExternalProcessSync(command, argsArray, out outputString, out errorString);
        }

        public string RunADBDevices()
        {
            string outString;
            string errorString;
            var command = ADB_COMMAND;
            string[] args = { "devices", "-l" };
            RunExternalProcessSync(command, args, out outString, out errorString);
            return outString;
        }

        /// <summary>
        /// ディレクトリーを作成する
        /// </summary>
        /// <param name="path">ディレクトリーパス</param>
        /// <returns>成否。 0: 成功, -1: 失敗</returns>
        public int MakeDirectory(string path)
        {
            Debug.Log("Make Directory:" + path);
            List<string> args = new List<string>();

            if (SerialNo != null)
            {
                args.Add("-s " + SerialNo);
            }
            string[] postArgs = { "shell", "mkdir", path };
            args.AddRange(postArgs);
            string outputStr, errorStr;
            var exitCode = RunExternalProcessSync(ADB_COMMAND, args.ToArray(), out outputStr, out errorStr);
            Debug.Log(outputStr);

            return exitCode;
        }

        /// <summary>
        /// ディレクトリーを削除する
        /// </summary>
        /// <param name="path">ディレクトリーパス</param>
        /// <returns>成否。 0: 成功, -1: 失敗</returns>
        public int DeleteDirectory(string path)
        {
            Debug.Log("Delete Directory:" + path);
            List<string> args = new List<string>();

            if (SerialNo != null)
            {
                args.Add("-s " + SerialNo);
            }
            string[] postArgs = { "shell", "rm", "-rf", path };
            args.AddRange(postArgs);
            string outputStr, errorStr;
            var exitCode = RunExternalProcessSync(ADB_COMMAND, args.ToArray(), out outputStr, out errorStr);
            Debug.Log(outputStr);

            return exitCode;
        }

        /// <summary>
        /// ファイルを端末へコピーする
        /// </summary>
        /// <param name="srcPath">コピー元ファイルパス</param>
        /// <param name="destPath">コピー先ファイルパス</param>
        /// <returns>成否。 0: 成功, -1: 失敗</returns>
        public int PushFile(string srcPath, string destPath)
        {
            Debug.Log("Push File: src:" + srcPath + " dest:" + destPath);
            List<string> args = new List<string>();
            if (SerialNo != null)
            {
                args.Add("-s " + SerialNo);
            }

            args.Add("push");
            args.Add(EncloseWithQuotation(srcPath));
            args.Add(EncloseWithQuotation(destPath));
            string outputStr, errorStr;
            var exitCode = RunExternalProcessSync(ADB_COMMAND, args.ToArray(), out outputStr, out errorStr);
            Debug.Log(outputStr);

            return exitCode;
        }

        /// <summary>
        /// URL Schemeを作成する。
        /// </summary>
        /// <param name="guid">シーンGUID</param>
        /// <param name="userName">ユーザー名</param>
        /// <param name="testMode">テストモードか否か。デフォルトはfalse</param>
        /// <returns>URL文字列</returns>
        public string CreateURLScheme(string guid, string userName = null, bool testMode = false)
        {
            if (userName == null)
            {
                userName = "dummy";
            }

            var urlScheme = "styly-app://styly.world" + "/" + userName + "/" + guid;

            // テストモードならクエリを追加する
            if (testMode)
            {
                urlScheme += "?testmode=true";
            }

            return urlScheme;
        }

        /// <summary>
        /// STYLYを起動する
        /// </summary>
        /// <returns>成否。 0: 成功, -1: 失敗</returns>
        public int StartSTYLY()
        {
            List<string> args = new List<string>();
            if (SerialNo != null)
            {
                args.Add("-s " + SerialNo);
            }

            if (PackageName == null)
            {
                Debug.LogError("PackageName is null!!!");
                return -1;
            }

            // STYLY-1912 PackageNameと/comの間にスペースがあると、エラーになるので注意する。
            string[] postArgs = { "shell am start -n", PackageName + "/com.psychicvrlab.styly.styly_androidlib.STYLYActivity" };
            args.AddRange(postArgs);
            string outputStr, errorStr;
            var exitCode = RunExternalProcessSync(ADB_COMMAND, args.ToArray(), out outputStr, out errorStr);

            return exitCode;
        }

        /// <summary>
        /// URLを指定してSTYLYを起動する
        /// </summary>
        /// <param name="urlScheme">URL</param>
        /// <returns>成否。 0: 成功, -1: 失敗</returns>
        public int StartSTYLY(string urlScheme)
        {
            List<string> args = new List<string>();
            if (SerialNo != null)
            {
                args.Add("-s " + SerialNo);
            }

            string[] postArgs = { "shell am start -a android.intent.action.VIEW -d ", "\"" + urlScheme + "\"" };
            args.AddRange(postArgs);
            string outputStr, errorStr;
            var exitCode = RunExternalProcessSync(ADB_COMMAND, args.ToArray(), out outputStr, out errorStr);

            return exitCode;
        }

        public Dictionary<string, string> GetDeviceModelName()
        {
            var deviceDic = new Dictionary<string, string>();
            var output = RunADBDevices();

            Debug.Log("output = " + output);

            var match = Regex.Matches(output, "\n([^ ]+).*model:([^ ]+)");
            foreach (Match model in match)
            {
                Debug.Log("model = " + model);

                var serialNo = model.Groups[1].Value.TrimStart().TrimEnd();
                var modelName = model.Groups[2].Value.TrimStart().TrimEnd();
                deviceDic[serialNo] = modelName;
            }

            return deviceDic;
        }

        private const string PACKAGE_NAME_OCULUS = "com.psychicvrlab.styly.gearvr";
        private const string PACKAGE_NAME_DAYDREAM = "com.psychicvrlab.styly.daydream";

        string GetProperty()
        {
            string[] args = { "-s", SerialNo, "shell", "getprop" };
            string outputStr, errorStr;
            RunExternalProcessSync(ADB_COMMAND, args, out outputStr, out errorStr);
            //            Debug.Log(outputStr);

            return outputStr.TrimStart().TrimEnd();
        }

        public string GetPackageName()
        {
            if (PackageName != null)
            {
                return PackageName;
            }
            if (propertyString == null)
            {
                propertyString = GetProperty();
            }

            if (propertyString.ToLower().IndexOf("oculus") >= 0)
            {
                return PACKAGE_NAME_OCULUS;
            }
            else
            {
                var stylyPackageName = GetPackageNameContains("com.psychicvrlab.styly");
                if (stylyPackageName.Length > 0)
                {
                    return stylyPackageName[0];
                }
                else
                {
                    return null;
                }
            }
        }

        string[] GetPackageList()
        {
            string[] args = { "-s", SerialNo, "shell", "pm list package" };
            string outputStr, errorStr;
            RunExternalProcessSync(ADB_COMMAND, args, out outputStr, out errorStr);

            outputStr = outputStr.TrimStart().TrimEnd();
            Debug.Log(outputStr);

            // STYLY-1912 Macでは改行コードが\nだったことにより正確にSplitできていないバグがあったため修正。
            var packgeList = outputStr.Split(new[] {'\n', '\r'});

            return packgeList;
        }

        public string[] GetPackageNameContains(string substring)
        {
            List<string> hitPackageList = new List<string>();

            var packageList = GetPackageList();
            foreach (var package in packageList)
            {
                if (package.Contains(substring))
                {
                    // package名を取り出す。
                    var hitPackageName = package.TrimStart().TrimEnd();
                    hitPackageName = hitPackageName.Substring(hitPackageName.IndexOf(":") + 1);
                    hitPackageList.Add(hitPackageName);
                }
            }

            return hitPackageList.ToArray();
        }

        public string GetIPAddress()
        {
            string[] args = { "-s", SerialNo, "shell ip addr show wlan0 | grep 'inet '", };
            string outputStr, errorStr;
            RunExternalProcessSync(ADB_COMMAND, args, out outputStr, out errorStr);
            Debug.Log(outputStr);
            outputStr = outputStr.TrimStart().TrimEnd();

            //     inet 192.168.55.171/24 brd 192.168.xxx.xxx scope global wlan0
            var match = Regex.Matches(outputStr, "inet ([^/]+)");
            if (match.Count <= 0)
            {
                Debug.Log("Ip address cannot get. Maybe offline.");
                return null;
            }
            var result = match[0].Groups[1].Value;
            return result;
        }

        public string ConnectWifi()
        {
            // adb.exe tcpip 5555
            // adb.exe connect 192.168.xxx.xxx:5555

            if (this.IPAddress != null)
            {
                string[] args = { "-s", SerialNo, "tcpip 5555", };
                string outputStr, errorStr;
                RunExternalProcessSync(ADB_COMMAND, args, out outputStr, out errorStr);

                args[2] = "connect " + this.IPAddress + ":5555";

                RunExternalProcessSync(ADB_COMMAND, args, out outputStr, out errorStr);
                Debug.Log(outputStr);
                outputStr = outputStr.TrimStart().TrimEnd();

                if (outputStr.IndexOf("connected to") >= 0)
                {
                    return outputStr;
                }
                return null;
            }

            return null;
        }

        public string DisconnectWifi()
        {
            string[] args = { "-s", SerialNo, "disconnect", };
            string outputStr, errorStr;
            RunExternalProcessSync(ADB_COMMAND, args, out outputStr, out errorStr);

            return outputStr.TrimStart().TrimEnd();
        }

        /// <summary>
        /// デストラクタ
        /// adb kill-serverを実行する。
        /// </summary>
        ~AndroidVRUtility()
        {
            // SerialNoを指定しないと、ADBコマンドが終了しないバグがあったため、バリデーションする。
            if (!string.IsNullOrEmpty(SerialNo))
            {
                string[] args = { "-s", SerialNo, "kill-server" };
                RunExternalProcessSync(ADB_COMMAND, args, out var outputStr, out var errorStr);
            }
        }

        /// <summary>
        /// STYLY_TESTMODEフォルダのファイルをすべて削除する
        /// </summary>
        public void ClearSTYLYTestMode()
        {
            string directoryPath = "/sdcard/Android/data/" + PackageName + "/files/STYLY_TESTMODE";

            // フォルダをクリアする。
            DeleteDirectory(directoryPath);
            MakeDirectory(directoryPath);
        }

        /// <summary>
        /// シーンXMLファイルをSTYLY_TESTMODEフォルダへ出力する
        /// </summary>
        /// <param name="sceneXml">シーンXML</param>
        public void SaveSceneXmlToSTYLYTestMode(string sceneXml)
        {
            string srcPath = Path.Combine(Application.dataPath, TestModeSceneXml);
            // 一時ファイルに保存
            File.WriteAllText(srcPath, sceneXml);

            if (PackageName == null)
            {
                Debug.LogError("PackageName is null!!!");
            }

            string destPath = "/sdcard/Android/data/" + PackageName + "/files/STYLY_TESTMODE/" + TestModeSceneXml;

            // AndroidへPush
            PushFile(srcPath, destPath);

            // 一時ファイル削除
            File.Delete(srcPath);
        }

        /// <summary>
        /// AssetBundleをSTYLY_TESTMODEフォルダへコピーする。
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <returns>成否。 0: 成功, -1: 失敗</returns>
        public int CopyBuildedAssetBundleToSTYLYTestMode(string guid)
        {
            var paltformName = "Android";

            string srcPath = Path.Combine(OutputPath, "STYLY_ASSET", paltformName, guid);
            Debug.Log("assetBundlePath:" + srcPath);

            if (PackageName == null)
            {
                Debug.LogError("PackageName is null!!!");
                return -1;
            }

            string destPath = "/sdcard/Android/data/" + PackageName + "/files/STYLY_TESTMODE/" + paltformName + "/" + guid;
            Debug.Log("STYLYPath:" + destPath);
            var result = PushFile(srcPath, destPath);

            if (result != 0)
            {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// 文字列をダブルクォーテーションで囲む
        /// </summary>
        /// <param name="inputStr">入力文字列</param>
        /// <returns>出力文字列</returns>
        private string EncloseWithQuotation(string inputStr)
        {
            // 念のため前後の空白文字を除去する
            string trimmedStr = inputStr.Trim();

            // すでにダブルクォーテーションで囲まれてるならそのまま
            if (trimmedStr.StartsWith("\"") && trimmedStr.EndsWith("\""))
            {
                return trimmedStr;
            }

            // シングルクォーテーションで囲むとADBでエラーになるので、
            // ダブルクォーテーションで囲む
            return "\"" + trimmedStr + "\"";
        }
    }
}
