using System.Text;

namespace STYLY.Analytics
{
    /// <summary>
    /// GoogleAnalyticsに記録するパラメーター
    /// パラメーターの定義はGoogle Analytics Measurement Protocolを参照。
    /// https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters?hl=ja
    /// </summary>
    public class GoogleAnalyticsRequestParams
    {
        /// <summary>トラッキングID</summary>
        public string TrackingId { get; set; }

        /// <summary>データソース</summary>
        public string DataSource { get; set; }

        /// <summary>クライアントID</summary>
        public string ClientId { get; set; }

        /// <summary>ユーザーID</summary>
        public string UserId { get; set; }

        /// <summary>セッションコントロール</summary>
        public string SessionControl { get; set; }

        /// <summary>ヒットタイプ</summary>
        public string HitType { get; set; }

        /// <summary>ドキュメントのホスト名</summary>
        public string DocumentHostName { get; set; }

        /// <summary>ドキュメントパス</summary>
        public string DocumentPath { get; set; }

        /// <summary>スクリーン名</summary>
        public string ScreenName { get; set; }

        /// <summary>アプリケーション名</summary>
        public string ApplicationName { get; set; }

        /// <summary>イベントカテゴリ</summary>
        public string EventCategory { get; set; }

        /// <summary>イベントアクション</summary>
        public string EventAction { get; set; }

        /// <summary>イベントラベル</summary>
        public string EventLabel { get; set; }

        /// <summary>カスタム速度のカテゴリ</summary>
        public string UserTimingCategory { get; set; }

        /// <summary>カスタム速度の変数名</summary>
        public string UserTimingVariableName { get; set; }

        /// <summary>カスタム速度の時間</summary>
        public string UserTimingTime { get; set; }

        /// <summary>カスタム速度のラベル</summary>
        public string UserTimingLabel { get; set; }

        /// <summary>カスタムディメンション1</summary>
        public string CustomDimension1 { get; set; }

        /// <summary>カスタムディメンション2</summary>
        public string CustomDimension2 { get; set; }

        /// <summary>
        /// クエリ文字列を構築する。
        /// </summary>
        /// <returns></returns>
        public string MakeQueryString()
        {
            StringBuilder query = new StringBuilder("v=1");

            if (TrackingId != null) query.Append("&tid=").Append(TrackingId);
            if (DataSource != null) query.Append("&ds=").Append(DataSource);
            if (ClientId != null) query.Append("&cid=").Append(ClientId);
            if (UserId != null) query.Append("&uid=").Append(UserId);
            if (SessionControl != null) query.Append("&sc=").Append(SessionControl);
            if (HitType != null) query.Append("&t=").Append(HitType);
            if (DocumentHostName != null) query.Append("&dh=").Append(DocumentHostName);
            if (DocumentPath != null) query.Append("&dp=").Append(DocumentPath);
            if (ScreenName != null) query.Append("&cd=").Append(ScreenName);
            if (ApplicationName != null) query.Append("&an=").Append(ApplicationName);
            if (EventCategory != null) query.Append("&ec=").Append(EventCategory);
            if (EventAction != null) query.Append("&ea=").Append(EventAction);
            if (EventLabel != null) query.Append("&el=").Append(EventLabel);
            if (UserTimingCategory != null) query.Append("&utc=").Append(UserTimingCategory);
            if (UserTimingVariableName != null) query.Append("&utv=").Append(UserTimingVariableName);
            if (UserTimingTime != null) query.Append("&utt=").Append(UserTimingTime);
            if (UserTimingLabel != null) query.Append("&utl=").Append(UserTimingLabel);
            if (CustomDimension1 != null) query.Append("&cd1=").Append(CustomDimension1);
            if (CustomDimension2 != null) query.Append("&cd2=").Append(CustomDimension2);

            return query.ToString();
        }
    }
}
