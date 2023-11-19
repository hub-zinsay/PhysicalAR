using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace STYLY.EnvConfig
{
    /// <summary>
    /// 設定ファイルのパーサー
    /// </summary>
    public class EnvConfigParser
    {
        /// <summary>設定ファイルの名前</summary>
        private const string fileName = "env-config.json";

        /// <summary>設定ファイルのパス</summary>
        public const string FilePath = "STYLY_Plugin/STYLY_MaintenanceTool/Scripts/Editor/EnvConfig/" + fileName;

        /// <summary>シングルトンインスタンス</summary>
        private static EnvConfigParser instance;

        /// <summary>
        /// シングルトンインスタンスの読み取り専用プロパティ
        /// </summary>
        public static EnvConfigParser Instance => instance = instance ?? new EnvConfigParser();

        /// <summary>設定情報</summary>
        private EnvConfigModel envConfig;

        /// <summary>
        /// 設定情報のプロパティ
        /// </summary>
        public EnvConfigModel EnvConfig
        {
            get
            {
                if (envConfig != null)
                {
                    return envConfig;
                }

                // 設定ファイルのフルパスを取得する
                var filePath = Path.Combine(Application.dataPath, FilePath);
                if (!File.Exists(filePath))
                {
                    // このソースコードと同階層の設定ファイルのパスを取得する
                    filePath = GetEnvFilePath();
                    if (!File.Exists(filePath))
                    {
                        return null;
                    }
                }

                // ファイルからJSON文字列を取得する
                var jsonString = File.ReadAllText(filePath);

                // JSON文字列をEnvConfigModelに変換する
                return envConfig = JsonUtility.FromJson<EnvConfigModel>(jsonString);
            }
        }

        /// <summary>
        /// GoogleAnalyticsのプロファイル（トラッキングID）の配列
        /// </summary>
        public List<string> GoogleAnalyticsProfiles => EnvConfig?.GoogleAnalyticsProfiles ?? new List<string>();

        /// <summary>
        /// このクラスと同階層の設定ファイルのパスを取得する
        /// </summary>
        /// <param name="sourceFilePath">このソースコードファイルのパス。CallerFilePathがメソッドの呼び出し元のパスを代入する。</param>
        /// <returns>設定ファイルのパス</returns>
        private string GetEnvFilePath([CallerFilePath] string sourceFilePath = "")
        {
            // このソースコードのフォルダパス
            var folderPath = Path.GetDirectoryName(sourceFilePath);

            // 設定ファイルのパス
            return Path.Combine(folderPath, fileName);
        }
    }
}
