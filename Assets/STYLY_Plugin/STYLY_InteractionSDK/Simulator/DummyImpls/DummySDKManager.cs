#pragma warning disable 649

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// シミュレーター用SDKManager。
    /// シーンに配置する事で、Play時にUnityEditor上でInteractionSDKの動作確認する機能を提供する。
    /// ・ダミープレイヤーの生成
    /// ・シーン内のカメラ無効化
    /// </summary>
    public class DummySDKManager : MonoBehaviour
    {
        [SerializeField] private GameObject dummyPlayerPrefab;
        [SerializeField] private GameObject guideCanvas;

        private void Awake()
        {
#if !UNITY_EDITOR
            Destroy(gameObject);
#else
            HideCamera();
            InstantiateDummyPlayer();
            guideCanvas.SetActive(true);
#endif
        }

        void InstantiateDummyPlayer()
        {
            if (dummyPlayerPrefab)
            {
                GameObject.Instantiate(dummyPlayerPrefab, new Vector3(0, 1.3f, -5), Quaternion.identity);
            }
        }

        void HideCamera()
        {
            var cameras = GetComponentsInActiveScene<Camera>(false);

            foreach (var camera in cameras)
            {
                DeactivateCameraIfUnnecessary(camera);
            }
        }

        private static T[] GetComponentsInActiveScene<T>(bool includeInactive = true)
        {
            // ActiveなSceneのRootにあるGameObject[]を取得する
            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            // 空の IEnumerable<T>
            IEnumerable<T> resultComponents = (T[]) Enumerable.Empty<T>();
            foreach (var item in rootGameObjects)
            {
                // includeInactive = true を指定するとGameObjectが非活性なものからも取得する
                var components = item.GetComponentsInChildren<T>(includeInactive);
                resultComponents = resultComponents.Concat(components);
            }

            return resultComponents.ToArray();
        }

        private void DeactivateCameraIfUnnecessary(Camera camera)
        {
            var audioListener = camera.GetComponent<AudioListener>();
            if (audioListener) { audioListener.enabled = false;}
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
        }
    }
}
