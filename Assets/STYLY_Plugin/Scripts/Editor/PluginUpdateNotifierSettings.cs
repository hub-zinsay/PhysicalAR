using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace STYLY.Uploader
{
    internal class PluginUpdateNotifierSettings : ScriptableObject
    {
        const string settingsAssetPath = "Assets/STYLY_Plugin/PluginUpdateNotifierSettings.asset";

        public static PluginUpdateNotifierSettings LoadOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<PluginUpdateNotifierSettings>(settingsAssetPath);

            if (settings == null) 
            {
                settings = ScriptableObject.CreateInstance<PluginUpdateNotifierSettings>();
                // Startup時に最新Updateが存在する場合の告知をするか
                // デフォルトはOn（true）
                settings.ShowAtStartup = true;
                AssetDatabase.CreateAsset(settings, settingsAssetPath);
            }

            return settings;
        }

        #region Properties and Fields

        [SerializeField]
        [HideInInspector]
        private bool showAtStartup;
        [SerializeField]
        [HideInInspector]
        private string skipVersion;

        public bool ShowAtStartup 
        {
            get { return showAtStartup; }
            set 
            {
                showAtStartup = value;
                Save();
            }
        }

        public string SkipVersion
        {
            get { return skipVersion; }
            set
            {
                skipVersion = value;
                Save();
            }
        }

        #endregion

        public void Save() 
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

