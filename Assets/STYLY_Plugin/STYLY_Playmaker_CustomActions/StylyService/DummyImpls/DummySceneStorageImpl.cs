using System;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// シーンストレージ読み書きのダミー実装
    /// Unity Editorでアップロード前確認時に動くものとして利用する。
    /// </summary>
    public class DummySceneStorageImpl : IStylyServiceSceneStorageImpl
    {
        private string GetDummyKey(string key, string sceneId)
        {
            return "dummy/" + (sceneId == null ? "currentScene" : sceneId) + $"/{key}";
        }
        
        public string SceneStorageReadString(string key, string defaultValue, string sceneId = null)
        {
            return PlayerPrefs.GetString(GetDummyKey(key, sceneId), defaultValue);
        }

        public void SceneStorageWriteString(string key, string value, string sceneId = null)
        {
            PlayerPrefs.SetString(GetDummyKey(key, sceneId), value);
        }

        public void SceneStorageDeleteKey(string key, string sceneId = null)
        {
            PlayerPrefs.DeleteKey((GetDummyKey(key, sceneId)));
        }
    }
}