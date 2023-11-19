using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace STYLY.Uploader.UI
{
    /// <summary>
    /// PluginのUpdate通知を行うウィンドウ。
    /// PluginUpdateNotifierSettingsとVersionInformationの内容に従って機能する。
    /// </summary>
    internal class NotifyPluginUpdateWindow : EditorWindow
    {
        /// <summary>更新通知設定</summary>
        private PluginUpdateNotifierSettings settings => PluginUpdateNotifierSettings.LoadOrCreateSettings();

        /// <summary>バージョン情報</summary>
        private VersionInformation versionInformation;

        /// <summary>ツリー</summary>
        private VisualElement tree;
        
        /// <summary>
        /// EditorWindowを開く
        /// </summary>
        public static void Open()
        {
            var wnd = GetWindow<NotifyPluginUpdateWindow>();
            wnd.Show();
        }

        /// <summary>
        /// ウィンドウが起動した時
        /// </summary>
        public void OnEnable()
        {
            // ウィンドウタイトル
            titleContent = new GUIContent("Update Checker");
            
            // 最小ウィンドウサイズ
            minSize = new Vector2(380, 190);
            
            // ツリーを構築する
            InitTree();
            
            // 動的要素を描画する
            versionInformation = VersionInformationProvider.Instance.VersionInformation;
            RenderCurrentVersion(versionInformation.CurrentVersion);
            RenderLatestVersion(versionInformation.LatestVersion);
            SetShowAtStartup(settings.ShowAtStartup);

            // 更新時のコールバック登録
            VersionInformationProvider.Instance.OnUpdateVersionInformation += OnUpdateVersionInformation;
            
            // チェックボックスのチェンジイベント登録
            tree.Q<Toggle>("show_at_startup").RegisterValueChangedCallback(OnChangeShowAtStartup);

            // Buttonのクリックイベント登録
            tree.Q<Button>("more_information_latest").clicked += OnClickMoreInformation; 
            tree.Q<Button>("more_information_notlatest").clicked += OnClickMoreInformation; 
            tree.Q<Button>("download_new_version").clicked += OnClickDownloadNewVersion;
            tree.Q<Button>("skip_this_version").clicked += OnClickSkipThisVersion;
            tree.Q<Button>("about_styly").clicked += OnClickAboutStyly;

            // UXMLの親要素の背景色をfooterと同じ色にする
            // UXMLの親のスタイルはUSSでは指定できないため、こちらで指定
            if (ColorUtility.TryParseHtmlString("#0A151C", out var col))
            {
                tree.parent.style.backgroundColor = col;
            }

            // 一旦、すべて非表示にする
            tree.Q<VisualElement>("notlatest").style.display = DisplayStyle.None;
            tree.Q<VisualElement>("latest").style.display = DisplayStyle.None;
            tree.Q<VisualElement>("error").style.display = DisplayStyle.None;

            // 状況によって描画内容を変更する
            switch (versionInformation.GetStatus())
            {
                case VersionInformation.Status.UpdateAvailable:
                    tree.Q<VisualElement>("notlatest").style.display = DisplayStyle.Flex;
                    break;
                case VersionInformation.Status.UpToDate:
                    tree.Q<VisualElement>("latest").style.display = DisplayStyle.Flex;
                    break;
                case VersionInformation.Status.Unavailable:
                    tree.Q<VisualElement>("error").style.display = DisplayStyle.Flex;
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected value versionInformation.GetStatus() = {versionInformation.GetStatus()}");
            }
        }

        /// <summary>
        /// 破棄時
        /// </summary>
        private void OnDestroy()
        {
            VersionInformationProvider.Instance.OnUpdateVersionInformation -= OnUpdateVersionInformation;
            tree.Q<Button>("more_information_latest").clicked -= OnClickMoreInformation; 
            tree.Q<Button>("more_information_notlatest").clicked -= OnClickMoreInformation; 
            tree.Q<Button>("download_new_version").clicked -= OnClickDownloadNewVersion;
            tree.Q<Button>("skip_this_version").clicked -= OnClickSkipThisVersion;
            tree.Q<Button>("about_styly").clicked -= OnClickAboutStyly;
        }

        /// <summary>
        /// VisualTreeを構築する
        /// </summary>
        private void InitTree()
        {
            // 本コードを格納しているフォルダパスを取得する
            var folderPath = Utility.GetFolderRelativePathFromAsset();

            // USSファイルを読み込む
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(folderPath, "NotifyPluginUpdateWindow.uss"));

            // UXMLファイルを読み込む
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Path.Combine(folderPath, "NotifyPluginUpdateWindow.uxml"));

            // UXMLからVisualElementのツリーを構築する
            tree = visualTree.CloneTree();

            // スタイルシートを適用する
            tree.styleSheets.Add(styleSheet);

            // 親要素に登録する
            rootVisualElement.Add(tree);
        }

        /// <summary>
        /// 現バージョンを描画する
        /// </summary>
        private void RenderCurrentVersion(string value)
        {
            // 複数箇所あるので、Queryで一括更新
            tree.Query<TextElement>("current_version").ForEach(element =>
            {
                element.text = $"Current Version : {value}";
            });
        }

        /// <summary>
        /// 最新バージョンを描画する
        /// </summary>
        private void RenderLatestVersion(string value)
        {
            // UI更新
            tree.Q<TextElement>("latest_version").text = $"Latest Version : {value}";
        }

        /// <summary>
        /// Show At Startupをセットする
        /// </summary>
        private void SetShowAtStartup(bool value)
        {
            // UI更新
            tree.Q<Toggle>("show_at_startup").value = value;

            // EditorPrefs更新
            settings.ShowAtStartup = value;
        }

        /// <summary>
        /// 更新時のコールバック
        /// </summary>
        /// <param name="newVersionInformation">新しいバージョン情報</param>
        private void OnUpdateVersionInformation(VersionInformation newVersionInformation)
        {
            // フィールドを更新
            versionInformation = newVersionInformation;
            
            // 再描画
            Repaint();
        }

        /// <summary>
        /// Show At Startupのチェックボックス変更イベント
        /// </summary>
        /// <param name="value">変更内容</param>
        private void OnChangeShowAtStartup(ChangeEvent<bool> value)
        {
            SetShowAtStartup(value.newValue);
        }
        
        /// <summary>
        /// Modules Informationクリック時
        /// </summary>
        private void OnClickMoreInformation()
        {
            Application.OpenURL(Config.PluginInformationUrl);
        }
        
        /// <summary>
        /// Download New Versionクリック時
        /// </summary>
        private void OnClickDownloadNewVersion()
        {
            Application.OpenURL(versionInformation.DownloadUrl);
        }
        
        /// <summary>
        /// Skip This Versionクリック時
        /// </summary>
        private void OnClickSkipThisVersion()
        {
            settings.SkipVersion = versionInformation.LatestVersion;
            Close();
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
