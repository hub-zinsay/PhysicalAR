using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace STYLY.MaintenanceTool
{
    public class BuildedAssetPathData : ScriptableObject
    {
        public List<string> prefabPathList;
        public List<string> guidList;

        public void AddData(string prefabPath, string assetPath)
        {
            if (prefabPathList == null)
            {
                prefabPathList = new List<string>();
                guidList = new List<string>();
            }

            var index = prefabPathList.IndexOf(prefabPath);
            if (index < 0)
            {
                prefabPathList.Add(prefabPath);
                guidList.Add(assetPath);
            }
            else
            {
                prefabPathList[index] = prefabPath;
                guidList[index] = assetPath;
            }

            // 値の保存
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public const string PREFAB_PATH_KEY = "prefab path";
        public const string GUID_KEY = "guid";

        public Dictionary<string, string> GetData(string prefabPath)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (prefabPathList == null)
            {
                prefabPathList = new List<string>();
                guidList = new List<string>();
            }

            var index = prefabPathList.IndexOf(prefabPath);

            if (index >= 0)
            {
                result.Add(PREFAB_PATH_KEY, prefabPathList[index]);
                result.Add(GUID_KEY, guidList[index]);
            }

            return result;
        }
    }
}
