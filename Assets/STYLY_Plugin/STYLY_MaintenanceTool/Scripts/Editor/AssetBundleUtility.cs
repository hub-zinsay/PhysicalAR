using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace STYLY.MaintenanceTool.Utility
{
    /// <summary>
    /// AssetBundle操作用Utilityクラス
    /// </summary>
    public class AssetBundleUtility
    {
        private const string OutputPath = "_Output";

        public bool SwitchPlatformAndPlayerSettings(BuildTarget buildTarget)
        {
            return SwitchPlatformAndPlayerSettings(GetRuntimePlatform(buildTarget));
        }

        public bool SwitchPlatformAndPlayerSettings(RuntimePlatform platform)
        {
            bool switchResult = false;
            if (platform == RuntimePlatform.WindowsPlayer)
            {
                switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new UnityEngine.Rendering.GraphicsDeviceType[] {
                    UnityEngine.Rendering.GraphicsDeviceType.Direct3D11
                });
            }
            else if (platform == RuntimePlatform.Android)
            {
                switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new UnityEngine.Rendering.GraphicsDeviceType[] {
                    UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2,
                    UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3
                });
            }
            else if (platform == RuntimePlatform.IPhonePlayer)
            {
                switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new UnityEngine.Rendering.GraphicsDeviceType[] {
                    UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2,
                    UnityEngine.Rendering.GraphicsDeviceType.Metal
                });
            }
            else if (platform == RuntimePlatform.OSXPlayer)
            {
                switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneOSX, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneOSX, new UnityEngine.Rendering.GraphicsDeviceType[] {
                    UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2,
                    UnityEngine.Rendering.GraphicsDeviceType.Metal
                });
            }
            else if (platform == RuntimePlatform.WebGLPlayer)
            {

                switchResult = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, true);
            }
            else if (platform == RuntimePlatform.WSAPlayerX86)
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.WSA, ScriptingImplementation.WinRTDotNET);
            }

            return switchResult;
        }

        public string GenerateGUID()
        {
            return Guid.NewGuid().ToString("D");
        }

        /// <summary>
        /// AssetBundleをビルドする
        /// </summary>
        /// <param name="guid">AssetBundleのGUID</param>
        /// <param name="path">シーンのパス</param>
        /// <param name="outputPath">出力ファイルパス</param>
        /// <param name="buildTarget">ビルドターゲット</param>
        /// <returns>ビルド結果</returns>
        public AssetBundleManifest Build(string guid, string path, string outputPath, BuildTarget buildTarget)
        {
            Debug.Log("guid:" + guid + " path:" + path + " outputPath:" + outputPath);

            // if (CheckPrefabAlreadyBuilded(path, buildTarget))
            // {
            //     Debug.Log("Already Builded.");
            //     return null;
            // }

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            // pathをGUID名にリネームする。
            var guidPath = GetAssetGUIDName(path, guid);
            RenameAsset(path, guidPath);

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = guid;
            buildMap[0].assetNames = new string[] { guidPath };
            var buildResult = BuildPipeline.BuildAssetBundles(outputPath, buildMap, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);

            // リネームしたPathを戻す。
            RenameAsset(guidPath, path);

            // ビルド結果保存
            var builded = GetBuildedAssetData();
            builded.AddData(path, guid);

            return buildResult;
        }

        // Assetファイル名をGUID名に変更
        public string GetAssetGUIDName(string assetPath, string stylyGuid)
        {
            var renamedAssetPath = Path.GetDirectoryName(assetPath) + "/" + stylyGuid + Path.GetExtension(assetPath);

            return renamedAssetPath;
        }

        // AssetのRename
        public void RenameAsset(string assetPath, string renamedAssetPath)
        {
            Debug.Log("assetpath:" + assetPath + " rename:" + renamedAssetPath);
            AssetDatabase.MoveAsset(assetPath, renamedAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void ClearOutputDirectory()
        {
            Delete(OutputPath);
        }

        /// 指定したディレクトリとその中身を全て削除する
        public static void Delete(string targetDirectoryPath)
        {
            Debug.Log("Delete");

            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                Delete(directoryPath);
            }

            Directory.Delete(targetDirectoryPath, false);
        }

        /// <summary>
        /// ビルドターゲットからプラットフォーム名を取得する
        /// </summary>
        /// <param name="platform">ビルドターゲット</param>
        /// <returns>プラットフォーム名</returns>
        public string GetPlatformName(BuildTarget platform)
        {
            switch (platform)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.WSAPlayer:
                    return "UWP";
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSX:
                    return "OSX";
                default:
                    return null;
            }
        }

        /// <summary>
        /// ビルドターゲットからランタイムプラットフォームを取得する
        /// </summary>
        /// <param name="buildTarget">ビルドターゲット</param>
        /// <returns>ランタイムプラットフォーム</returns>
        public RuntimePlatform GetRuntimePlatform(BuildTarget buildTarget)
        {
            RuntimePlatform platform = RuntimePlatform.WindowsPlayer;
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    platform = RuntimePlatform.Android;
                    break;
                case BuildTarget.StandaloneWindows64:
                    platform = RuntimePlatform.WindowsPlayer;
                    break;
                case BuildTarget.iOS:
                    platform = RuntimePlatform.IPhonePlayer;
                    break;

                case BuildTarget.StandaloneOSX:
                    platform = RuntimePlatform.OSXPlayer;
                    break;
                case BuildTarget.WebGL:
                    platform = RuntimePlatform.WebGLPlayer;
                    break;
            }

            return platform;
        }

        private const string STYLY_BUILDED_ASSET_PATH_DATA = "Assets/styly_temp/BuildedAssetPathData.asset";
        public BuildedAssetPathData GetBuildedAssetData()
        {
            var buildedAssetPathData = AssetDatabase.LoadAssetAtPath<BuildedAssetPathData>(STYLY_BUILDED_ASSET_PATH_DATA);
            if (buildedAssetPathData == null)
            {
                buildedAssetPathData = ClearBuildedAssetPathData();
            }

            return buildedAssetPathData;
        }

        public BuildedAssetPathData ClearBuildedAssetPathData()
        {
            Debug.Log("new");
            var buildedAssetPathData = ScriptableObject.CreateInstance<BuildedAssetPathData>();

            if (!Directory.Exists(Path.GetDirectoryName(STYLY_BUILDED_ASSET_PATH_DATA)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(STYLY_BUILDED_ASSET_PATH_DATA));
            }
            AssetDatabase.CreateAsset(buildedAssetPathData, STYLY_BUILDED_ASSET_PATH_DATA);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return buildedAssetPathData;
        }

        /// <summary>
        /// Prefabがビルド済みかどうかBuildedAssetPathDataから判定する。
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="buildTarget"></param>
        /// <returns>true: Already builded. false:Need to build.</returns>
        public bool CheckPrefabAlreadyBuilded(string prefabPath, BuildTarget buildTarget)
        {
            Debug.Log("CheckPrefabAlreadyBuilded:" + prefabPath);

            // AssetBundleのパスが記録されていなければビルド必要
            var assetBundlePath = GetAssetBundlePath(prefabPath, buildTarget);
            if (assetBundlePath == null)
            {
                Debug.Log("Need to build.");
                return false;
            }

            // ファイルの存在チェック
            // 存在しなければビルド必要
            Debug.Log("Exist is:" + File.Exists(assetBundlePath) + " path:" + assetBundlePath);
            if (!File.Exists(assetBundlePath))
            {
                Debug.Log("need build:assetBundle is not Exitsts");
                return false;
            }

            var prefabCreationTime = File.GetCreationTime(prefabPath);
            var assetBundleCreationTime = File.GetCreationTime(assetBundlePath);

            // Prefab/SceneファイルとAssetBundleのビルド時間の比較
            // Prefab/Sceneの方が古ければビルド必要
            Debug.Log("prefabCreationTime:" + prefabCreationTime + " assetBundle CreationTime:" + assetBundleCreationTime);
            if (assetBundleCreationTime < prefabCreationTime)
            {
                Debug.Log("need build:assetBundle is older than prefab.");
                return false;
            }

            return true;
        }

        public string GetAssetBundlePath(string prefabPath, BuildTarget buildTarget)
        {
            var buildedAssetData = GetBuildedAssetData();
            var buildedData = buildedAssetData.GetData(prefabPath);

            // 対象プラットフォームのビルドがあるか確認する。
            string assetBundlePath = Path.Combine(OutputPath, "STYLY_ASSET", GetPlatformName(buildTarget));
            string val;
            if (!buildedData.TryGetValue(BuildedAssetPathData.GUID_KEY, out val))
            {
                return null;
            }
            assetBundlePath = Path.Combine(assetBundlePath, val);

            return assetBundlePath;
        }

        public string GetGuidFromBuildedAssetData(string prefabPath)
        {
            var buildedAssetData = GetBuildedAssetData();
            var buildedData = buildedAssetData.GetData(prefabPath);

            string guidString;
            if (buildedData.TryGetValue(BuildedAssetPathData.GUID_KEY, out guidString))
            {
                return guidString;
            }
            else
            {
                return null;
            }
        }
    }
}

