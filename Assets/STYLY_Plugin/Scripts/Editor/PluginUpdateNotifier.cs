using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using STYLY.Uploader;
using System;
using UnityEngine.Networking;

namespace STYLY.Uploader
{
    [InitializeOnLoad]
    /// <summary>
    /// 最新のバージョン情報の更新を確認して、更新があればNotifyPluginUpdateWindowを開いて通知するクラス
    /// </summary>
    public class PluginUpdateNotifier
    {
        static PluginUpdateNotifier()
        {
            EditorApplication.update += OnStartUp;
        }
        //
        // Startup時PluginのUpdateの有無のチェックし、有効なUpdateがある場合はNotifyPluginUpdateWindowをOpenする。
        private static void OnStartUp()
        {
            EditorApplication.update -= OnStartUp;

            VersionInformationProvider.Instance.CheckForLatestVersion(() =>
            {
                var settings = PluginUpdateNotifierSettings.LoadOrCreateSettings();
                var versionInformation = VersionInformationProvider.Instance.VersionInformation;

                // 更新を通知すべきか確認。必要ならNotifyPluginUpdateWindowを開いて通知する。
                if (CheckIfNotifyUpdateOnStartUp(settings, versionInformation))
                {
                    UI.NotifyPluginUpdateWindow.Open();
                }
            }, (ex) => OnErrorCheckForUpdate(ex));
        }

        /// <summary>
        /// Pluginの更新情報を通知するか（NotifyPluginUpdateWindowを開くかどうか）チェックするためのメソッド。trueは通知する。
        /// </summary>
        /// <returns></returns>
        private static bool CheckIfNotifyUpdateOnStartUp(PluginUpdateNotifierSettings settings, VersionInformation versionInformation)
        {
            // UpdateAvailableでない時は通知しない。
            if (versionInformation.GetStatus() != VersionInformation.Status.UpdateAvailable)
            {
                return false;
            }

            // SettingsのShow At Startupがfalseなら通知しない。
            if (!settings.ShowAtStartup)
            {
                return false;
            }

            // スキップ対象バージョンなら通知しない。
            if (CheckIfSkipUpdate(settings, versionInformation)) 
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// VersionInformation.LatestVersionが通知スキップ対象バージョンかどうかチェックする。trueならスキップ対象バージョン。
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="versionInformation"></param>
        private static bool CheckIfSkipUpdate(PluginUpdateNotifierSettings settings, VersionInformation versionInformation)
        {
            // SkipVersionの値が空白の時はスキップしない。
            if(string.IsNullOrEmpty(settings.SkipVersion))
            {
                return false;
            }

            // なんらかの理由でSkipVersionが不適切な値だった場合はスキップしない。
            if (!Utility.IsVersionString(settings.SkipVersion)) 
            {
                return false;
            }

            System.Version skipVersion = new System.Version(settings.SkipVersion);
            System.Version latestVersion = new System.Version(versionInformation.LatestVersion);
            // LatestVersionとSkipVersionが同一でないならスキップしない。
            if (skipVersion.CompareTo(latestVersion) != 0)
            {
                return false;
            }

            return true;
        }

        [MenuItem("STYLY/Check for Updates")]
        public static void OpenWindowFromMenuItem()
        {
            VersionInformationProvider.Instance.CheckForLatestVersion(() =>
            {
                UI.NotifyPluginUpdateWindow.Open();
            },
            (ex) =>
            {
                // 何らかの理由で対象URLから正しいJsonが取得できなかった状態
                // エラー時の処理
                OnErrorCheckForUpdate(ex);

                // エラーでもウィンドウは開く
                UI.NotifyPluginUpdateWindow.Open();
            });
        }

        private static void OnErrorCheckForUpdate(Exception ex)
        {
            Debug.Log("[STYLY Plugin] failed to fetch latest version info. exception:" + ex);
        }
    }
}