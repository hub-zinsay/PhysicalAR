#if PLAYMAKER

using System;
using UnityEngine;
using STYLY;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// PlayMaker custom action to get length of the video clip.
    /// 
    /// If it succeed FINISHED event will be sent.
    /// ErrorEvent will be sent if there's an error.
    /// also ErrorString will be set if an error occurs.
    /// </summary>
    [ActionCategory("STYLY")]
    [Tooltip("Get length of the video clip. The video player must be initialized beforehand.")]
    public class VideoGetDuration : FsmStateAction
    {
        [Tooltip("Screen GameObject")]
        public FsmOwnerDefault screenObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("The value of the video duration.")]
        public FsmFloat duration;

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

            StylyServiceForPlayMaker.Instance.VideoGetDuration(go, (val, error) =>
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

                duration.Value = val;
                Finish();
            });
        }

        public override void Reset()
        {
            base.Reset();
            screenObject = null;
            duration = null;
            errorEvent = null;
            errorString = null;
        }
    }
}

#endif