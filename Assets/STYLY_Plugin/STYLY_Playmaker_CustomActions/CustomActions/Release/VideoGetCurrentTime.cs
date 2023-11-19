#if PLAYMAKER

using System;
using UnityEngine;
using STYLY;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// PlayMaker custom action to get the current playback time of the video.
    /// 
    /// If it succeed FINISHED event will be sent.
    /// ErrorEvent will be sent if there's an error.
    /// also ErrorString will be set if an error occurs.
    /// </summary>
    [ActionCategory("STYLY")]
    [Tooltip("Get the current playback time of the video. The video player must be initialized beforehand.")]
    public class VideoGetCurrentTime : FsmStateAction
    {
        [Tooltip("Screen GameObject")]
        public FsmOwnerDefault screenObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("The value of the current playback time.")]
        public FsmFloat currentTime;

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

            StylyServiceForPlayMaker.Instance.VideoGetCurrentTime(go, (val, error) =>
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

                currentTime.Value = val;
                Finish();
            });
        }

        public override void Reset()
        {
            base.Reset();
            screenObject = null;
            currentTime = null;
            errorEvent = null;
            errorString = null;
        }
    }
}

#endif