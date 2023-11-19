using System;

namespace STYLY
{
    /// <summary>
    /// interface to handle web canvas requests
    /// </summary>
    public interface IStylyServiceWebCanvasImpl
    {
        /// <summary>
        /// WebCanvasを初期化する。
        /// </summary>
        void WebCanvasInit();
        
        /// <summary>
        /// WebCanvasを表示する。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="displayCloseButton">閉じるボタンを表示するかどうか</param>
        /// <param name="onFinished">終了時ハンドラ。エラーがあれば引数のExceptionに値が入る。</param>
        void WebCanvasShow(string url, bool displayCloseButton, Action<Exception> onFinished);
       
        /// <summary>
        /// WebCanvasを非表示にする。
        /// </summary>
        /// <param name="onFinished">終了時ハンドラ。エラーがあれば引数のExceptionに値が入る。</param>
        void WebCanvasHide(Action<Exception> onFinished);
    }
}
