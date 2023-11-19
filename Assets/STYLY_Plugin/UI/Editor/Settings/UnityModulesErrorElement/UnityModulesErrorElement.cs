using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace STYLY.Uploader.UI
{
    /// <summary>
    /// UnityModuleのインストール状況チェック結果を表示するVisualElement
    /// 問題がある場合はエラーを表示する
    /// </summary>
    public class UnityModulesErrorElement : VisualElement
    {
        /// <summary>ツリー</summary>
        private VisualElement tree;
            
        /// <summary>
        /// 独自のUXML要素として他のUXMLから参照できるようにする
        /// </summary>
        public new class UxmlFactory : UxmlFactory<UnityModulesErrorElement>
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UnityModulesErrorElement()
        {
            // ツリーを構築する
            InitTree();
            
            // UnityModules検査
            ValidateUnityModules();
            
            // Buttonのクリックイベント登録
            tree.Q<Button>("more_information").clicked += OnClickMoreInformation;
        }

        /// <summary>
        /// VisualTreeを構築する
        /// </summary>
        private void InitTree()
        {
            // 本コードを格納しているフォルダパスを取得する
            var folderPath = Utility.GetFolderRelativePathFromAsset();
            
            // USSファイルを読み込む
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(folderPath, "UnityModulesErrorElement.uss"));
            
            // UXMLファイルを読み込む
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Path.Combine(folderPath, "UnityModulesErrorElement.uxml"));

            // UXMLからVisualElementのツリーを構築する
            tree = visualTree.CloneTree();
            
            // スタイルシートを適用する
            tree.styleSheets.Add(styleSheet);
            
            // 親要素に登録する
            hierarchy.Add(tree);
        }

        /// <summary>
        /// UnityModulesのインストール状況を検査する
        /// </summary>
        /// <returns>成否</returns>
        private void ValidateUnityModules()
        {
            // Unityモジュールのインストール状況を確認するクラス
            var modulesChecker = new UnityModulesChecker();

            // Unityモジュールのインストール状況を確認し、エラーメッセージを表示する
            var countPlatformNotInstalled = Config.PlatformList
                // プラットフォーム名をBuildTargetの辞書に変換する
                .Select(platform => Config.PlatformBuildTargetDic[platform])
                // Unityモジュールのインストール状況を確認する
                .Where(target => !modulesChecker.IsPlatformSupportLoaded(target))
                // インストールされてなければエラーメッセージを追加
                .Select(target =>
                {
                    var errorModule = new TextElement
                    {
                        text = $"{target.ToString()} module not installed."
                    };
                    tree.Q<VisualElement>("single_module_error").Add(errorModule);
                    return target;
                })
                // インストールされていないUnityモジュールを数える
                .Count();

            // 表示切り替え
            tree.Q<VisualElement>("error_area").style.display =
                countPlatformNotInstalled > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        /// <summary>
        /// Modules Informationクリック時
        /// </summary>
        private void OnClickMoreInformation()
        {
            Application.OpenURL(Config.ModulesErrorUrl);
        }
    }
}
