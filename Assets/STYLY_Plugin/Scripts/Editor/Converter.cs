using STYLY.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.Networking;
using UnityEditor.SceneManagement;

namespace STYLY.Uploader
{
    public class Converter
    {
        /// <summary>対象オブジェクト</summary>
        private UnityEngine.Object asset;
        
        /// <summary>対象オブジェクトのファイルパス</summary>
        private string assetPath;
        
        /// <summary>対象オブジェクトのタイトル</summary>
        private string assetTitle;
        
        /// <summary>ユーザーのメールアドレス</summary>
        private string email;
        
        /// <summary>ユーザーのAPIキー</summary>
        private string apiKey;
        
        /// <summary>azcopyの有効化フラグ</summary>
        private bool azcopyEnable;
        
        /// <summary>Unityのバージョン</summary>
        private string unityVersion;
        
        /// <summary>対象オブジェクトのUnity側のアセットGUID</summary>
        private string unityGuid;
        
        /// <summary>管理者かどうか</summary>
        private bool isAdministrator = false;

        /// <summary>対象オブジェクトのSTYLY側のアセットGUID</summary>
        private string stylyGuid;

        /// <summary>ハッシュ値</summary>
        private string stylyHash;

        /// <summary>対象オブジェクトのプロットフォーム毎のURLのリスト</summary>
        private Dictionary<string, string> signedUrls = new Dictionary<string, string>();

        /// <summary>エラー</summary>
        public Error error;

        /// <summary>対象オブジェクトをリネームした場合のファイルパス</summary>
        private string renamedAssetPath;

        /// <summary>
        /// azcopy非利用時のアップロードリトライ回数
        /// </summary>
        const int uploadHttpRetryLimit = 3;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="asset">対象のオブジェクト</param>
        public Converter(UnityEngine.Object asset)
        {
            this.email = EditorPrefs.GetString(UI.Settings.SETTING_KEY_STYLY_EMAIL);
            this.apiKey = EditorPrefs.GetString(UI.Settings.SETTING_KEY_STYLY_API_KEY);
            this.azcopyEnable = Config.IsAzCopyEnabled;
            
            isAdministrator = this.email.Equals("info@styly.cc");

            /*
            現在、unityGuidが同じでアカウントがinfoの場合、サーバーが同じSTYLY-GUIDを返す仕様になっているため、unityGuidをユニークに発番している。サーバー側の仕様を変更して、常に新しいSTYLY-GUIDを発番するようにすること
            this.unityGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
            */
            this.unityGuid = Guid.NewGuid().ToString("D");

            this.asset = asset;
            this.assetPath = AssetDatabase.GetAssetPath(this.asset);
            this.assetTitle = System.IO.Path.GetFileNameWithoutExtension(this.assetPath);
            this.unityVersion = UnityEngine.Application.unityVersion;
        }

        /// <summary>
        /// アセットをビルドする
        /// </summary>
        /// <returns></returns>
        public bool BuildAsset()
        {
            try
            {
                return ProcessBuildAsset();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error Building & Uploading Asset: {e.Message} StackTrace: {e.StackTrace}");
                return false;
            }
            finally
            {
                // ProcessBuildAssetの後処理
                OnFinishBuildAsset();
            }
        }

        /// <summary>
        /// アセットをビルドする処理
        /// </summary>
        /// <returns>成否</returns>
        private bool ProcessBuildAsset()
        {
            CheckError();
            if (error != null)
            {
                return false;
            }

            GetAzureSignedUrls();
            if (error != null)
            {
                return false;
            }

            // シーンアセットの場合、アクティブシーンのisDirtyを解消する
            var isScene = IsSceneAsset(assetPath);
            if (isScene)
            {
                var result = ResolveSceneIsDirty();
                if (result == false)
                {
                    Debug.Log("Active scene is still dirty");
                    // アクティブシーンのisDirtyは解消されていない場合、処理を中断する。
                    error = new Error(Environment.NewLine + "Saving the active scene is failed or cancelled by user.");
                    return false;
                }
            }

            RenameAssetForBuild();

            //フォルダのクリーンアップ
            Delete(Config.OutputPath + "STYLY_ASSET");

            CopyPackage();
            if (error != null)
            {
                return false;
            }

            MakeThumbnail();

            StylyAnalyticsForUnityPlugin.Instance.StartBuildAsset(stylyGuid, isScene);

            // 全てのプラットフォームをビルドする。
            var resultBuildAll = Config.PlatformList.All(Build);
            if (!resultBuildAll)
            {
                // 一つでも失敗したら終了。
                return false;
            }

            StylyAnalyticsForUnityPlugin.Instance.FinishBuildAsset();

            if (!UploadAssets())
            {
                return false;
            }

            if (error != null)
            {
                return false;
            }

            StylyAnalyticsForUnityPlugin.Instance.FinishUploadAsset();

            PostUploadComplete();

            StylyAnalyticsForUnityPlugin.Instance.RecordAsset();

            return error == null;
        }
        
