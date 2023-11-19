using UnityEngine;

namespace STYLY.MaintenanceTool
{
    //STYLYアセットとデータとして保持する内容
    [System.Serializable]
    public class stylyAssetData
    {
        public string prefabName;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public string title;
        public string description;
        public string exclusiveCategory;
        public string[] vals;
        public string itemURL;
        public string visible = true.ToString();

        // 同一アセットチェック用キー生成
        public string GetSameCheckKey()
        {
            string valStr = "";
            if (vals != null)
            {
                valStr = string.Join(",", vals);
            }
            return prefabName + exclusiveCategory + valStr + itemURL + visible;
        }
    }

    [System.Serializable]
    public class stylyAssetDataSet
    {
        public stylyAssetData[] AssetDataSet;

        public bool IsEmpty
        {
            get { return (AssetDataSet.Length == 0); }
        }
    }

}
