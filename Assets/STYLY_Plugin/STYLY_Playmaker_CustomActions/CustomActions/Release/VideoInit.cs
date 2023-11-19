#if PLAYMAKER

using UnityEngine;
using STYLY;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// PlayMaker custom action to init video player
    /// 
    /// If it succeed FINISHED event will be sent.
    /// ErrorEvent will be sent if there's an error.
    /// also ErrorString will be set if an error occurs.
    /// </summary>
    [ActionCategory("STYLY")]
    [Tooltip("Initialize Video Player")]
    public class VideoInit : FsmStateAction
    {
        [Tooltip("Screen GameObject")]
        public FsmOwnerDefault screenObject;

        [Tooltip("URL of the video.")]
        public FsmString videoURL;

        [Tooltip("Auto play after the video is initialized.")]
        public FsmBool autoPlay = true;

        [Tooltip("Loop play or not.")]
        public FsmBool loop = true;

        [Tooltip("Audio volume.")]
        public FsmFloat volume = 1.0f;

        [Tooltip("Caching method.\n- Never: don't use cache, use streaming.\n- Auto: cache if available, start playing immediately.\n- Always: play after download the video.\n(on WebGL player, cache type will be ignored and always use streaming)")]
        [ObjectType(typeof(VideoCacheType))]
        public FsmEnum cacheType = VideoCacheType.Auto;

        [Tooltip("Event to send if there's an error initializing video")]
        public FsmEvent errorEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Error message if there's an error initializing video")]
        public FsmString errorString;
        
        /// <summary>
        /// プロキシを使用するかどうか
        /// Editor拡張でデフォルトでは表示されないように工夫する
        /// </summary>
        [HideInInspector]
        public FsmBool useProxyOnWebGL = true;

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(screenObject);
            if (string.IsNullOrEmpty(videoURL.Value) || go == null)
            {
                Finish();
                return;
            }
            // ビデオプレイヤー設定
            var videoParams = new VideoParams()
            {
                uri = videoURL.Value,
                autoPlay = autoPlay.Value,
                loop = loop.Value,
                cacheType = (VideoCacheType)cacheType.Value,
                volume = volume.Value,
                useProxyOnWebGL = useProxyOnWebGL.Value
            };
            StylyServiceForPlayMaker.Instance.VideoInit(go, videoParams, error =>
            {
                if (error != null)
                {
                    if (errorString != null)
                    {
                        errorString.Value = error.Message;
                    }
                    Fsm.Event(errorEvent);
                    return;
                }
                Finish();
            });
        }

        public override void Reset()
        {
            base.Reset();
            screenObject = null;
            videoURL = null;
            errorEvent = null;
            errorString = null;
            autoPlay = true;
            loop = true;
            useProxyOnWebGL = true;
            volume = 1.0f;
            cacheType.Value = VideoCacheType.Auto;
        }
    }
}
#endif