using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace STYLY.Uploader
{
    public class Config
    {
        public static readonly string CurrentVersion = "1.7.3";

        public static readonly string[] UNITY_VERSIONS = {"2019.3", "2019.4"};

        /// <summary>
        /// STYLYアセット対象プラットフォームリスト
        /// アセットバンドルのビルド対象として利用
        /// </summary>
        public static readonly RuntimePlatform[] PlatformList = 
        {
            RuntimePlatform.Android,
            RuntimePlatform.IPhonePlayer,
            RuntimePlatform.OSXPlayer,
            RuntimePlatform.WebGLPlayer,
            RuntimePlatform.WindowsPlayer,
            // RuntimePlatform.WSAPlayerX86, // UWP用
        };

        /// <summary>
        /// RuntimePlatformとBuildTargetの対応Dictionary
        /// </summary>
        public static readonly Dictionary<RuntimePlatform, BuildTarget> PlatformBuildTargetDic =
            new Dictionary<RuntimePlatform, BuildTarget>
            {
                {RuntimePlatform.Android, BuildTarget.Android},
                {RuntimePlatform.IPhonePlayer, BuildTarget.iOS},
                {RuntimePlatform.OSXPlayer, BuildTarget.StandaloneOSX},
                {RuntimePlatform.WebGLPlayer, BuildTarget.WebGL},
                {RuntimePlatform.WindowsPlayer, BuildTarget.StandaloneWindows64},
                {RuntimePlatform.WSAPlayerX86, BuildTarget.WSAPlayer},
            };

        /// <summary>
        /// STYLYアセットに変換可能な拡張子一覧
        /// </summary>
        public static readonly string[] AcceptableExtentions =
        {
            ".prefab",
            ".obj",
            ".fbx",
            ".skp",
            ".unity"
        };

        //禁止タグ
        public static readonly string[] ProhibitedTags =
        {
            "MainCamera",
            "sphere",
            "FxTemporaire",
            "TeleportIgnore",
            "Fire",
            "projectile",
            "GameController",
            "EditorOnly",
            "Finish",
            "Respawn"
        };

        /// <summary>upload prefab</summary>
        public static readonly string STYLY_TEMP_DIR = "styly_temp";

        public static readonly string UploadPrefabName = "Assets/" + STYLY_TEMP_DIR + "/{0}.prefab";

        /// <summary>一時出力フォルダ</summary>
        public static readonly string OutputPath = "_Output/";

        /// <summary>Assetアップロード用URL取得API</summary>
        public static readonly string AzureSignedUrls = "https://api.styly.cc/api/v2/asset/signed_url";

        /// <summary>Assetアップロード完了通知API</summary>
        public static readonly string StylyAssetUploadCompleteUrl = "https://api.styly.cc/api/v2/asset/{0}/complete";

        // StudioのAPI Key取得用ページのURL
        public static readonly string GetAPIKeyUrl = "https://gallery.styly.cc/settings/api_key";

        // Get startedのURL prefabアップロードについての記事
        public static readonly string GetStartedUrl = "https://styly.cc/manual/unity-asset-uploader/";

        // Upload失敗時
        public static readonly string UploadErrorUrl = "https://styly.cc/manual/unity_plugin_error/";

        // モジュールに関してのerror
        public static readonly string ModulesErrorUrl = "https://styly.cc/manual/add-modules-unity/";

        // Pluginダウンロード
        public static readonly string PluginInformationUrl = "https://styly.cc/download/";

        // Sceneリスト
        public static readonly string ListOfScenesUrl = "https://gallery.styly.cc/studio/";

        // About STYLY
        public static readonly string AboutStylyUrl = "https://styly.cc";

        public static readonly int ThumbnailWidth = 640;
        public static readonly int ThumbnailHeight = 480;

        #region PluginUpdate関連

        public static readonly string VersionInformationJsonUrl = "https://build.styly.cc/unity-plugin/version.json";
        public static readonly string LatestVersionKey = "latestVersion";
        public static readonly string DownloadUrlKey = "downloadUrl";

        #endregion

        public static readonly int LIMIT_PACKAGE_FILE_SIZE_MB = 256;
        public static readonly int LIMIT_PACKAGE_FILE_SIZE_MB_AZCOPY = 512;

        // <summary>azcopyに与えるブロックサイズ値。アップロードの並列数に影響する。</summary>
        public static readonly int AZCOPY_BLOCK_SIZE_MB = 4;

        /// <summary>
        /// AZCOPYの並列度
        /// 数値または"AUTO"を指定する。
        /// </summary>
        public static readonly string AZCOPY_CONCURRENCY_VALUE = "10";

        /// <summary>
        /// AzCopyに与える追加パラメータ
        /// 例: "--cap-mbps=10" を与えると10mbpsに制限できる
        /// </summary>
        public static readonly string AZCOPY_OTHER_ARGS = "";

        // azcopy path
        public static readonly string AzcopyPathBase = "Assets/STYLY_Plugin/ThirdParty/azcopy/";
        public static readonly string AzcopyPathWindows = AzcopyPathBase + "azcopy.exe";
        public static readonly string AzcopyPkgPathMac = AzcopyPathBase + "azcopy.pkg";
        public static readonly string AzcopyShPathMac = AzcopyPathBase + "pkg_installer.sh";
        public static readonly string AzcopyPathMac = AzcopyPathBase + "InstallLocationForMacOSX~/azcopy-osx";

        public static string GetInternalAzcopyPath()
        {
#if UNITY_EDITOR_WIN
            return AzcopyPathWindows;
#elif UNITY_EDITOR_OSX
            return AzcopyPathMac;
#else
            return null;
#endif
        }

        /// <summary>
        /// AzCopyを利用するかどうかのフラグ。SettingsWindowで設定される。
        /// </summary>
        public static bool IsAzCopyEnabled
        {
            get => EditorPrefs.GetBool(UI.Settings.SETTING_KEY_STYLY_AZCOPY_ENABLED, GetAzCopyEnabledDefaultValue());
            set => EditorPrefs.SetBool(UI.Settings.SETTING_KEY_STYLY_AZCOPY_ENABLED, value);
        }

        /// <summary>
        /// AzCopyを利用するかどうかの初期値を返す
        /// WindowsではAzCopyインストール不要のため、デフォルトON(true)にする
        /// </summary>
        /// <returns></returns>
        private static bool GetAzCopyEnabledDefaultValue()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return true;
            }

            return false;
        }
    }
}
