using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STYLY.Uploader
{
    public class SceneProcessor : IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        void IProcessSceneWithReport.OnProcessScene(Scene scene, BuildReport report)
        {
            // UnityPlugin For STYLYによるアップロードのためのビルドでない場合は、処理せず終了
            if (!Editor.IsUploading)
            {
                return;
            }

            var cameras = GetComponentsInActiveScene<Camera>(false);

            foreach (var camera in cameras)
            {
                DeactivateCameraIfUnnecessary(camera);
            }
        }

        private void DeactivateCameraIfUnnecessary(Camera camera)
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

        private static T[] GetComponentsInActiveScene<T>(bool includeInactive = true)
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