        /// <summary>
        /// ProcessBuildAssetの後処理 
        /// </summary>
        private void OnFinishBuildAsset()
        {
            // Prefab名を戻す
            // renamedAssetPathが存在する場合は、対象オブジェクトのリネーム後なので元に戻す。
            if (!string.IsNullOrEmpty(this.renamedAssetPath))
            {
                AssetDatabase.MoveAsset(this.renamedAssetPath, assetPath);
                AssetDatabase.Refresh();
            }

            if (isAdministrator)
            {
                Debug.Log("[Admin]STYLY_GUID: " + this.stylyGuid);
            }

            DeleteCamera();
        }

        /// <summary>
        /// 対象オブジェクトを検査する
        /// </summary>
        public void CheckError()
        {
            // Validate AzCopy
            if (azcopyEnable && !AzcopyExecutableFileInstaller.Validate())
            {
                error = new Error("AzCopy enabled but exec file not found");
                return;
            }

            // unity version check
            if (!UnityVersionChecker.Check(Application.unityVersion))
            {
                Debug.Log("isUnSupportedVersion");
                this.error = new Error("Please use Unity " + string.Join(", ", Config.UNITY_VERSIONS) + ".");
                return;
            }

            if (this.email == null || this.email.Length == 0
                || this.asset == null || this.apiKey.Length == 0)
            {
                Debug.Log("You don't have a account settings. [STYLY/Asset Uploader Settings]");
                this.error = new Error("You don't have a account settings. [STYLY/Asset Uploader Settings]");
                return;
            }

            //もしSTYLYアセット対応拡張子ではなかったらエラー出して終了
            if (Array.IndexOf(Config.AcceptableExtentions, System.IO.Path.GetExtension(assetPath)) < 0)
            {
                Debug.Log("Unsupported format ");
                this.error = new Error("Unsupported format " + System.IO.Path.GetExtension(assetPath));
                return;
            }

            string assetType = this.asset.GetType().ToString();
            if ((!isAdministrator) && (assetType == "UnityEngine.GameObject"))
            {
                GameObject assetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                Transform[] childTransforms = assetObj.GetComponentsInChildren<Transform>();
                foreach (Transform t in childTransforms)
                {
                    //禁止アセットのチェック
                    if (Config.ProhibitedTags.Contains(t.gameObject.tag))
                    {
                        this.error = new Error(string.Format("{0} can not be used as a tag.", t.tag));
                        return;
                    }
                    //禁止レイヤーのチェック
                    //Defaultレイヤー以外使用できない
                    if (t.gameObject.layer != 0)
                    {
                        this.error = new Error("Object needs to be in Default layer. You use " + LayerMask.LayerToName(t.gameObject.layer));
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// ウェブサーバーにフォームデータをPOSTして結果を得る関数
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="formCollection">フォームパラメーター</param>
        /// <example>
        /// 利用方法
        /// <code>
        ///     var t = PostToServer(Config.AzureSignedUrls, form);
        ///     while (t.MoveNext()){
        ///         // Do Nothing.
        ///     }
        ///     var responseString = t.Current?.ToString().Trim();
        /// </code>
        /// </example>
        private IEnumerator PostToServer(string url, NameValueCollection formCollection)
        {
            var form = new WWWForm();
            foreach (string key in formCollection)
            {
                form.AddField(key, formCollection[key]);
            }

            using (var request = UnityWebRequest.Post(url, form))
            {
                request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

                yield return request.SendWebRequest();
                while (!request.isDone)
                {
                    yield return 0;
                }

                if (request.isNetworkError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    if (request.responseCode == 200)
                    {
                        var response = request.downloadHandler.text;
                        yield return response;
                        yield break;
                    }

                    // ダイアログに表示するメッセージ
                    string errorMessage;
                    // コンソールに表示するメッセージ
                    string errorLogMessage;
                    if (request.responseCode == 400)
                    {
                        const string incorrectAccountSettingMessage = "The API key or the email address is incorrect.";
                        errorMessage = $"{Environment.NewLine}{Environment.NewLine}{incorrectAccountSettingMessage}";
                        errorLogMessage =
                            $"{incorrectAccountSettingMessage}{MakeDisplayMessageFromResponse(request)}";
                    }
                    else
                    {
                        errorMessage =
                            $" Error cannot complete. responseCode:{request.responseCode}{MakeDisplayMessageFromResponse(request)}";
                        errorLogMessage = errorMessage;
                    }

                    error = new Error(errorMessage);
                    Debug.LogError(errorLogMessage);
                }
            }
        }

        string MakeDisplayMessageFromResponse(UnityWebRequest request)
        {
            return (request.downloadHandler != null)
                ? $"{Environment.NewLine}Response message:{request.downloadHandler.text}"
                : "";
        }

        /// <summary>
        /// AzureのURLを取得する
        /// </summary>
        private void GetAzureSignedUrls()
        {
            var form = new NameValueCollection
            {
                ["unity_version"] = unityVersion,
            };

            try
            {
                // Post to server
                var t = PostToServer(Config.AzureSignedUrls, form);
                while (t.MoveNext())
                {
                    // Do Nothing.
                }

                var responseString = t.Current?.ToString().Trim();

                var responseJson = Json.Deserialize(responseString);
                if (!(responseJson is Dictionary<string, object> dict))
                {
                    Debug.LogError("Failed to get API response.");
                    return;
                }

                stylyGuid = (string)dict["id"];
                stylyHash = (string)dict["asset_register_hash"];
                signedUrls.Add("Package", (string)dict["package_url"]);
                signedUrls.Add("Thumbnail", (string)dict["thumbnail_url"]);
                var platforms = (Dictionary<string, object>)dict["platform"];
                signedUrls.Add("Android", (string)platforms["Android"]);
                signedUrls.Add("iOS", (string)platforms["iOS"]);
                signedUrls.Add("OSX", (string)platforms["OSX"]);
                signedUrls.Add("Windows", (string)platforms["Windows"]);
                signedUrls.Add("WebGL", (string)platforms["WebGL"]);

                if (isAdministrator)
                {
                    Debug.Log("[Admin]" + Json.Serialize(signedUrls));
                }
            }
            catch (WebException e)
            {
                var errorMessage = getErrorMessageFromResponseJson(e);
                Debug.LogError("[STYLY]" + errorMessage);
                error = new Error(errorMessage + " \n\nAuthentication failed." + e.Message);
            }
        }

        /// <summary>
        /// エラーが出た際、JSONにErrorメッセージがあれば取得する
        /// サーバーはStatus400を返すため本文にJSONがあってもWebExceptionが発生します
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private string getErrorMessageFromResponseJson(WebException e)
        {
            string ret = "";
            try
            {
                Stream s = e.Response.GetResponseStream();
                StreamReader sr = new StreamReader(s);
                string responseString = sr.ReadToEnd();
                Dictionary<string, object> dict = Json.Deserialize(responseString) as Dictionary<string, object>;
                if (dict.ContainsKey("error"))
                {
                    ret = (string)dict["error"];
                }
            }
            catch
            {
                ret = "Unknown error";
            }
            return ret;
        }


        /// <summary>
        /// Prefab名をSTYLY GUIDに一時的に変更
        /// </summary>
        private void RenameAssetForBuild()
        {
            Debug.Log("RenameAssetForBuild");

            var activeScenePath = EditorSceneManager.GetActiveScene().path;

            this.renamedAssetPath = System.IO.Path.GetDirectoryName(assetPath) + "/" + stylyGuid + System.IO.Path.GetExtension(assetPath);
            AssetDatabase.MoveAsset(assetPath, this.renamedAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            // ActiveSceneをRenameした場合は開きなおす
            if (activeScenePath.Equals(assetPath))
            {
                EditorSceneManager.OpenScene(this.renamedAssetPath);
            }
        }

        /// <summary>
        /// パッケージをコピーする
        /// </summary>
        private void CopyPackage()
        {
            Debug.Log("CopyPackage");


            //パッケージ形式でバックアップ用にExport
            if (!Directory.Exists(Config.OutputPath + "/STYLY_ASSET/Packages/"))
                Directory.CreateDirectory(Config.OutputPath + "/STYLY_ASSET/Packages/");

            var exportPackageFile = Config.OutputPath + "/STYLY_ASSET/Packages/" + this.stylyGuid + ".unitypackage";

            // これを入れないとMacだと落ちる。
            // なぜなら、AssetDatabase.ExportPackage メソッドなどがプログレスウィンドウを表示しようとするので、すでにプログレスウィンドウがあると問題になる模様。
            EditorUtility.ClearProgressBar();

            AssetDatabase.ExportPackage(this.renamedAssetPath, exportPackageFile, ExportPackageOptions.IncludeDependencies);

            //ファイルサイズチェック
            System.IO.FileInfo fi = new System.IO.FileInfo(exportPackageFile);
            long fileSize = fi.Length;

            float limitSize = azcopyEnable ? Config.LIMIT_PACKAGE_FILE_SIZE_MB_AZCOPY : Config.LIMIT_PACKAGE_FILE_SIZE_MB;
            if (fileSize >= limitSize * 1024 * 1024)
            {
                this.error = new Error("Size over :Maximum size is " + limitSize + "MB. Your file size is " + (fileSize / (1024 * 1024)).ToString("#,0") + " MB");
            }
        }
        
        /// <summary>
        /// サムネイルを作成する
        /// </summary>
        private void MakeThumbnail()
        {
            Debug.Log("MakeThumbnail");

            var thumbnailOutputDirPath = $"{Config.OutputPath}/STYLY_ASSET/Thumbnails/";
            if (!Directory.Exists(thumbnailOutputDirPath)) { Directory.CreateDirectory(thumbnailOutputDirPath); }

            //Dummyサムネイルのコピー
            File.Copy( $"{Application.dataPath}/STYLY_Plugin/Resources/dummy_thumbnail.png", $"{thumbnailOutputDirPath}/{stylyGuid}.png");

            try
            {
                string thumbnailPath = $"{Directory.GetParent(Application.dataPath)}/{thumbnailOutputDirPath}/{stylyGuid}.png";
                if (IsSceneAsset(renamedAssetPath))
                {
                    Thumbnail.MakeThumbnailForScene(renamedAssetPath, thumbnailPath);
                }
                else
                {
                    Thumbnail.MakeThumbnailForPrefab(renamedAssetPath, thumbnailPath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// アップロード後にサーバーに通知
        /// </summary>
        private void PostUploadComplete()
        {
            try
            {
                var form = new NameValueCollection
                {
                    ["unity_version"] = unityVersion,
                    ["title"] = assetTitle,
                    ["asset_register_hash"] = stylyHash
                };

                var url = string.Format(Config.StylyAssetUploadCompleteUrl, stylyGuid);

                // Post to server
                var t = PostToServer(url, form);
                while (t.MoveNext())
                {
                    // Do Nothing.
                }
            }
            catch (Exception e)
            {
                error = new Error(e.Message);
            }
        }

        /// <summary>
        /// アセットをアップロードする
        /// </summary>
        /// <returns>成否</returns>
        private bool UploadAssets()
        {
            Debug.Log("UploadAssets");

            // 進捗率表示用
            var progress = 0.1f;

            var assetBundlesOutputPath = Config.OutputPath + "STYLY_ASSET";

            var uploadResult = true;
            uploadResult = UploadToStorage(signedUrls["Package"],
                Directory.GetParent(Application.dataPath) + "/" + assetBundlesOutputPath + "/Packages/" +
                stylyGuid + ".unitypackage", progress);

            if (uploadResult == false)
            {
                return false;
            }

            progress = 0.2f;
            uploadResult = UploadToStorage(signedUrls["Thumbnail"],
                Directory.GetParent(Application.dataPath) + "/" + assetBundlesOutputPath + "/Thumbnails/" +
                stylyGuid + ".png", progress);

            if (uploadResult == false)
            {
                return false;
            }

            progress = 0.3f;
            var progressStep = 0.8f / (Config.PlatformList.Length);
            foreach (var platform in Config.PlatformList)
            {
                var platformString = GetPlatformName(platform);
                var targetBlobUrl = signedUrls[platformString];
                var filePath = Directory.GetParent(Application.dataPath) + "/" + assetBundlesOutputPath +
                               "/" + platformString + "/" + stylyGuid;
                uploadResult = UploadToStorage(targetBlobUrl, filePath, progress);
                progress += progressStep;
                if (uploadResult == false)
                {
                    return false;
                }
            }

            return true;
        }

        // return true if cancelled
        bool ShowProgressBar(bool useAzcopy, float progress)
        {
            return EditorUtility.DisplayCancelableProgressBar(useAzcopy ? "Uploading... (azcopy)" : "Uploading...", "", progress);
        }

        /// <summary>
        /// azcopyまたはhttpを使ってAzureストレージにアップロードする
        /// </summary>
        /// <param name="targetBlobUrl">アップロード先URL</param>
        /// <param name="filePath">アップロード元ファイルパス</param>
        /// <param name="progress">進捗率</param>
        /// <returns>成否</returns>
        private bool UploadToStorage(string targetBlobUrl, string filePath, float progress)
        {
            ShowProgressBar(azcopyEnable, progress);
            try
            {
                if (azcopyEnable)
                {
                    // AZCOPYでアップロード
                    if (!UploadToStorageWithAzcopy(targetBlobUrl, filePath))
                    {
                        return false;
                    }
                }
                else
                {
                    // リトライつきでHTTPアップロード
                    if (!RetryUtil.Retry(
                        () => UploadToStorageWithHttp(targetBlobUrl, filePath),
                        uploadHttpRetryLimit, 1000))
                    {
                        return false;
                    }
                }
                if (ShowProgressBar(azcopyEnable, progress))
                {
                    // cancelled
                    return false;
                }
                return true;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// azcopyコマンドを使ってAzureにアップロードする
        /// </summary>
        /// <param name="url">アップロード先URL</param>
        /// <param name="filePath">アップロード元ファイルパス</param>
        /// <returns>成否</returns>
        private bool UploadToStorageWithAzcopy(string url, string filePath)
        {
            var stdout = new StringBuilder();
            var stderr = new StringBuilder();

            var p = new Process();
            p.EnableRaisingEvents = true;
            p.StartInfo.FileName = new FileInfo(Config.GetInternalAzcopyPath()).FullName;
            p.StartInfo.Arguments = $"copy \"{filePath}\" \"{url}\" --block-size-mb={Config.AZCOPY_BLOCK_SIZE_MB} {Config.AZCOPY_OTHER_ARGS}";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.EnvironmentVariables.Add("AZCOPY_CONCURRENCY_VALUE", Config.AZCOPY_CONCURRENCY_VALUE);

            p.OutputDataReceived += (sender, e) => { if (e.Data != null) { stdout.AppendLine(e.Data); } };
            p.ErrorDataReceived += (sender, e) => { if (e.Data != null) { stderr.AppendLine(e.Data); } };

            try
            {
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                Debug.LogError("azcopy execution error: " + e.Message);
                return false;
            }

            p.WaitForExit();
            p.CancelOutputRead();
            p.CancelErrorRead();

            Debug.Log("azcopy stdout:" + stdout);
            if (p.ExitCode != 0)
            {
                Debug.LogError("executing azcopy failed. exitcode:" + p.ExitCode + " stderr:" + stderr);
                return false;
            }

            Debug.Log("uploading with azcopy succeeded.");
            return true;
        }

        /// <summary>
        /// HTTPを使ってアップロードする
        /// </summary>
        /// <param name="url">アップロード先URL</param>
        /// <param name="filePath">アップロード元ファイルパス</param>
        /// <returns>成否</returns>
        private bool UploadToStorageWithHttp(string url, string filePath)
        {
            try
            {
                byte[] myData = LoadBinaryData(filePath);

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.CheckCertificateRevocationList = false;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;

                HttpWebRequest client = WebRequest.Create(url) as HttpWebRequest;
                client.Timeout = 10 * 60 * 1000;
                client.KeepAlive = true;
                client.Method = "PUT";
                client.ContentLength = myData.Length;
                client.Credentials = CredentialCache.DefaultCredentials;
                client.Headers.Add("x-ms-blob-type", "BlockBlob");
                client.Headers.Add("x-ms-date", DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture));

                using (Stream requestStream = client.GetRequestStream())
                {
                    requestStream.Write(myData, 0, myData.Length);
                }

                using (HttpWebResponse res = client.GetResponse() as HttpWebResponse)
                {
                    bool isOK = true;
                    if (res.StatusCode != HttpStatusCode.Created)
                    {
                        isOK = false;
                    }
                    using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                    {
                        string data = reader.ReadToEnd();
                        if (!isOK)
                        {
                            this.error = new Error(data, new Dictionary<string, string> {
                                { "url", url },
                                { "file_path", filePath }
                            });
                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("UploadToStorageWithHttp error: " + e);
                this.error = new Error(e);
                return false;
            }
            return true;
        }

        private static bool RemoteCertificateValidationCallback(object sender,
                                                                 System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                                                 System.Security.Cryptography.X509Certificates.X509Chain chain,
                                                                 System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


        static byte[] LoadBinaryData(string path)
        {


            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open);
            }
            catch (FileNotFoundException e)
            {
                if (path.IndexOf("/Thumbnails/") >= 0)
                {
                    fs = new FileStream(Application.dataPath + "/STYLY_Plugin/Resources/dummy_thumbnail.png", FileMode.Open);
                }
                else
                {
                    throw e;
                }
            }

            BinaryReader br = new BinaryReader(fs);
            byte[] buf = br.ReadBytes((int)br.BaseStream.Length);
            br.Close();
            return buf;
        }

        /// <summary>
        /// 対象プラットフォーム毎にアセットバンドルを作成する
        /// </summary>
        /// <param name="platform">対象プラットフォーム</param>
        /// <returns>成否</returns>
        public bool Build(RuntimePlatform platform)
        {
            Debug.Log("Build");

            //対象プラットフォームごとに出力フォルダ作成
            string outputPath = Path.Combine(Config.OutputPath + "STYLY_ASSET", GetPlatformName(platform));
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            bool switchResult = true;

            if (platform == RuntimePlatform.WindowsPlayer)
            {
                // プラットフォームとGraphic APIを常に同じにする
                switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                //PlayerSettings.colorSpace = ColorSpace.Gamma;
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new UnityEngine.Rendering.GraphicsDeviceType[] {
					//UnityEngine.Rendering.GraphicsDeviceType.Direct3D9,
					UnityEngine.Rendering.GraphicsDeviceType.Direct3D11
                });
            }
            else if (platform == RuntimePlatform.Android)
            {

                switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

                //PlayerSettings.colorSpace = ColorSpace.Gamma;
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new UnityEngine.Rendering.GraphicsDeviceType[] {
                    UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2,
                    UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3
                });
            }
            else if (platform == RuntimePlatform.IPhonePlayer)
            {
                switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                //PlayerSettings.colorSpace = ColorSpace.Gamma;
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new UnityEngine.Rendering.GraphicsDeviceType[] {
                    UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2,
                    UnityEngine.Rendering.GraphicsDeviceType.Metal
                });
            }
            else if (platform == RuntimePlatform.OSXPlayer)
            {
                switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
                //PlayerSettings.colorSpace = ColorSpace.Gamma;
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneOSX, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneOSX, new UnityEngine.Rendering.GraphicsDeviceType[] {
                    UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2,
                    UnityEngine.Rendering.GraphicsDeviceType.Metal
                });
            }
            else if (platform == RuntimePlatform.WebGLPlayer)
            {

                switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
                //PlayerSettings.colorSpace = ColorSpace.Gamma;
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, true);
                // web gl 1.0, web gl 2.0 がUnityEngine.Rendering.GraphicsDeviceTypeにないからautoで設定している
            }
            else if (platform == RuntimePlatform.WSAPlayerX86)
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.WSA, ScriptingImplementation.WinRTDotNET);
            }

            if (switchResult == false)
            {
                this.error = new Error("Can not switch Build target to " + GetPlatformName(platform) + ".\n"
                + "Make sure you have installed the target build module.\n"
                + "This tool requires Android, iOS, OSX, WebGL, Windows platforms.");
                return false;
            }

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = this.stylyGuid;
            buildMap[0].assetNames = new string[] { this.renamedAssetPath };

            AssetBundleManifest buildResult = BuildPipeline.BuildAssetBundles(outputPath, buildMap, BuildAssetBundleOptions.ChunkBasedCompression, GetBuildTarget(platform));
            if (buildResult == null)
            {
                this.error = new Error("Buid asset bundle failed for platform " + GetPlatformName(platform));
                return false;
            }
            return true;
        }

        private void DeleteCamera()
        {


            UnityEditor.EditorApplication.delayCall += () =>
            {
                var deleteCamera = GameObject.Find(Thumbnail.STYLY_Thumbnail_Camera_Name);
                if (deleteCamera)
                {
                    UnityEngine.Object.DestroyImmediate(deleteCamera);
                }
                var deleteObject = GameObject.Find(Thumbnail.STYLY_Thumbnail_Object_Name);
                if (deleteObject)
                {
                    UnityEngine.Object.DestroyImmediate(deleteObject);
                }
            };
        }

        public BuildTarget GetBuildTarget(RuntimePlatform platform)
        {
            Debug.Log("GetBuildTarget");


            switch (platform)
            {
                case RuntimePlatform.Android:
                    return BuildTarget.Android;
                case RuntimePlatform.IPhonePlayer:
                    return BuildTarget.iOS;
                case RuntimePlatform.WebGLPlayer:
                    return BuildTarget.WebGL;
                case RuntimePlatform.WSAPlayerX86:
                    return BuildTarget.WSAPlayer;
                case RuntimePlatform.WindowsPlayer:
                    return BuildTarget.StandaloneWindows64;
                case RuntimePlatform.OSXPlayer:
                    return BuildTarget.StandaloneOSX;
                default:
                    return BuildTarget.StandaloneWindows64;
            }
        }


        public string GetPlatformName(RuntimePlatform platform)
        {


            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WSAPlayerX86:
                    return "UWP";
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    return "OSX";
                default:
                    return null;
            }
        }

        /// <summary>
        /// 指定したディレクトリとその中身を全て削除する
        /// http://kan-kikuchi.hatenablog.com/entry/DirectoryProcessor
        /// </summary>
        public static void Delete(string targetDirectoryPath)
        {
            Debug.Log("Delete");

            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            //ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            //ディレクトリの中のディレクトリも再帰的に削除
            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                Delete(directoryPath);
            }

            //中が空になったらディレクトリ自身も削除
            Directory.Delete(targetDirectoryPath, false);
        }

        /// <summary>
        /// アクティブシーンのisDirtyを解消する
        /// </summary>
        /// <returns>true:保存成功 false:保存失敗（ユーザーキャンセルを含む）</returns>
        static bool ResolveSceneIsDirty()
        {
            if (!EditorSceneManager.GetActiveScene().isDirty)
            {
                return true;
            }
            
            // アクティブシーンが未保存なら保存する。
            Debug.Log("Saving the active scene because it's dirty.");
            return EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }

        /// <summary>
        /// シーンアセットかどうかをパスから判定する
        /// </summary>
        /// <param name="path">アセットへのファイルパス</param>
        /// <returns>true:シーンアセット false:シーンアセットではない</returns>
        static bool IsSceneAsset(string path)
        {
            return Path.GetExtension(path).ToLower().Equals(".unity");
        }
    }
}
