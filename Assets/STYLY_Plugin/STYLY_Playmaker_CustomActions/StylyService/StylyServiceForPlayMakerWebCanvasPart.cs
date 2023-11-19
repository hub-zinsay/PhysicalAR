using System;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// web canvas part of StylyServiceForPlayMaker (partial class)
    /// </summary>
    public partial class StylyServiceForPlayMaker
    {
        /// <summary>
        /// 実装部
        /// </summary>
        private IStylyServiceWebCanvasImpl _webCanvasImpl;

        /// <summary>
        /// 実装部のセッター
        /// </summary>
        /// <param name="impl">実装部</param>
        public void SetWebCanvasImpl(IStylyServiceWebCanvasImpl impl)
        {
            _webCanvasImpl = impl;
        }

        /// <summary>
        /// WebCanvasを初期化する。
        /// </summary>
        public void WebCanvasInit()
        {
            if (_webCanvasImpl != null)
            {
                _webCanvasImpl.WebCanvasInit();
            }
            else
            {
                Debug.Log("WebCanvasInit called with");
            }
        }

        /// <summary>
        /// WebCanvasを表示する。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="displayCloseButton">閉じるボタンを表示するかどうか</param>
        /// <param name="onFinished">終了時ハンドラ。エラーがあれば引数のExceptionに値が入る。</param>
        public void WebCanvasShow(string url, bool displayCloseButton, Action<Exception> onFinished)
        {
            if (_webCanvasImpl != null)
            {
                _webCanvasImpl.WebCanvasShow(url, displayCloseButton, onFinished);
            }
            else
            {
                Debug.Log("WebCanvasShow called with url: <" + url + ">");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }

        /// <summary>
        /// WebCanvasを非表示にする。
        /// </summary>
        /// <param name="onFinished">終了時ハンドラ。エラーがあれば引数のExceptionに値が入る。</param>
        public void WebCanvasHide(Action<Exception> onFinished)
        {
            if (_webCanvasImpl != null)
            {
                _webCanvasImpl.WebCanvasHide(onFinished);
            }
            else
            {
                Debug.Log("WebCanvasHide called");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }
    }
}
