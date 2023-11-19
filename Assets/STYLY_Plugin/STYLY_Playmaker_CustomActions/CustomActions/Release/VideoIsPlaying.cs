#if PLAYMAKER

using System;
using UnityEngine;
using STYLY;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// PlayMaker custom action to get whether the video is playing or not.
    /// 
    /// If it succeed FINISHED event will be sent.
    /// ErrorEvent will be sent if there's an error.
    /// also ErrorString will be set if an error occurs.
    /// </summary>
    [ActionCategory("STYLY")]
    [Tooltip("Get whether the video is playing or not. The video player must be initialized beforehand.")]
    public class VideoIsPlaying : FsmStateAction
    {
        [Tooltip("Screen GameObject")]
        public FsmOwnerDefault screenObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Whether the video is being played.")]
        public FsmBool isPlaying;

        [Tooltip("Event to send error if there is an error in the video")]
        public FsmEvent errorEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Error message if there is an error in the video")]
        public FsmString errorString;

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(screenObject);
            if (go == null)
            {
                Finish();
                return;
            }

            StylyServiceForPlayMaker.Instance.VideoIsPlaying(go, (val, error) =>
            {
                if (error != null)
                {
                    if (errorString != null)
                    {
                        errorString.Value = error.Message;
                    }
                    Fsm.Event(errorEvent);
                    Finish();
                    
                    return;
                }

                isPlaying.Value = val;
                Finish();
            });
        }

        public override void Reset()
        {
            base.Reset();
            screenObject = null;
            isPlaying = null;
            errorEvent = null;
            errorString = null;
        }
    }
}

#endif