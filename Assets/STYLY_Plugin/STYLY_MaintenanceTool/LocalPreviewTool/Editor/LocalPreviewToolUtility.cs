using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STYLY.MaintenanceTool.Utility
{
    /// <summary>
    /// Local Preview Tool 用のユーティリティークラス
    /// <para>staticメソッドとして記述できる処理のみを扱う。</para>
    /// </summary>
    public static class LocalPreviewToolUtility
    {
        /// <summary>
        /// Android adb WiFiへ接続する
        /// </summary>
        /// <param name="androidUtils">AndroidVRUtilityのリスト</param>
        public static void ConnectOverWiFi(List<IPlatformUtility> androidUtils)
        {
            foreach (AndroidVRUtility util in androidUtils)
            {
                util.ConnectWifi();
            }
        }

        /// <summary>
        /// Android adb WiFiから切断する
        /// </summary>
        /// <param name="androidUtils">AndroidVRUtilityのリスト</param>
        public static void DisonnectOverWiFi(List<IPlatformUtility> androidUtils)
        {
            foreach (AndroidVRUtility util in androidUtils)
            {
                util.DisconnectWifi();
            }
        }

        /// <summary>
        /// 不必要であればカメラを非アクティブ化する
        /// </summary>
        /// <param name="camera">カメラ</param>
        public static void DeactivateCameraIfUnnecessary(Camera camera)
        {
            // ターゲットになるRenderTextureが設定されている場合は非アクティブ化しない。
            if (camera.targetTexture != null)
            {
                return;
            }

            // stereoTargetEyeが0（＝StereoTargetEyeMask.None）なら非アクティブ化しない。
            if (camera.stereoTargetEye == StereoTargetEyeMask.None)
            {
                return;
            }

            Debug.Log("deactivate Camera:" + camera.name);
            // それ以外は非アクティブ化
            camera.enabled = false;
            
            // カメラをdisableする場合はAudioListenerもdisableする。
            // これをしないと、Audio Listenerが２つあるという警告が出がちなため。
            var audioListener = camera.GetComponent<AudioListener>();
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
        }

        /// <summary>
        /// アクティブシーンから指定した種類のコンポーネントを取得する
        /// </summary>
        /// <typeparam name="T">コンポーネントの種類</typeparam>
        /// <param name="includeInactive">非アクティブのものも含めるかどうか</param>
        /// <returns></returns>
        public static T[] GetComponentsInActiveScene<T>(bool includeInactive = true)
        {
            // ActiveなSceneのRootにあるGameObject[]を取得する
            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            // 空の IEnumerable<T>
            IEnumerable<T> resultComponents = (T[])Enumerable.Empty<T>();
            foreach (var item in rootGameObjects)
            {
                // includeInactive = true を指定するとGameObjectが非活性なものからも取得する
                var components = item.GetComponentsInChildren<T>(includeInactive);
                resultComponents = resultComponents.Concat(components);
            }

            return resultComponents.ToArray();
        }
    }
}
