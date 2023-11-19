using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

namespace STYLY.MaintenanceTool.Utility
{
    public class PackageManagerUtility
    {
        private static PackageManagerUtility instance;

        public static PackageManagerUtility Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PackageManagerUtility();
                }
                return instance;
            }
        }

        public string targetPackagePath;

        public Dictionary<string, string> packagePathDic = new Dictionary<string, string>();

        /// <summary>
        /// 指定したパッケージのパスを返却する。
        /// </summary>
        /// <param name="packageId">
        /// example:com.psychicvrlab.styly-interaction-sdk
        /// </param>
        /// <returns>
        /// package.jsonまでの絶対パス
        /// 　PackageManagerでfrom diskでImportしている場合
        /// 　　EX) C:\github\STYLY-VR-Interactions\STYLY-InteractionSDK
        /// 　PackageManagerでfrom git URLでImportしている場合
        ///   EX) C:\github\STYLY-Maintenance-tools\STYLY-Maintenance-tools\Library\PackageCache\com.afjk.test-package@d86bc0eb7f
        /// </returns>
        public String GetPackagePath(string packageId)
        {
            IEnumerator t = GetPackagePathCoroutine(packageId);
            while (t.MoveNext())
            {
                Debug.Log("GetPackagePath waiting...");
            }

            return targetPackagePath;
        }

        public IEnumerator GetPackagePathCoroutine(string targetPackage)
        {
            if (packagePathDic.TryGetValue(targetPackage, out targetPackagePath))
            {
                yield break;
            }

            var listRequest = UnityEditor.PackageManager.Client.List();

            while (listRequest.Status == StatusCode.InProgress)
            {
                yield return null;
            }

            if (listRequest.Status == StatusCode.Success)
            {
                foreach (var packageInfo in listRequest.Result)
                {
                    Debug.Log(packageInfo.packageId + " : " + packageInfo.resolvedPath);
                    // @以降は切り捨て
                    string[] buff = packageInfo.packageId.Split('@');

                    packagePathDic[buff[0]] = packageInfo.resolvedPath;
                }
            }

            if (packagePathDic.TryGetValue(targetPackage, out targetPackagePath))
            {
                Debug.Log("Success! :" + targetPackagePath);
            }
        }
    }
}