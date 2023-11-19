using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEditor;

namespace STYLY.Uploader
{
    [InitializeOnLoad]
    public class Editor
    {
        static Editor()
        {
            EditorApplication.update += Startup;
        }


        static void Startup()
        {
            EditorApplication.update -= Startup;

            bool requirementSatisfied = true;
            string email = EditorPrefs.GetString(UI.Settings.SETTING_KEY_STYLY_EMAIL);
            string api_key = EditorPrefs.GetString(UI.Settings.SETTING_KEY_STYLY_API_KEY);
            if (email.Length == 0 || api_key.Length == 0)
            {
                requirementSatisfied = false;
            }
            // http://answers.unity3d.com/questions/1324195/detect-if-build-target-is-installed.html
            var moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
            var isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            GUIStyle platformStyle = new GUIStyle();
            GUIStyleState platformStyleState = new GUIStyleState();
            platformStyleState.textColor = Color.red;
            platformStyle.normal = platformStyleState;
            if (!(bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.StandaloneWindows64 }) }))
            {
                requirementSatisfied = false;
            }
            if (!(bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.Android }) }))
            {
                requirementSatisfied = false;
            }
            if (!(bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.iOS }) }))
            {
                requirementSatisfied = false;
            }
            if (!(bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.StandaloneOSX }) }))
            {
                requirementSatisfied = false;
            }
            if (!(bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.WebGL }) }))
            {
                requirementSatisfied = false;
            }
            if (requirementSatisfied == false)
            {
                OpenSettings();
            }
        }


        private static bool isUploading = false;
        public static bool IsUploading { get { return isUploading; } }
        //http://baba-s.hatenablog.com/entry/2014/05/13/213143
        /// <summary>
        /// 選択中のPrefabアセットをアセットバンドルとしてビルドしてアップロードする。
        /// </summary>
        [MenuItem(@"Assets/STYLY/Upload prefab or scene to STYLY", false, 10000)]
        private static void BuildAndUpload()
        {
            bool isUpload = EditorUtility.DisplayDialog("Asset Upload",
             "Are you sure you want to Upload to STYLY ?", "Upload", "Cancel");
            if (!isUpload)
            {
              return;
            }
            // アセットのアップローディング中であることを表すフラグ
            // SceneProcessorがビルド対象シーンに処理を施していいかどうか判断する基準となる
            isUploading = true;

            try
            {
                BuildAndUploadImplement();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            // アップロードがおわったらフラグをオフ
            isUploading = false;
        }

        private static void BuildAndUploadImplement()
        {
            if (CheckSelectedObjectIsPrefab())
            {
                bool isError = false;
                // 選択中のPrefabアセットパス, アセット名を取得
                var assetList = new List<UnityEngine.Object>();
                assetList.AddRange(Selection.objects);
                var unprocessedAssetList = new List<UnityEngine.Object>();
                unprocessedAssetList.AddRange(Selection.objects);
                string errorMessages = "";

                int count = 0;
                int selectLength = Selection.objects.Length;
                for (count = 0; count < selectLength; count++)
                {
                    var selectObject = assetList[count];
                    Converter converter = new Converter(selectObject);

                    if (!converter.BuildAsset() || converter.error != null)
                    {
                        isError = true;
                        errorMessages += "Failed to upload object <" + selectObject.name + ">";
                        if (converter.error != null)
                        {
                            errorMessages += " : " + converter.error.message;
                        }
                        errorMessages += "\r\n";
                    }
                    else
                    {
                        Debug.Log(selectObject.name + " Upload Success!");
                        unprocessedAssetList.Remove(selectObject);
                    }
                }

                if (isError)
                {
                    // エラーが発生した場合、処理されていないオブジェクトを選択する。
                    Selection.objects = unprocessedAssetList.ToArray();
                    EditorUtility.ClearProgressBar();
                    bool isOpen = EditorUtility.DisplayDialog("Asset Upload failed", errorMessages, "More infomation", "Close");
                    if (isOpen) { Application.OpenURL(Config.UploadErrorUrl); }
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    bool isOpen = EditorUtility.DisplayDialog("Asset Upload succeeded", "Upload succeeded.", "Launch STYLY studio", "Close");
                    if (isOpen) { Application.OpenURL(Config.ListOfScenesUrl); }
                }
            }
        }

        /// <summary>
        /// 選択中のPrefab他アセットのサイズを調べます
        /// </summary>
        [MenuItem(@"Assets/STYLY/Check File Size", false, 10001)]
        private static void CheckFileSize()
        {
            if (CheckSelectedObjectIsPrefab())
            {
                var asset = Selection.objects[0];
                var assetPath = AssetDatabase.GetAssetPath(asset);
                string AssetBundlesOutputPath = Config.OutputPath + "STYLY_ASSET";
                //フォルダのクリーンアップ
                Converter.Delete(AssetBundlesOutputPath);
                //パッケージ形式でExport
                if (!Directory.Exists(AssetBundlesOutputPath + "/Packages/"))
                    Directory.CreateDirectory(AssetBundlesOutputPath + "/Packages/");
                var exportPackageFile = AssetBundlesOutputPath + "/Packages/" + "temp_for_for_filesize_check" + ".unitypackage";
                AssetDatabase.ExportPackage(assetPath, exportPackageFile, ExportPackageOptions.IncludeDependencies);
                //ファイルサイズチェック
                System.IO.FileInfo fi = new System.IO.FileInfo(exportPackageFile);
                long fileSize = fi.Length;
                //Debug.Log("File Size: " + fileSize.ToString("#,0") + " Byte");
                if (fileSize < 1024 * 1024)
                {
                    Editor.ShowFileSizeDialog(((fileSize / 1024).ToString("#,0") + " KByte"));
                }
                else
                {
                    Editor.ShowFileSizeDialog(((fileSize / (1024 * 1024)).ToString("#,0") + " MByte"));
                }
            }
        }

        [MenuItem(@"Assets/STYLY/Settings", false, 10002)]
        private static void OpenSettings()
        {
            var settings = ScriptableObject.CreateInstance<UI.Settings>();
            settings.Show();
        }

        [MenuItem("STYLY/Asset Uploader Settings")]
        static void ShowSettingView()
        {
            var settings = ScriptableObject.CreateInstance<UI.Settings>();
            settings.Show();
        }

        static bool CheckSelectedObjectIsPrefab()
        {
            // check only one prefab selected
            Error error = null;
            if (Selection.objects.Length == 0)
            {
                error = new Error("There is no prefab selected.");
            }
            if (error != null)
            {
                error.ShowDialog();
                return false;
            }
            return true;
        }

        public static void ShowUploadProgress(string description, float t)
        {
            //			EditorUtility.DisplayProgressBar ("STYLY Asset Uploader", description, t);
            //			if (t >= 1f) {
            //				EditorUtility.ClearProgressBar ();
            //			}
        }

        public static void ShowWaringDialog(string description)
        {
            ShowDialog("Warning", description, "OK");
        }

        public static void ShowErrorDialog(string description)
        {
            ShowDialog("Asset Upload failed", description, "OK");
        }

        public static void ShowFileSizeDialog(string size)
        {
            ShowDialog("Asset File Size", size, "OK");
        }

        public static void ShowUploadSucessDialog()
        {
            ShowDialog("Asset Upload", "Upload succeeded.", "OK");
        }

        private static void ShowDialog(string title, string description, string buttonName)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog(title, description, buttonName);
        }
    }
}
