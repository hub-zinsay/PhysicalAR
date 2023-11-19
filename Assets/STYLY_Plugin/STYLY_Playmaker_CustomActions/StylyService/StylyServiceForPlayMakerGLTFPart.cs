using System;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// GLTF load of StylyServiceForPlayMaker (partial class)
    /// </summary>
    public partial class StylyServiceForPlayMaker
    {
        /// <summary>
        /// GLTFの読み込み機能のインターフェース
        /// </summary>
        private IStylyServiceGLTFImpl gltfImpl;
        
        /// <summary>
        /// インターフェースをセット
        /// </summary>
        /// <param name="impl">実際の機能の中身</param>
        public void SetGLTFImpl(IStylyServiceGLTFImpl impl)
        {
            this.gltfImpl = impl;
        }

        /// <summary>
        /// GLTFをロードする
        /// </summary>
        /// <param name="parentObject">ロード結果の親となるオブジェクト</param>
        /// <param name="sourceUrl">ロード元のURL</param>
        /// <param name="activeOnStart">ロード後のアクティブ</param>
        /// <param name="generateCollider">コライダー生成</param>
        /// <param name="onFinished">終了時のコールバックイベント</param>
        public void GLTFLoad(GameObject parentObject, string sourceUrl, bool activeOnStart,
            bool generateCollider, Action<GameObject, Exception> onFinished)
        {
            // 実装が存在せず、Unity Editor上の実行の場合はアップロード環境と判断し、ダミー用の実装をセットする
            if (gltfImpl == null && Application.isEditor)
            {
                // DummyGLTFLoadImplはずっと利用するのでDontDestroyOnLoadなGameObjectにしておく
                var dummyGameObject = new GameObject("DummyGLTFLoader");
                GameObject.DontDestroyOnLoad(dummyGameObject);
                var dummyImpl = dummyGameObject.AddComponent<DummyGLTFLoadImpl>();
                if (dummyImpl == null)
                {
                    onFinished(null, new Exception("AddComponent of DummyGLTFLoadImpl failed."));
                    return;
                }
                SetGLTFImpl(dummyImpl);
            }

            if (gltfImpl != null)
            {
                gltfImpl.GLTFLoad(parentObject, sourceUrl, activeOnStart, generateCollider, onFinished);
            }
            else
            {
                //未実装ならエラー通知
                Debug.Log("GLTFLoad called with url: <" + sourceUrl + ">");
                onFinished(null, new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }
    }
}