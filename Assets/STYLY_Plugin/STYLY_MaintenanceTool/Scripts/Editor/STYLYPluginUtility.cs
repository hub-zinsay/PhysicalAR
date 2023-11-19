using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STYLY.MaintenanceTool.Utility
{
    /// <summary>
    /// STYLY Pluginの機能を実装するUtilityクラス
    /// </summary>
    public class STYLYPluginUtility
    {
        private const string OutputPath = "_Output/";

        /// <summary>
        /// AssetBundleのビルドを実行する
        /// </summary>
        /// <param name="scenePath">シーンのパス</param>
        /// <param name="buildTarget">ビルドターゲット（プラットフォーム）</param>
        /// <param name="guid">アセットバンドルのGUID</param>
        /// <returns></returns>
        public bool BuildSTYLYSceneAsset(string scenePath, BuildTarget buildTarget, string guid = null)
        {
            Debug.Log("BuildSTYLYSceneAsset:guid:" + guid);
            var abUtility = new AssetBundleUtility();

            // プラットフォーム切換え
            abUtility.SwitchPlatformAndPlayerSettings(buildTarget);

            if (guid == null)
            {
                guid = abUtility.GenerateGUID();
            }

            string outputPath = Path.Combine(OutputPath + "STYLY_ASSET", abUtility.GetPlatformName(buildTarget));

            var buildResult = abUtility.Build(guid, scenePath, outputPath, buildTarget);

            return (buildResult != null);
        }

        public string GenerateSceneXMLforSceneOnly(string scenePath)
        {
            var stylyAssetDataSet = CreateStylyAssetDataSetforSceneOnly(scenePath);
            return STYLY.STYLY_Functions.XmlUtil.SerializeToXmlString<stylyAssetDataSet>(stylyAssetDataSet);
        }

        public stylyAssetDataSet CreateStylyAssetDataSetforSceneOnly(string scenePath)
        {
            //保存するアセットの情報をシリアライズするかするためにクラスに格納
            List<stylyAssetData> _stylyAssetsDataSetList = new List<stylyAssetData>();
            stylyAssetDataSet _stylyAssetsDataSet = new stylyAssetDataSet();

            stylyAssetData _stylyAssetsData = new stylyAssetData();
            _stylyAssetsData.prefabName = GetBuildedGUID(scenePath);
            _stylyAssetsData.Position = Vector3.zero;
            _stylyAssetsData.Rotation = Quaternion.identity;
            _stylyAssetsData.Scale = Vector3.one;
            _stylyAssetsData.title = Path.GetFileNameWithoutExtension(scenePath);
            _stylyAssetsData.description = "";
            _stylyAssetsData.exclusiveCategory = "scene";
            _stylyAssetsData.itemURL = "";
            _stylyAssetsData.vals = new string[] { };
            _stylyAssetsData.visible = true.ToString();

            //DataSetに追加
            _stylyAssetsDataSetList.Add(_stylyAssetsData);
            _stylyAssetsDataSet.AssetDataSet = _stylyAssetsDataSetList.ToArray();

            return _stylyAssetsDataSet;
        }

        string GetBuildedGUID(string prefabPath)
        {
            var abUtility = new AssetBundleUtility();
            var buildedAssetData = abUtility.GetBuildedAssetData();
            var buildedData = buildedAssetData.GetData(prefabPath);
            string guid = null;
            buildedData.TryGetValue(BuildedAssetPathData.GUID_KEY, out guid);
            return guid;
        }
    }
}

namespace STYLY.STYLY_Functions
{
    //http://ftvoid.com/blog/post/1061
    public class XmlUtil
    {
        // クラスのシリアライズをXMLテキストで取得
        public static string SerializeToXmlString<T>(T data)
        {
            var output = new MemoryStream();
            var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

            using (var xmlWriter = XmlWriter.Create(output, settings))
            {
                var serializer = new XmlSerializer(typeof(T));
                var namespaces = new XmlSerializerNamespaces();
                xmlWriter.WriteStartDocument();
                namespaces.Add(string.Empty, string.Empty);
                serializer.Serialize(xmlWriter, data, namespaces);
            }
            output.Seek(0L, SeekOrigin.Begin);
            string xml = new StreamReader(output).ReadToEnd();
            return xml;
        }
    }
}
