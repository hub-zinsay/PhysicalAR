using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace STYLY.Uploader.UI
{
    /// <summary>
    /// CacheServerの設定状況チェック結果を表示するVisualElement
    /// 問題がある場合はエラーを表示する
    /// </summary>
    public class CacheServerErrorElement : VisualElement
    {
        /// <summary>ツリー</summary>
        private VisualElement tree;
            
        /// <summary>
        /// 独自のUXML要素として他のUXMLから参照できるようにする
        /// </summary>
        public new class UxmlFactory : UxmlFactory<CacheServerErrorElement>
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CacheServerErrorElement()
        {
            // ツリーを構築する
            InitTree();

            // CacheServer検査
            ValidateCacheServer();
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
                AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(folderPath, "CacheServerErrorElement.uss"));
            
            // UXMLファイルを読み込む
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Path.Combine(folderPath, "CacheServerErrorElement.uxml"));

            // UXMLからVisualElementのツリーを構築する
            tree = visualTree.CloneTree();
            
            // スタイルシートを適用する
            tree.styleSheets.Add(styleSheet);
            
            // 親要素に登録する
            hierarchy.Add(tree);
        }

        /// <summary>
        /// キャッシュサーバーの設定を検査する
        /// </summary>
        /// <returns>成否</returns>
        private void ValidateCacheServer()
        {
            var isEnabled = CacheServerChecker.IsCacheServerEnabled();

            // 表示切り替え
            tree.Q<VisualElement>("error_area").style.display =
                isEnabled ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}
