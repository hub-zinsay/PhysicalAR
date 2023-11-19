using STYLY.Analytics;
using STYLY.MaintenanceTool.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace STYLY.MaintenanceTool
{
    /// <summary>
    /// Local Preview Tool のUIに関するクラス
    /// <para>ビルド関連の処理はここには記述しないようにする。</para>
    /// </summary>
    public class STYLYLocalPreviewToolWindow : EditorWindow
    {
        /// <summary>
        /// 処理中フラグ。
        /// <para>
        /// 通常のビルドでもOnProcessSceneは呼び出されるため、
        /// ローカルテストのためのシーンビルドである事をこのフラグを使って判断する。
        /// </para>
        /// </summary>
        private static bool isProcessing = false;

        /// <summary>PCVRUtilityのリスト</summary>
        private List<IPlatformUtility> pcUtils;

        /// <summary>AndroidVRUtilityのリスト</summary>
        private List<IPlatformUtility> androidUtils;

        /// <summary>pcUtilsとandroidUtilsを結合したリスト</summary>
        private IEnumerable<IPlatformUtility> platformUtils;
        
        /// <summary>
        /// EditorWindowを表示する
        /// </summary>
        [MenuItem("STYLY/STYLY Local Preview Tool")]
        private static void Open()
        {
            EditorWindow.GetWindow<STYLYLocalPreviewToolWindow>("STYLY Local Preview Tool");
        }

        /// <summary>GUIの最下段に表示するステータス</summary>
        private string status;

        /// <summary>ビルド対象シーン</summary>
        [FormerlySerializedAs("targetScene")]
        [SerializeField]
        private string targetScene;

        /// <summary>
        /// 初期化
        /// </summary>
        private void Setup()
        {
            isProcessing = true;

            // pcUtilsを新規作成する
            pcUtils = new List<IPlatformUtility>() { new PCVRUtility() };

            // packagePathからandroidUtilsをインスタンス化する
            var packagePath = PackageManagerUtility.Instance.GetPackagePath("com.psychicvrlab.styly-maintenance-tools");
            AndroidVRUtility.PackageBasePath = packagePath;
            androidUtils = AndroidVRUtility.GetInstances();

            // pcUtilsとandroidUtilsを結合する
            platformUtils = pcUtils.Concat(androidUtils);
        }

        /// <summary>
        /// インスタンス破棄時の処理
        /// </summary>
        private void Dispose()
        {
            isProcessing = false;

            // ユーティリティー系のフィールドをすべて破棄する。
            platformUtils = null;
            pcUtils = null;
            androidUtils = null;
        }

        /// <summary>
        /// EditorWindowのGUIを実装する
        /// </summary>
        private void OnGUI()
        {
            targetScene = SceneManager.GetActiveScene().path;

            // 自身のSerializedObjectを取得
            var so = new SerializedObject(this);

            so.Update();

            GUILayout.Label("Target Scene: " + targetScene);

            // isPlayingが真なら要素を操作不可にする
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            so.ApplyModifiedProperties();

            // Windowsの時だけボタンを表示する
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Preview on PC VR"))
                {
                    Setup();
                    var builder = new LocalPreviewToolBuilder(pcUtils, targetScene);
                    builder.TestBuildAndRunScene();
                    Dispose();
                    status = "Done.";
                    StylyAnalyticsForLocalPreviewTool.Instance.LocalPreview("pc");
                }
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Preview on Android VR"))
            {
                Setup();
                var builder = new LocalPreviewToolBuilder(androidUtils, targetScene);
                builder.TestBuildAndRunScene();
                Dispose();
                status = "Done.";
                StylyAnalyticsForLocalPreviewTool.Instance.LocalPreview("android");
            }

            // Windowsの時だけボタンを表示する
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Preview for PC & Android VR"))
                {
                    Setup();
                    var builder = new LocalPreviewToolBuilder(platformUtils, targetScene);
                    builder.TestBuildAndRunScene();
                    Dispose();
                    status = "Done.";
                    StylyAnalyticsForLocalPreviewTool.Instance.LocalPreview("pc_and_android");
                }
            }

            // Android adb WiFi接続
            GUILayout.Label("adb over WiFi");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Connect"))
            {
                Debug.Log("Connect.");
                Setup();
                LocalPreviewToolUtility.ConnectOverWiFi(androidUtils);
                status = "Connect.";
            }
            if (GUILayout.Button("Disconnect"))
            {
                Debug.Log("Disconnect.");
                Setup();
                LocalPreviewToolUtility.DisonnectOverWiFi(androidUtils);
                status = "Disconnect.";
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            GUILayout.Label("Status: " + status);
        }

        /// <summary>
        /// AssetBundleビルド時にシーン内容を処理する
        /// STYLYLocalPreviewToolWindowにIProcessSceneWithReportを実装すると
        /// Maximize On Playでエラーが発生するためインナークラス化
        /// </summary>
        public class SceneProcessor : IProcessSceneWithReport
        {
            /// <summary>
            /// コールバック順を最優先にする
            /// </summary>
            public int callbackOrder
            {
                get { return 0; }
            }

            /// <summary>
            /// ビルド時にシーンが参照されたときのコールバック
            /// IProcessSceneWithReportのメソッド
            /// </summary>
            /// <param name="scene">参照されたシーン</param>
            /// <param name="report">ビルドレポート</param>
            public void OnProcessScene(Scene scene, BuildReport report)
            {
                // 処理中の場合のみ処理
                if (!isProcessing)
                {
                    return;
                }
                
                Debug.Log("OnProcessScene:" + scene.path);

                var cameras = LocalPreviewToolUtility.GetComponentsInActiveScene<Camera>(false);

                foreach (var camera in cameras)
                {
                    LocalPreviewToolUtility.DeactivateCameraIfUnnecessary(camera);
                }
            }
        }
    }
}
