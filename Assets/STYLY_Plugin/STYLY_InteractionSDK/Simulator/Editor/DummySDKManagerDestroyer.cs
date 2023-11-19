using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// DummySDKManagerをビルド時に無効化するもの。
    /// </summary>
    public class DummySDKManagerDestroyer : IProcessSceneWithReport
    {

        public int callbackOrder
        {
            get { return 0; }
        }

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            var rootGameObjects = scene.GetRootGameObjects();

            foreach (var rootGameObject in rootGameObjects)
            {
                var dummySDKManagers = rootGameObject.GetComponentsInChildren<DummySDKManager>();

                foreach (var dummySdkManager in dummySDKManagers)
                {
                    dummySdkManager.gameObject.SetActive(false);
                }
            }
        }
    }
}