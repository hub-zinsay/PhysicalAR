using UnityEditor;

namespace STYLY.Uploader
{
    /// <summary>
    /// キャッシュサーバーを確認するクラス
    /// </summary>
    public class CacheServerChecker
    {
        private enum CacheServerMode { Local, Remote, Disabled }
        private enum CacheServer2Mode { Enabled, Disabled }

        private const string CacheServerModeKey = "CacheServerMode";
        private const string CacheServerEnabledKey = "CacheServerEnabled";
        private const string CacheServer2ModeKey = "CacheServer2Mode";

        /// <summary>
        /// returns true if the cache server is enabled.
        /// </summary>
        private static bool IsCacheServerV1Enabled()
        {
            var defaultValue = EditorPrefs.GetBool(CacheServerEnabledKey) ? CacheServerMode.Remote : CacheServerMode.Disabled;
            var cacheServerMode = (CacheServerMode)EditorPrefs.GetInt(CacheServerModeKey, (int)defaultValue);
            // cache server v1 is enabled when CacheServerMode.Local or CacheServerMode.Remote.
            return cacheServerMode == CacheServerMode.Local || cacheServerMode == CacheServerMode.Remote;
        }

        /// <summary>
        /// returns true if the cache server for AssetPipelineV2 is enabled.
        /// </summary>
        private static bool IsCacheServerV2Enabled()
        {
#if UNITY_2019_1_OR_NEWER
            return (CacheServer2Mode)EditorPrefs.GetInt(CacheServer2ModeKey, (int)CacheServer2Mode.Disabled) == CacheServer2Mode.Enabled;
#else
            return false;
#endif
        }

        /// <summary>
        /// Check if cache server is enabled or not, including cache server for AssetPipeline-v2.
        /// </summary>
        public static bool IsCacheServerEnabled()
        {
            if (IsCacheServerV1Enabled())
            {
                return true;
            }

            if (IsCacheServerV2Enabled())
            {
                return true;
            }

            return false;
        }
    }
}
