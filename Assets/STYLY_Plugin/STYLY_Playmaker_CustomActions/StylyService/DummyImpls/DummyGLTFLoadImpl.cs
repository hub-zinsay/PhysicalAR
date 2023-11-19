using System;
using System.Collections;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// ダミーのGLTF読み込み実装。
    /// "シーン編集用プロジェクトのUnityEditor上"でのみ使用される。ロード結果のダミーとしてCubeが出現する。
    /// </summary>
    public class DummyGLTFLoadImpl : MonoBehaviour,IStylyServiceGLTFImpl
    {
        /// <summary>
        /// ダミーの読み込み処理 本番の挙動に合わせて非同期で処理
        /// </summary>
        /// <param name="parentObject">ロード結果の親となるオブジェクト</param>
        /// <param name="activeOnStart">ロード後のアクティブ</param>
        /// <param name="generateCollider">コライダー生成</param>
        /// <param name="onFinished">検知したエラー</param>
        private IEnumerator GLTFLoadCoroutine(GameObject parentObject, bool activeOnStart,
            bool generateCollider, Action<GameObject, Exception> onFinished)
        {
            //成功時のシミュレート　Cubeが出てくるだけ
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Dummy load result";
            cube.transform.SetParent(parentObject.transform,false);
            cube.GetComponent<Collider>().enabled = generateCollider;
            cube.SetActive(activeOnStart);
            yield return null;
            onFinished(cube, null);
        }

        /// <summary>
        /// ダミーの読み込み処理
        /// </summary>
        /// <param name="parentObject">ロード結果の親となるオブジェクト</param>
        /// <param name="sourceUrl">ロード元のURL</param>
        /// <param name="activeOnStart">ロード後のアクティブ</param>
        /// <param name="generateCollider">コライダー生成</param>
        /// <param name="onFinished">終了時のコールバックイベント</param>
        public void GLTFLoad(GameObject parentObject, string sourceUrl, bool activeOnStart, bool generateCollider,
            Action<GameObject, Exception> onFinished)
        {
            StartCoroutine(GLTFLoadCoroutine(parentObject, activeOnStart, generateCollider, onFinished));
        }
    }
}
