using System;
using System.Collections.Generic;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// video part of StylyServiceForPlayMaker (partial class)
    /// </summary>
    public partial class StylyServiceForPlayMaker
    {
        private IStylyServiceVideoImpl videoImpl;

        public void SetVideoImpl(IStylyServiceVideoImpl impl)
        {
            this.videoImpl = impl;
        }

        /// <summary>
        /// initialize video player.
        /// </summary>
        /// <param name="targetObj">ビデオプレイヤーが追加されるGameObject。スクリーンとなるメッシュを保持している必要がある。</param>
        /// <param name="videoParams"></param>
        /// <param name="onFinished"></param>
        public void VideoInit(GameObject targetObj, VideoParams videoParams, Action<Exception> onFinished)
        {
            GetVideoImplOrError("VideoInit", onFinished)?.VideoInit(targetObj, videoParams, onFinished);
        }

        /// <summary>
        /// play video with video player
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="onFinished"></param>
        public void VideoPlay(GameObject targetObj, Action<Exception> onFinished)
        {
            GetVideoImplOrError("VideoPlay", onFinished)?.VideoPlay(targetObj, onFinished);
        }

        /// <summary>
        /// stop playing video with video player
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="onFinished"></param>
        public void VideoStop(GameObject targetObj, Action<Exception> onFinished)
        {
            GetVideoImplOrError("VideoStop", onFinished)?.VideoStop(targetObj, onFinished);
        }

        /// <summary>
        /// pause playing video with video player
        /// it can be resumed by calling VideoPlay().
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="onFinished"></param>
        public void VideoPause(GameObject targetObj, Action<Exception> onFinished)
        {
            GetVideoImplOrError("VideoPause", onFinished)?.VideoPause(targetObj, onFinished);
        }

        /// <summary>
        /// setting audio volume of video player
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="volume">ボリューム値 (0～1)</param>
        /// <param name="onFinished"></param>
        public void VideoSetVolume(GameObject targetObj, float volume, Action<Exception> onFinished)
        {
            GetVideoImplOrError("VideoSetVolume", onFinished)?.VideoSetVolume(targetObj, volume, onFinished);
        }

        /// <summary>
        /// returns the total play time of the video in seconds
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <returns></returns>
        public void VideoGetDuration(GameObject targetObj, Action<float, Exception> onFinished) 
        {
            GetVideoImplOrError("VideoGetDuration", (ex) =>  onFinished(0f, ex))?.VideoGetDuration(targetObj, onFinished);
        }

        /// <summary>
		/// returns the current video time in seconds
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <returns></returns>
        public void VideoGetCurrentTime(GameObject targetObj, Action<float, Exception> onFinished) 
        {
            GetVideoImplOrError("VideoGetCurrentTime", (ex) => onFinished(0f, ex))?.VideoGetCurrentTime(targetObj, onFinished);
        }

        /// <summary>
        /// returns whether the video is playing or not
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObjec</param>
        /// <returns></returns>
        public void VideoIsPlaying(GameObject targetObj, Action<bool, Exception> onFinished) 
        {
            GetVideoImplOrError("VideoIsPlaying", (ex) => onFinished(false, ex))?.VideoIsPlaying(targetObj, onFinished);
        }

        /// <summary>
        /// seek the video at specified time
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObjec</param>
        /// <param name="time">指定時間</param>
        public void VideoSeek(GameObject targetObj, float time, Action<Exception> onFinished) 
        {
            GetVideoImplOrError("VideoSeek", onFinished)?.VideoSeek(targetObj, time, onFinished);
        }

        /// <summary>
        /// videoのimplがあればそれを返却し、ない場合はonFinishedをエラー引数で呼んでnullを返す便利メソッド
        /// </summary>
        /// <param name="actionName">ログ表示用アクション名</param>
        /// <param name="onFinished"></param>
        /// <returns>IStylyServiceVideoImplの実装、またはなければnull</returns>
        private IStylyServiceVideoImpl GetVideoImplOrError(string actionName, Action<Exception> onFinished)
        {
            // アップロード環境向けに、Unity Editor環境でimplが入っていなければダミーimplをセットする。
            if (videoImpl == null && Application.isEditor)
            {
                SetVideoImpl(new DummyVideoImpl());
            }
            if (videoImpl != null)
            {
                return videoImpl;
            }
            else
            {
                var msg = $"<{actionName}> called, but the IStylyServiceVideoImpl implementation is not available.";
                Debug.LogError(msg);
                onFinished(new Exception(msg));
                return null;
            }
        }
    }
}
