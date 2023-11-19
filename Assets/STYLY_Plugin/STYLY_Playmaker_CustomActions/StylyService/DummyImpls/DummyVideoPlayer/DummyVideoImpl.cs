using System;
using System.Collections.Generic;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// DummyVideoPlayerを利用したStylyServiceVideoの実装。
    /// Unity Editorでアップロード前確認時に動くものとして利用する。
    /// このクラスは橋渡し程度。
    /// </summary>
    public class DummyVideoImpl : IStylyServiceVideoImpl
    {
        public void VideoInit(GameObject targetObj, VideoParams videoParams, Action<Exception> onFinished)
        {
            var player = targetObj.GetComponent<DummyVideoPlayer>();
            if (player == null)
            {
                player = targetObj.AddComponent<DummyVideoPlayer>();
            }
            player.Init(videoParams, onFinished);
        }

        public void VideoPlay(GameObject targetObj, Action<Exception> onFinished)
        {
            var player = GetDummyVideoPlayerOrError(targetObj, "Video Play", onFinished);
            if (player != null)
            {
                player.Play(onFinished);
            }
        }

        public void VideoStop(GameObject targetObj, Action<Exception> onFinished)
        {
            var player = GetDummyVideoPlayerOrError(targetObj, "Video Stop", onFinished);
            if (player != null)
            {
                player.Stop(onFinished);
            }
        }

        public void VideoPause(GameObject targetObj, Action<Exception> onFinished)
        {
            var player = GetDummyVideoPlayerOrError(targetObj, "Video Pause", onFinished);
            if (player != null)
            {
                player.Pause(onFinished);
            }
        }

        public void VideoSetVolume(GameObject targetObj, float volume, Action<Exception> onFinished)
        {
            var player = GetDummyVideoPlayerOrError(targetObj, "Video Play", onFinished);
            if (player != null)
            {
                player.SetVolume(volume, onFinished);
            }
        }

        /// <summary>
        /// DummyVideoPlayerからVideoの総再生時間を取得する。
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="onFinished"></param>
        public void VideoGetDuration(GameObject targetObj, Action<float, Exception> onFinished)
        {
            var player = GetDummyVideoPlayerOrError(targetObj, "Video Get Duration", (ex) => onFinished(0f, ex));
            if (player != null) 
            {
                player.GetDuration(onFinished);
            }
        }

        /// <summary>
        /// DummyVideoPlayerからVideoの現在再生時間を取得する。
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="onFinished"></param>
        public void VideoGetCurrentTime(GameObject targetObj, Action<float, Exception> onFinished)
        {
            var player = GetDummyVideoPlayerOrError(targetObj, "Video Get Current Time", (ex) => onFinished(0f, ex));
            if (player != null)
            {
                player.GetCurrentTime(onFinished);
            }
        }

        /// <summary>
        /// DummyVideoPlayerから現在再生中かどうかを取得する。
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="onFinished"></param>
        public void VideoIsPlaying(GameObject targetObj, Action<bool, Exception> onFinished)
        {
            var player = GetDummyVideoPlayerOrError(targetObj, "Video Is Playing", (ex) => onFinished(false, ex));
            if (player != null) 
            {
                player.IsPlaying(onFinished);
            }
        }

        /// <summary>
        /// Videoを指定時間までシークする。
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="time"></param>
        /// <param name="onFinished"></param>
        public void VideoSeek(GameObject targetObj, float time, Action<Exception> onFinished)
        {
            var player = GetDummyVideoPlayerOrError(targetObj, "Video Seek", onFinished);
            if (player != null)
            {
                player.Seek(time, onFinished);
            }
        }

        /// <summary>
        /// DummyVideoPlayerを取得し、なければエラー処理をする。
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="actionName"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        private DummyVideoPlayer GetDummyVideoPlayerOrError(GameObject targetObj, string actionName, Action<Exception> onFinished)
        {
            var player = targetObj.GetComponent<DummyVideoPlayer>();
            if (player == null)
            {
                var msg = $"{actionName}: No video player found. GameObject:{targetObj.name}";
                Debug.LogError(msg);
                onFinished?.Invoke(new Exception(msg));
                return null;
            }
            return player;
        }
    }
}
