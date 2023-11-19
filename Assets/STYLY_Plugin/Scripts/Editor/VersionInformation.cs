using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STYLY.Uploader
{
    /// <summary>
    /// Config.VersionInformationJsonUrlから取得したJsonの変換対象となるクラス。
    /// </summary>
    internal class VersionInformation
    {
        internal enum Status
        {
            UpToDate,
            UpdateAvailable,
            Unavailable,
        }

        private string currentVersion;
        public string CurrentVersion
        {
            get { return currentVersion; }
            private set { currentVersion = value; }
        }

        private string latestVersion;
        public string LatestVersion
        {
            get { return latestVersion; }
            private set { latestVersion = value; }
        }

        private string downloadUrl;
        public string DownloadUrl
        {
            get { return downloadUrl; }
            private set { downloadUrl = value; }
        }

        internal VersionInformation(string currentVersion, string latestVersion, string downloadUrl)
        {
            // System.Versionが対応可能な形式かどうかチェック
            if (Utility.IsVersionString(currentVersion))
            {
                CurrentVersion = currentVersion;
            }

            if (Utility.IsVersionString(latestVersion))
            {
                LatestVersion = latestVersion;
            }

            DownloadUrl = downloadUrl;
        }

        public Status GetStatus()
        {

            // Version情報がない場合はデータ取得に失敗しているので、Unavailable
            if (string.IsNullOrEmpty(CurrentVersion) || string.IsNullOrEmpty(LatestVersion))
            {
                return Status.Unavailable;
            }

            System.Version current = new System.Version(CurrentVersion);
            System.Version latest = new System.Version(LatestVersion);

            // 現在のバージョンとLatestVersionを比較して同一もしくはLatestVersionが古ければ（通常はないが）UpToDate
            // 引数として渡されたバージョンよりも自身の方が新しければ1、同じであれば0、自身の方が古ければ-1
            if (current.CompareTo(latest) != -1)
            {
                return Status.UpToDate;
            }

            // 有効なUrlでなかったらUnavailableをかえす。
            if (!Utility.IsUrl(DownloadUrl))
            {
                return Status.Unavailable;
            }

            return Status.UpdateAvailable;
        }
    }
}