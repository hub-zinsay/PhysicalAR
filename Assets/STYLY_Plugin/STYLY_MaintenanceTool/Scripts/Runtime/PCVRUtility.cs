using System.IO;
using UnityEngine;

namespace STYLY.MaintenanceTool.Utility
{
    /// <summary>
    /// PCVR(Steam)に関するUtilityクラス
    /// </summary>
    public class PCVRUtility : IPlatformUtility
    {
        /// <summary>AssetBundleのキャッシュフォルダ</summary>
        private const string OutputPath = "_Output";

        /// <summary>テストモード時に端末へ送るXMLファイル名</summary>
        private const string TestModeSceneXml = "TestModeScene.xml";

        /// <summary>
        /// STYLYを起動する
        /// </summary>
        /// <returns>成否。 0: 成功, -1: 失敗</returns></returns>
        public int StartSTYLY()
        {
            // PCでは初回起動不要なため無効にする。
            //			return StartSTYLY("steam://rungameid/693990");
            return 0;
        }

        /// <summary>
        /// URLを指定してSTYLYを起動する
        /// </summary>
        /// <param name="urlScheme">URL</param>
        /// <returns>成否。 0: 成功, -1: 失敗</returns>
        public int StartSTYLY(string urlScheme)
        {
            var command = "rundll32.exe";
            string[] args = { "url.dll,FileProtocolHandler", urlScheme };
            string outputStr, errorStr;
            var exitCode = RunExternalProcessSync(command, args, out outputStr, out errorStr);

            return exitCode;
        }

        public int RunExternalProcessSync(string command, string[] argsArray, out string outputString, out string errorString)
        {
            var commonUtility = new CommonUtility();
            return commonUtility.RunExternalProcessSync(command, argsArray, out outputString, out errorString);
        }

        private const string STYLY_DATA_PATH = "/Psychic VR Lab/STYLY";
        public string GetSTYLYPersistentDataPath()
        {
            var dataPath = Application.persistentDataPath;
            Debug.Log("persistentDataPath:" + dataPath);
            // C:/Users/[UserName]/AppData/LocalLow/[CompanyName]/[ProjectName]
            // が取れるので、STYLYのものに読み替える。
            var length = dataPath.LastIndexOf("/");
            length = dataPath.LastIndexOf("/", length - 1);
            var result = dataPath.Substring(0, length) + STYLY_DATA_PATH;

            Debug.Log(result);
            return result;
        }

        /// <summary>
        /// ファイルをsrcからdestへコピーする
        /// </summary>
        /// <param name="srcPath">コピー元ファイルパス</param>
        /// <param name="destPath">コピー先ファイルパス</param>
        /// <returns>成否。 0: 成功, -1: 失敗</returns>
        public int PushFile(string srcPath, string destPath)
        {
            Debug.Log("Push File: src:" + srcPath + " dest:" + destPath);
            if (File.Exists(srcPath))
            {
                var dirName = Path.GetDirectoryName(destPath);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }

                if (!File.Exists(destPath))
                {
                    File.Copy(srcPath, destPath);
                }
            }
            else if (Directory.Exists(srcPath))
            {
                CopyDirectory(srcPath, destPath);
            }

            return 0;
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

            var urlScheme = "styly-steam://styly.world" + "/" + userName + "/" + guid;

            // テストモードならクエリを追加する
            if (testMode)
            {
                urlScheme += "?testmode=true";
            }

            return urlScheme;
        }

        public static void CopyDirectory(string srcPath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
                File.SetAttributes(destPath, File.GetAttributes(srcPath));
            }

            if (destPath[destPath.Length - 1] != Path.DirectorySeparatorChar)
            {
                destPath = destPath + Path.DirectorySeparatorChar;
            }

            string[] files = Directory.GetFiles(srcPath);

            foreach (string file in files)
            {
                File.Copy(file, destPath + Path.GetFileName(file), true);
            }

            string[] dirs = Directory.GetDirectories(srcPath);
            foreach (string dir in dirs)
            {
                CopyDirectory(dir, destPath + Path.GetFileName(dir));
            }
        }

        /// <summary>
        /// STYLY_TESTMODEフォルダのファイルをすべて削除する
        /// </summary>
        public void ClearSTYLYTestMode()
        {
            string directoryPath = Path.Combine(GetSTYLYPersistentDataPath(), "STYLY_TESTMODE");

            if (Directory.Exists(directoryPath))
            {
                // サブディレクトリも含めて再帰的に削除
                Directory.Delete(directoryPath, true);
            }

            // 空のディレクトリを作る
            Directory.CreateDirectory(directoryPath);
        }

        /// <summary>
        /// シーンXMLファイルをSTYLY_TESTMODEフォルダへ出力する
        /// </summary>
        /// <param name="sceneXml">シーンXML</param>
        public void SaveSceneXmlToSTYLYTestMode(string sceneXml)
        {
            string destPath = Path.Combine(GetSTYLYPersistentDataPath(), "STYLY_TESTMODE", TestModeSceneXml);

            string directoryPath = Path.GetDirectoryName(destPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(destPath, sceneXml);
        }

        /// <summary>
        /// AssetBundleをSTYLY_TESTMODEフォルダへコピーする。
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <returns>成否。 0: 成功, -1: 失敗</returns>
        public int CopyBuildedAssetBundleToSTYLYTestMode(string guid)
        {
            string paltformName = "Windows";

            string srcPath = Path.Combine(OutputPath, "STYLY_ASSET", paltformName, guid);
            Debug.Log("assetBundlePath:" + srcPath);

            string destPath = Path.Combine(GetSTYLYPersistentDataPath(), "STYLY_TESTMODE", paltformName, guid);
            Debug.Log("STYLYPath:" + destPath);

            if (!File.Exists(srcPath))
            {
                Debug.LogError("Error:Src File Not Found. " + srcPath);
                return -1;
            }

            var result = PushFile(srcPath, destPath);

            if (result != 0)
            {
                return -1;
            }

            return 0;
        }
    }
}

