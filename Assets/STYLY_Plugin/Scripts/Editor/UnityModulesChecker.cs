using UnityEditor;

namespace STYLY.Uploader
{
    /// <summary>
    /// Unityモジュールのインストール状況を確認するクラス
    /// 参考: http://answers.unity3d.com/questions/1324195/detect-if-build-target-is-installed.html
    /// </summary>
    public class UnityModulesChecker
    {
        /// <summary>IsPlatformSupportLoadedを格納するメソッドインフォ</summary>
        private System.Reflection.MethodInfo isPlatformSupportLoaded;
        
        /// <summary>GetTargetStringFromBuildTargetを格納するメソッドインフォ</summary>
        private System.Reflection.MethodInfo getTargetStringFromBuildTarget;

        /// <summary>
        /// コンストラクター
        /// </summary>
        public UnityModulesChecker()
        {
            var moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
            isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        }

        /// <summary>
        /// ビルドターゲットからターゲット名を取得する
        /// </summary>
        /// <param name="target">ビルドターゲット</param>
        /// <returns>プラットフォーム名</returns>
        private string GetTargetStringFromBuildTarget(BuildTarget target)
        {
            return (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { target });
        }

        /// <summary>
        /// プラットフォームサポートがロードされているか確認する
        /// </summary>
        /// <param name="platform">プラットフォーム名</param>
        /// <returns>ロードされているかどうか</returns>
        private bool IsPlatformSupportLoaded(string platform)
        {
            return (bool)isPlatformSupportLoaded.Invoke(null, new object[] { platform });
        }

        /// <summary>
        /// ビルドターゲットのプラットフォームサポートがロードされているか確認する
        /// </summary>
        /// <param name="target">ビルドターゲット</param>
        /// <returns>ロードされているかどうか</returns>
        public bool IsPlatformSupportLoaded(BuildTarget target)
        {
            return IsPlatformSupportLoaded(GetTargetStringFromBuildTarget(target));
        }
    }
}
