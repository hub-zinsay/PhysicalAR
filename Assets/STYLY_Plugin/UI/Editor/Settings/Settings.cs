using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace STYLY.Uploader.UI
{
    /// <summary>
    /// Settings画面のUI
    /// </summary>
    public class Settings : EditorWindow
    {
        /// <summary>EditorPrefsのKey: Email</summary>
        public const string SETTING_KEY_STYLY_EMAIL = "SUITE.STYLY.CC.ASSET_UPLOADER_EMAIL";
        
        /// <summary>EditorPrefsのKey: ApiKey</summary>
        public const string SETTING_KEY_STYLY_API_KEY = "SUITE.STYLY.CC.API_KEY";
        
        /// <summary>EditorPrefsのKey: AzcopyEnabled</summary>
        public const string SETTING_KEY_STYLY_AZCOPY_ENABLED = "SUITE.STYLY.CC.AZCOPY_ENABLED";
        
        /// <summary>ツリー</summary>
        private VisualElement tree;
        
        /// <summary>
        /// Emailの値
        /// </summary>
        private string Email
        {
            set
            {
                // UI更新
                rootVisualElement.Q<TextField>("email").value = value;

                // EditorPrefs更新
                EditorPrefs.SetString(SETTING_KEY_STYLY_EMAIL, value);
            }
        }

        /// <summary>
        /// ApiKeyの値
        /// </summary>
        private string ApiKey
        {
            set
            {
                // UI更新
                rootVisualElement.Q<TextField>("api_key").value = value;

                // EditorPrefs更新
                EditorPrefs.SetString(SETTING_KEY_STYLY_API_KEY, value);
            }
        }

        /// <summary>
        /// Azcopyの値
        /// </summary>
        private bool AzcopyEnabled
        {
            get => Config.IsAzCopyEnabled;
            set
            {
                // UI更新
                rootVisualElement.Q<Toggle>("azcopy_enabled").value = value;

                // EditorPrefs更新
                Config.IsAzCopyEnabled = value;
            }
        }

        /// <summary>
        /// ウィンドウが起動した時
        /// </summary>
        public void OnEnable()
        {
            // ウィンドウタイトル
            titleContent = new GUIContent("Settings");
            
            // 最小ウィンドウサイズ
            minSize = new Vector2(400, 280);

            // ツリーを構築する
            InitTree();
            
            // 初期化
            Email = EditorPrefs.GetString(SETTING_KEY_STYLY_EMAIL);
            ApiKey = EditorPrefs.GetString(SETTING_KEY_STYLY_API_KEY);
            AzcopyEnabled = Config.IsAzCopyEnabled;
            
            // テキストボックスのチェンジイベント登録
            tree.Q<TextField>("email").RegisterValueChangedCallback(OnChangeEmail);
            tree.Q<TextField>("api_key").RegisterValueChangedCallback(OnChangeApiKey);
            tree.Q<Toggle>("azcopy_enabled").RegisterValueChangedCallback(OnChangeAzcopyEnabled);
            
            // Buttonのクリックイベント登録
            tree.Q<Button>("get_styly_api_key").clicked += OnClickGetStylyApiKey;
            tree.Q<Button>("get_started").clicked += OnClickGetStarted;
            tree.Q<Button>("about_styly").clicked += OnClickAboutStyly;

            // UXMLの親要素の背景色をfooterと同じ色にする
            // UXMLの親のスタイルはUSSでは指定できないため、こちらで指定
            if (ColorUtility.TryParseHtmlString("#0A151C", out var col))
            {
                tree.parent.style.backgroundColor = col;
            }

            // Azcopyのインストール状況を検査する
            ValidateAzcopy();
        }

        /// <summary>
        /// VisualTreeを構築する
        /// </summary>
        private void InitTree()
        {
            // 本コードを格納しているフォルダパスを取得する
            var folderPath = Utility.GetFolderRelativePathFromAsset();
            
            // USSファイルを読み込む
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(folderPath, "Settings.uss"));

            // UXMLファイルを読み込む
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Path.Combine(folderPath, "Settings.uxml"));

            // UXMLからVisualElementのツリーを構築する
            tree = visualTree.CloneTree();
            
            // スタイルシートを適用する
            tree.styleSheets.Add(styleSheet);
            
            // 親要素に登録する
            rootVisualElement.Add(tree);
        }

        /// <summary>
        /// Validate AzCopy exec file presence
        /// NOTE this will run even if the "enable" toggle wasn't pressed
        /// </summary>
        private void ValidateAzcopy()
        {
            if (AzcopyEnabled && !AzcopyExecutableFileInstaller.Validate())
            {
                // Azcopyが見つからない
                AzcopyNotFound();
            }
        }

        /// <summary>
        /// Azcopyが見つからない
        /// </summary>
        private void AzcopyNotFound()
        {
            AzcopyEnabled = false;

            EditorUtility.DisplayDialog(
                "AzCopy exec file not found",
                "Please click the 'Enable AzCopy' checkbox to install AzCopy again.",
                "OK");
        }

        /// <summary>
        /// AzCopyをインストールする
        /// </summary>
        /// <returns>成否</returns>
        private bool InstallAzcopy()
        {
            EditorUtility.DisplayProgressBar("Installing AzCopy", "", 0);

            // インストール
            var successInstall = AzcopyExecutableFileInstaller.Install(out var error);

            if (successInstall)
            {
                EditorUtility.DisplayDialog(
                    "AzCopy installation success",
                    "AzCopy was installed properly!",
                    "OK"
                );
            }
            else
            {
                Debug.LogError($"AzCopy was not installed: {error}");
                EditorUtility.DisplayDialog(
                    "Failed installing AzCopy",
                    $"AzCopy was not installed.\n{error}",
                    "OK"
                );
            }

            EditorUtility.ClearProgressBar();

            return successInstall;
        }

        /// <summary>
        /// Emailのテキストボックス変更イベント
        /// </summary>
        /// <param name="value">変更内容</param>
        private void OnChangeEmail(ChangeEvent<string> value)
        {
            Email = value.newValue.Trim();
        }

        /// <summary>
        /// ApiKeyのテキストボックス変更イベント
        /// </summary>
        /// <param name="value">変更内容</param>
        private void OnChangeApiKey(ChangeEvent<string> value)
        {
            ApiKey = value.newValue.Trim();
        }

        /// <summary>
        /// AzCopyのチェックボックス変更イベント
        /// </summary>
        /// <param name="value">変更内容</param>
        private void OnChangeAzcopyEnabled(ChangeEvent<bool> value)
        {
            AzcopyEnabled = (!value.previousValue && value.newValue)
                ? InstallAzcopy()
                : value.newValue;
        }

        /// <summary>
        /// Get STYLY API Keyクリック時
        /// </summary>
        private void OnClickGetStylyApiKey()
        {
            Application.OpenURL(Config.GetAPIKeyUrl);
        }

        /// <summary>
        /// Get Startedクリック時
        /// </summary>
        private void OnClickGetStarted()
        {
            Application.OpenURL(Config.GetStartedUrl);
        }

        /// <summary>
        /// About STYLYクリック時
        /// </summary>
        private void OnClickAboutStyly()
        {
            Application.OpenURL(Config.AboutStylyUrl);
        }
    }
}
