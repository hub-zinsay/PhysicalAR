using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace STYLY.Uploader
{
    /// <summary>
    /// Config.VersionInformationJsonUrlからネットワークを通じてJsonを取得し、VersionInformationへ変換して外部への提供を担当するクラス。
    /// </summary>
    internal class VersionInformationProvider
    {
        public static VersionInformationProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VersionInformationProvider();
                }

                return instance;
            }
        }

        public VersionInformation VersionInformation
        {
            get { return versionInformation; }
            private set
            {
                versionInformation = value;
                // 更新されたVersionInformationを通知する
                if (OnUpdateVersionInformation != null) OnUpdateVersionInformation(versionInformation);
            }
        }
        // VersionInformationが更新された際に通知するためのイベント
        public event Action<VersionInformation> OnUpdateVersionInformation;

        private static VersionInformationProvider instance;
        private VersionInformation versionInformation;

        private VersionInformationProvider() 
        {
            // CheckForLatestVersionが呼び出される（データを取りに行く）まではとりあえずStatus.UpToDateをかえすVersionInformationを保持。
            VersionInformation = new VersionInformation(Config.CurrentVersion, Config.CurrentVersion, "");
        }

        public void CheckForLatestVersion(Action onComplete = null, Action<Exception> onError = null)
        {
            // Config.VersionInformationJsonUrlに記載されたUrlからJsonを取得し
            // 内容をVersionInformationに変換して利用する
            var request = UnityWebRequest.Get(Config.VersionInformationJsonUrl);
            request.EditorSendWebRequest(() =>
            {
                var json = request.downloadHandler.text;

                Dictionary<string, object> data = Json.Deserialize(json) as Dictionary<string, object>;

                // 有効なdataではない場合はエラー処理して終了。
                if (!IsValidData(data))
                {
                    OnErrorGetVersionInformation(new Exception("Invalid json object passed in."), onError);
                    return;
                }

                var latestVersion = data[Config.LatestVersionKey] as string;
                var downloadUrl = data[Config.DownloadUrlKey] as string;

                var info = new VersionInformation(Config.CurrentVersion, latestVersion, downloadUrl);
                // Unavailableなら正しくないパラメータが与えられているので、エラー処理して終了。
                if (info.GetStatus() == VersionInformation.Status.Unavailable)
                {
                    OnErrorGetVersionInformation(new Exception("Invalid parameters passed in."), onError);
                    return;
                }

                // 入手したバージョン情報を格納。
                // 格納時にOnUpdateVersionInformationで通知が行われる。
                VersionInformation = info;

                // 全部終わったらonCompleteを発火。
                if (onComplete != null) onComplete();

            }, (ex) => OnErrorGetVersionInformation(ex, onError));
        }

        private void OnErrorGetVersionInformation(Exception error, Action<Exception> onError)
        {
            // データ取得エラーを示すためLatestVersionが空のVersionInformationを格納。
            VersionInformation = new VersionInformation(Config.CurrentVersion, "", "");

            if (onError != null) onError(error);
        }

        /// <summary>
        /// Json.Deserializeでデシリアライズしたオブジェクトが有効なものかチェックするメソッド。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool IsValidData(Dictionary<string, object> data)
        {
            if (data == null)
            {
                return false;
            }

            // 必要なキーが含まれない場合はfalse
            if (!data.ContainsKey(Config.LatestVersionKey) || !data.ContainsKey(Config.DownloadUrlKey))
            {
                return false;
            }

            return true;
        }
    }
}