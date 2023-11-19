namespace STYLY.Analytics
{
    /// <summary>
    /// Analytics 処理用クラス
    /// LocalPreviewTool専用
    /// </summary>
    public class StylyAnalyticsForLocalPreviewTool
    {
        /// <summary>プロジェクト名。最終的にUnityPluginに組み込まれるのでUnityPlugin</summary>
        private const string projectName = "UnityPlugin";
        
        /// <summary>データソース名</summary>
        private const string dataSourceName = "unity";
        
        /// <summary>シングルトンインスタンス</summary>
        private static StylyAnalyticsForLocalPreviewTool instance;
        
        /// <summary>記録するかどうか</summary>
        private readonly bool enableAnalytics;

        /// <summary>
        /// コンストラクター
        /// </summary>
        private StylyAnalyticsForLocalPreviewTool()
        {
            // 初期化
            enableAnalytics = GoogleAnalyticsClient.Instance.Init();
        }
        
        /// <summary>
        /// シングルトンインスタンスの読み取り専用プロパティ
        /// </summary>
        public static StylyAnalyticsForLocalPreviewTool Instance => instance = instance ?? new StylyAnalyticsForLocalPreviewTool();

        /// <summary>
        /// GoogleAnalytics終了処理
        /// </summary>
        public void LocalPreview(string type)
        {
            if (!enableAnalytics) return;
            
            var req = new GoogleAnalyticsRequestParams
            {
                DataSource = dataSourceName,
                HitType = "event",
                EventCategory = "local_preview",
                EventAction = $"preview_{type}",
                CustomDimension1 = projectName
            };

            GoogleAnalyticsClient.Instance.Record(req);
        }
    }
}
