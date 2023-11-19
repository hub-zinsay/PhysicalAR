using System.Collections.Generic;

namespace STYLY.EnvConfig
{
    /// <summary>
    /// 設定ファイルのJSONモデル
    /// </summary>
    public class EnvConfigModel
    {
        /// <summary>GoogleAnalyticsのプロファイル（トラッキングID）の配列</summary>
        public List<string> GoogleAnalyticsProfiles;
    }
}
