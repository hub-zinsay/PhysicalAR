using System.Linq;

namespace STYLY.Uploader
{
    /// <summary>
    /// Unityバージョンが有効なものか検査するクラス
    /// </summary>
    public class UnityVersionChecker
    {
        /// <summary>
        /// インスタンス化防止のためのprotectedコンストラクター
        /// </summary>
        protected UnityVersionChecker()
        {
        }

        /// <summary>
        /// Unityバージョンを検査する
        /// </summary>
        /// <param name="version">
        /// Unityバージョン文字列
        /// 例. 2019.4.29f1
        /// </param>
        /// <returns>OK/NG</returns>
        public static bool Check(string version)
        {
            return Config.UNITY_VERSIONS.Any(version.Contains);
        }
    }
}
