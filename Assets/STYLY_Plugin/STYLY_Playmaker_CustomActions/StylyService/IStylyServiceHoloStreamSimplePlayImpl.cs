using System;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// interface to play and load HoloStreamPlayer video
    /// </summary>
    public interface IStylyServiceHoloStreamSimplePlayImpl
    {
        /// <summary>
        /// 動画をロードして自動再生する
        /// </summary>
        /// <param name="parentObject">親オブジェクト</param>
        /// <param name="sourceUrl">ロード元のURL</param>
        /// <param name="onFinished">終了時のコールバックイベント</param>
        void PlayOnLoad(GameObject parentObject, string sourceUrl, Action<Exception> onFinished);
    }
}
