namespace STYLY.MaintenanceTool.Utility
{
    /// <summary>
    /// Local Preview Tool 独自エラー定義
    /// </summary>
    public class LocalPreviewToolError
    {
        /// <summary>
        /// エラー種別の列挙型
        /// </summary>
        public enum ErrorType
        {
            DeviceNotFound,
            TargetSceneNotFound,
            StartStylyFailed,
            BuildStylySceneAssetFailed,
        }

        /// <summary>エラー種別</summary>
        public ErrorType Type { get; set; }

        /// <summary>プラットフォーム名（オプション）</summary>
        public string PlatformName { get; set; } = "";
        
        /// <summary>
        /// エラーの種類ごとのメッセージを取得する
        /// </summary>
        /// <returns>エラーメッセージ</returns>
        public string GetErrorMessage()
        {
            switch (Type)
            {
                case ErrorType.DeviceNotFound:
                    return "The headset is not found.\r\nPlease connect the headset to your PC.";
                case ErrorType.TargetSceneNotFound:
                    return "The target scene is not found.\r\nPlease open a scene.";
                case ErrorType.StartStylyFailed:
                    return "STYLY failed to start.\r\nPlease make sure the headset is correctly connected to your PC.";
                case ErrorType.BuildStylySceneAssetFailed:
                    return "Build asset bundle failed for platform " + PlatformName + 
                           ".\r\nPlease fix compile error.";
                default:
                    return "";
            }
        }
    }
}
