using System;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// interface to handle GLTF Load requests
    /// </summary>
    public interface IStylyServiceGLTFImpl
    {
        /// <summary>
        /// GLTFをロードする
        /// </summary>
        /// <param name="parentObject">ロード結果の親となるオブジェクト</param>
        /// <param name="sourceUrl">ロード元のURL</param>
        /// <param name="activeOnStart">ロード後のアクティブ</param>
        /// <param name="generateCollider">コライダー生成</param>
        /// <param name="onFinished">終了時のコールバックイベント</param>
        void GLTFLoad(GameObject parentObject, string sourceUrl, bool activeOnStart, bool generateCollider,
            Action<GameObject, Exception> onFinished);
    }
}