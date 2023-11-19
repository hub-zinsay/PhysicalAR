using System;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// AR Detector of StylyServiceForPlayMaker (partial class)
    /// </summary>
    public partial class StylyServiceForPlayMaker
    {
        /// <summary>
        /// ARの位置合わせ機能のインターフェース
        /// </summary>
        private IStylyServiceARDetectorImpl arDetectormpl;
        
        /// <summary>
        /// インターフェースをセット
        /// </summary>
        /// <param name="impl">実際の機能の中身</param>
        public void SetARDetectorImpl(IStylyServiceARDetectorImpl impl)
        {
            this.arDetectormpl = impl;
        }

        /// <summary>
        /// Immersalによる位置合わせを行う
        /// </summary>
        /// <param name="localizeInterval">ローカライズのインターバル</param>
        /// <param name="burstMode">バーストモードを利用するかどうか</param>
        /// <param name="mapFile">Immersalのマップ作成アプリで作成したbytes形式のファイル</param>
        /// <param name="onFinished">終了イベント</param>
        public void ImmersalDetect(float localizeInterval, bool burstMode, TextAsset mapFile,
            Action<Exception> onFinished)
        {
            if (arDetectormpl != null)
            {
                arDetectormpl.ImmersalDetect(localizeInterval, burstMode, mapFile, onFinished);
            }
            else
            {
                //未実装ならエラー通知
                Debug.Log("ImmersalDetect called with map file: <" + mapFile?.name + ">");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }
    }
}
