using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STYLY.MaintenanceTool.Utility
{
    /// <summary>
    /// Local Preview Tool のビルド処理
    /// <para>AssetBundleのビルド、STYLY起動等を扱う。</para>
    /// <para>UI関連の処理はここには記述しないようにする。</para>
    /// </summary>
    public class LocalPreviewToolBuilder
    {
        /// <summary>ビルド対象のリスト</summary>
        private BuildTarget[] AllBuildTargets =
        {
            BuildTarget.StandaloneWindows64,
            BuildTarget.Android,
        };

        /// <summary>STYLYプラグインユーティリティー</summary>
        private STYLYPluginUtility stylyPluginUtility;

        /// <summary>アセットバンドルユーティリティー</summary>
        private AssetBundleUtility abUtility;

        /// <summary>pcUtilsとandroidUtilsを結合したリスト</summary>
        private IEnumerable<IPlatformUtility> platformUtils;

        /// <summary>ビルド対象シーン</summary>
        private string targetScene;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="platformUtils">pcUtilsとandroidUtilsを結合したリスト</param>
        /// <param name="targetScene">ビルド対象シーン</param>
        public LocalPreviewToolBuilder(IEnumerable<IPlatformUtility> platformUtils, string targetScene)
        {
            // Setup
            stylyPluginUtility = new STYLYPluginUtility();

            // abUtilityを新規作成する
            abUtility = new AssetBundleUtility();

            // DI
            this.platformUtils = platformUtils;
            this.targetScene = targetScene;
        }

        /// <summary>
        /// インスタンス破棄時の処理
        /// </summary>
        private void Dispose()
        {
            // ユーティリティー系のフィールドをすべて破棄する。
            stylyPluginUtility = null;
            abUtility = null;
        }

        /// <summary>
        /// プラットフォームユーティリティーの種類からビルドターゲットを取得する
        /// </summary>
        /// <param name="util">プラットフォームユーティリティー</param>
        /// <returns>ビルド対象OS</returns>
        BuildTarget PlatformUtilToBuildTarget(IPlatformUtility util)
        {
            if (util.GetType() == typeof(PCVRUtility))
            {
                return BuildTarget.StandaloneWindows64;
            }

            if (util.GetType() == typeof(AndroidVRUtility))
            {
                return BuildTarget.Android;
            }

            Debug.LogError("No Target");
            return BuildTarget.NoTarget;
        }

        /// <summary>
        /// シーンをビルドして起動する
        /// </summary>
        public void TestBuildAndRunScene()
        {
            // Outputフォルダを空にする。※毎回必ずビルドする。
            abUtility.ClearOutputDirectory();

            // シーンをビルドする
            LocalPreviewToolError error = BuildSceneToAssetBundle();
            if (error != null)
            {
                EditorUtility.DisplayDialog("Error.", error.GetErrorMessage(), "OK");
                return;
            }

            // シーンXMLを生成する。
            var sceneXml = stylyPluginUtility.GenerateSceneXMLforSceneOnly(targetScene);

            // シーンGUIDを生成する。
            var sceneGuid = abUtility.GenerateGUID();

            // テストモード独自プロトコルでファイルを出力する
            GenerateXmlAndCopyBuildedAssetData(sceneXml, sceneGuid);

            foreach (IPlatformUtility util in platformUtils)
            {
                // STYLYを起動する。
                var urlScheme = util.CreateURLScheme(sceneGuid, "testUser", true);
                var result = util.StartSTYLY(urlScheme);
            }
            Dispose();

            EditorUtility.DisplayDialog("Success.", "STYLY will start soon.", "OK");
        }

        /// <summary>
        /// シーンファイルをビルドしてAssetBundleを作る
        /// <returns>エラーインスタンス。成功時はnull</returns>
        /// </summary>
        private LocalPreviewToolError BuildSceneToAssetBundle()
        {
            // 要素数がゼロならエラーにする
            if (!platformUtils.Any())
            {
                return new LocalPreviewToolError { Type = LocalPreviewToolError.ErrorType.DeviceNotFound };
            }

            // ビルド対象シーンが未設定なら、アクティブシーンを設定する。
            if (string.IsNullOrEmpty(targetScene))
            {
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                targetScene = SceneManager.GetActiveScene().path;
            }

            if (string.IsNullOrEmpty(targetScene))
            {
                return new LocalPreviewToolError { Type = LocalPreviewToolError.ErrorType.TargetSceneNotFound };
            }

            // アクティブシーンが未保存なら保存する。
            if (SceneManager.GetActiveScene().isDirty)
            {
                Debug.Log("Save scene beause dirty");
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            }

            // STYLYを事前に起動する。
            foreach (IPlatformUtility util in platformUtils)
            {
                int result = util.StartSTYLY();
                if (result != 0)
                {
                    return new LocalPreviewToolError { Type = LocalPreviewToolError.ErrorType.StartStylyFailed };
                }
            }

            // ビルド対象のシーンを読み込む
            var scene = EditorSceneManager.OpenScene(targetScene);

            // ビルドが必要なプラットフォームを抽出する
            var needBuildTargets = new HashSet<BuildTarget>();

            // シーンが未ビルドならneedBuildTargetsに加える。
            foreach (IPlatformUtility util in platformUtils)
            {
                if (!abUtility.CheckPrefabAlreadyBuilded(targetScene, PlatformUtilToBuildTarget(util)))
                {
                    needBuildTargets.Add(PlatformUtilToBuildTarget(util));
                }
            }

            // アセットバンドルGUIDを初期化する。
            string abGuid = null;

            if (needBuildTargets.Count > 0)
            {
                Debug.Log("Need to build.");
                // ビルド対象が存在する場合

                // AssetBundleのGUIDを決定する。
                // ビルド済みのプラットフォームが存在する場合、そのGUIDを流用する。
                foreach (var buildTarget in AllBuildTargets)
                {
                    if (abUtility.CheckPrefabAlreadyBuilded(targetScene, buildTarget))
                    {
                        abGuid = abUtility.GetGuidFromBuildedAssetData(targetScene);
                        break;
                    }
                }

                // ビルド済みのプラットフォームが存在しない場合、GUIDを生成する。
                if (abGuid == null)
                {
                    abGuid = abUtility.GenerateGUID();
                }
            }
            else
            {
                Debug.Log("No need to build.");
            }

            // AssetBundleのビルド実行
            foreach (var buildTarget in needBuildTargets)
            {
                var buildResult = stylyPluginUtility.BuildSTYLYSceneAsset(targetScene, buildTarget, abGuid);
                if (buildResult == false)
                {
                    return new LocalPreviewToolError
                    {
                        Type = LocalPreviewToolError.ErrorType.BuildStylySceneAssetFailed,
                        PlatformName = abUtility.GetPlatformName(buildTarget)
                    };
                }
            }
            
            return null;
        }

        /// <summary>
        /// シーンXMLファイルを生成し、persistentDataPath\STYLY_TESTMODEへ出力する。
        /// AssetBundleも同じフォルダへコピーする。
        /// persistentDataPath\STYLY_TESTMODEは毎回クリアする。
        /// </summary>
        /// <param name="sceneXml">シーンXML</param>
        /// <param name="sceneGuid">シーンGUID</param>
        private void GenerateXmlAndCopyBuildedAssetData(string sceneXml, string sceneGuid)
        {
            string guid = abUtility.GetGuidFromBuildedAssetData(targetScene);

            // AssetBundleをSTYLYへコピーする。
            foreach (IPlatformUtility util in platformUtils)
            {
                // STYLY_TESTMODEをクリアする。
                util.ClearSTYLYTestMode();

                // アセットバンドルをコピーする。
                util.CopyBuildedAssetBundleToSTYLYTestMode(guid);

                // シーン情報XMLをSTYLYへ保存する。
                util.SaveSceneXmlToSTYLYTestMode(sceneXml);
            }
        }
    }
}
