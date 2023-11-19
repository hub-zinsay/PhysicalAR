using STYLY.EnvConfig;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace STYLY.Analytics
{
    /// <summary>
    /// GoogleAnalyticsのWebAPIのクライアント
    /// </summary>
    public class GoogleAnalyticsClient
    {
        /// <summary>Google Analytics Api host</summary>
        private const string host = "https://www.google-analytics.com/collect?";

        /// <summary>Client IDのPlayerPrefsのKey</summary>
        private const string clientIdPlayerPrefsKey = "styly.analytics.clientId";

        /// <summary>シングルトンインスタンス</summary>
        private static GoogleAnalyticsClient instance;

        /// <summary>Tracking IDのリスト</summary>
        private List<string> listTrackingId = new List<string>();

        /// <summary>Client ID</summary>
        private string clientId;

        /// <summary>
        /// Client IDのプロパティ
        /// </summary>
        private string ClientId
        {
            get
            {
                // なければ新規作成
                if (clientId == null)
                {
                    clientId = PlayerPrefs.GetString(clientIdPlayerPrefsKey);

                    if (string.IsNullOrEmpty(clientId))
                    {
                        clientId = System.Guid.NewGuid().ToString();

                        PlayerPrefs.SetString(clientIdPlayerPrefsKey, clientId);
                        PlayerPrefs.Save();
                    }
                }

                return clientId;
            }
        }

        /// <summary>
        /// URLを構築する。
        /// </summary>
        /// <param name="req">GoogleAnalyticsに記録するパラメーター</param>
        /// <returns>URL</returns>
        private string FormatUrl(GoogleAnalyticsRequestParams req)
        {
            return $"{host}{req.MakeQueryString()}";
        }

        /// <summary>
        /// サーバ通信
        /// </summary>
        /// <param name="url">URL</param>
        private void SendRequest(string url)
        {
            using (var webRequest = UnityWebRequest.Post(url, ""))
            {
                // デフォルトのUserAgentだとGoogleAnalyticsに拒絶されるため変更
                webRequest.SetRequestHeader("User-Agent", "Unity " + Application.unityVersion);
                
                // サーバー通信
                var asyncOperation = webRequest.SendWebRequest();

                while (!asyncOperation.isDone)
                {
                    Thread.Sleep(10);
                    
                    // 通信失敗検査
                    if (webRequest.isHttpError || webRequest.isNetworkError)
                    {
                        // 通信失敗    
                        Debug.LogError(webRequest.error);
                        return;
                    }
                }

                // 通信成功
                Debug.Log("Google Analytics API Response: " + webRequest.downloadHandler.text);
            }
        }

        /// <summary>
        /// シングルトンインスタンスの読み取り専用プロパティ
        /// </summary>
        public static GoogleAnalyticsClient Instance => instance = instance ?? new GoogleAnalyticsClient();

        /// <summary>
        /// ConfigからTrackingIDを取得する。
        /// </summary>
        /// <returns>成否</returns>
        public bool Init()
        {
            // ConfigからIDを取得する
            listTrackingId = EnvConfigParser.Instance.GoogleAnalyticsProfiles;

            // IDが1つ以上あればAnalyticsは有効
            return listTrackingId.Count > 0;
        }

        /// <summary>
        /// GoogleAnalyticsに記録する。
        /// </summary>
        /// <param name="req">GoogleAnalyticsに記録するデータ</param>
        public void Record(GoogleAnalyticsRequestParams req)
        {
            req.ClientId = ClientId;

            listTrackingId.ForEach(trackingId =>
            {
                req.TrackingId = trackingId;

                var url = FormatUrl(req);
                SendRequest(url);
            });
        }
    }
}
