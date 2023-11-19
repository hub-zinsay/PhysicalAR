using UnityEngine;

namespace STYLY.Analytics
{
    /// <summary>
    /// Analytics 処理用クラス
    /// UnityPlugin専用
    /// </summary>
    public class StylyAnalyticsForUnityPlugin
    {
        /// <summary>プロジェクト名</summary>
        private const string projectName = "UnityPlugin";
        
        /// <summary>データソース名</summary>
        private const string dataSourceName = "unity";
        
        /// <summary>シングルトンインスタンス</summary>
        private static StylyAnalyticsForUnityPlugin instance;
        
        /// <summary>記録するかどうか</summary>
        private readonly bool enableAnalytics;

        /// <summary>ビルド中のアセットのID</summary>
        private string assetId;

        /// <summary>シーンアセットかどうか</summary>
        private bool isScene;

        /// <summary>ビルド開始時刻</summary>
        private float startBuildTime;

        /// <summary>ビルド終了時刻</summary>
        private float finishBuildTime;

        /// <summary>アップロード終了時刻</summary>
        private float finishUploadTime;

        /// <summary>
        /// コンストラクター
        /// </summary>
        private StylyAnalyticsForUnityPlugin()
        {
            // 初期化
            enableAnalytics = GoogleAnalyticsClient.Instance.Init();
        }
        
        /// <summary>
        /// シングルトンインスタンスの読み取り専用プロパティ
        /// </summary>
        public static StylyAnalyticsForUnityPlugin Instance => instance = instance ?? new StylyAnalyticsForUnityPlugin();

        /// <summary>
        /// アセットのビルド開始を記録する。
        /// GoogleAnalyticsとの通信はRecordStartBuildAssetでまとめて行うので、このメソッドでは行わない。
        /// </summary>
        /// <param name="assetId">アセットID</param>
        /// <param name="isScene">シーンアセットかどうか</param>
        public void StartBuildAsset(string assetId, bool isScene)
        {
            if (!enableAnalytics) return;
            
            // アセットビルド開始時刻を記録
            startBuildTime = Time.realtimeSinceStartup;

            // アセットIDを記録
            this.assetId = assetId;

            // シーナセットかどうかを記録
            this.isScene = isScene;
        }

        /// <summary>
        /// アセットのビルド終了を記録する。
        /// GoogleAnalyticsとの通信はRecordStartBuildAssetでまとめて行うので、このメソッドでは行わない。
        /// </summary>
        public void FinishBuildAsset()
        {
            if (!enableAnalytics) return;
            
            // アセットビルド終了時刻を記録
            finishBuildTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// アセットのアップロード終了を記録する。
        /// GoogleAnalyticsとの通信はRecordStartBuildAssetでまとめて行うので、このメソッドでは行わない。
        /// </summary>
        public void FinishUploadAsset()
        {
            if (!enableAnalytics) return;
            
            // アセットアップロード終了時刻を記録
            finishUploadTime = Time.realtimeSinceStartup;
        }
        
        /// <summary>
        /// アセットの情報をGoogleAnalyticsに送信する。
        /// </summary>
        public void RecordAsset()
        {
            // GoogleAnalyticsに順番に送信
            RecordStartBuildAsset();
            RecordFinishBuildAsset();
            RecordFinishUploadAsset();
        }

        /// <summary>
        /// アセットのビルド開始をGoogleAnalyticsに記録する
        /// </summary>
        private void RecordStartBuildAsset()
        {
            // イベントアクションを定義する。
            var eventAction = isScene ? "start_build_scene_asset" : "start_build_prefab_asset";  
            
            var req = new GoogleAnalyticsRequestParams
            {
                DataSource = dataSourceName,
                HitType = "event",
                EventCategory = "asset",
                EventAction = eventAction,
                EventLabel = assetId,
                CustomDimension1 = projectName
            };

            GoogleAnalyticsClient.Instance.Record(req);
        }

        /// <summary>
        /// アセットのビルド終了をGoogleAnalyticsに記録する
        /// </summary>
        private void RecordFinishBuildAsset()
        {
            // アセットビルド時間を計算
            var buildTime = (int)((finishBuildTime - startBuildTime) * 1000f);
            
            // イベントアクションを定義する。
            var eventAction = isScene ? "finish_build_scene_asset" : "finish_build_prefab_asset";  
            
            var req = new GoogleAnalyticsRequestParams
            {
                DataSource = dataSourceName,
                HitType = "timing",
                UserTimingCategory = "scene",
                UserTimingVariableName = eventAction,
                UserTimingTime = buildTime.ToString(),
                UserTimingLabel = assetId,
                CustomDimension1 = projectName
            };

            GoogleAnalyticsClient.Instance.Record(req);
        }

        /// <summary>
        /// アセットのアップロード終了をGoogleAnalyticsに記録する
        /// </summary>
        private void RecordFinishUploadAsset()
        { 
            // アセットアップロード時間を計算
            var uploadTime = (int)((finishUploadTime - finishBuildTime) * 1000f);
            
            // イベントアクションを定義する。
            var eventAction = isScene ? "finish_upload_scene_asset" : "finish_upload_prefab_asset";  
            
            var req = new GoogleAnalyticsRequestParams
            {
                DataSource = dataSourceName,
                HitType = "timing",
                UserTimingCategory = "scene",
                UserTimingVariableName = eventAction,
                UserTimingTime = uploadTime.ToString(),
                UserTimingLabel = assetId,
                CustomDimension1 = projectName
            };

            GoogleAnalyticsClient.Instance.Record(req);
        }
    }
}
