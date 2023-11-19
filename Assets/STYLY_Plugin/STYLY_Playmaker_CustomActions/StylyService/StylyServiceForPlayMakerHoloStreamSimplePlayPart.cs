using System;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// HoloStreamSimplePlayer of StylyServiceForPlayMaker (partial class)
    /// </summary>
    public partial class StylyServiceForPlayMaker
    {
        private IStylyServiceHoloStreamSimplePlayImpl holoStreamSimplePlayImpl;

        /// <summary>
        /// インターフェースをセット
        /// </summary>
        /// <param name="impl">実際の機能の中身</param>
        public void SetHoloStreamSimplePlayImpl(IStylyServiceHoloStreamSimplePlayImpl impl)
        {
            //Implの上書きはできない
            //テスト時にテスト用Implが上書きされることを防ぐため
            if (holoStreamSimplePlayImpl == null)
            {
                this.holoStreamSimplePlayImpl = impl;
            }
        }
        
        /// <summary>
        /// インターフェースをリセット
        /// </summary>
        public void ResetImpl()
        {
            holoStreamSimplePlayImpl = null;
        }

        /// <summary>
        /// 動画をロードして自動再生する
        /// </summary>
        /// <param name="parentObject">親オブジェクト</param>
        /// <param name="sourceUrl">ロード元のURL</param>
        /// <param name="onFinished">終了時のコールバックイベント</param>
        public void PlayOnLoad(GameObject parentObject, string sourceUrl, Action<Exception> onFinished)
        {
            if (holoStreamSimplePlayImpl != null)
            {
                holoStreamSimplePlayImpl.PlayOnLoad(parentObject, sourceUrl, onFinished);
            }
            else
            {
                //未実装ならエラー通知
                Debug.Log("HoloStreamSimplePlay called with url: <" + sourceUrl + ">");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }
    }
}
