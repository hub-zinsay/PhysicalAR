#if PLAYMAKER

using System;
using UnityEngine;
using STYLY;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// PlayMaker custom action to seek video to a specified time.
    /// 
    /// If it succeed FINISHED event will be sent.
    /// ErrorEvent will be sent if there's an error.
    /// also ErrorString will be set if an error occurs.
    /// </summary>
    [ActionCategory("STYLY")]
    [Tooltip("Seek video to a specified time. The video player must be initialized beforehand.")]
    public class VideoSeek : FsmStateAction
    {
        [RequiredField]
        public FsmFloat time;

        [Tooltip("Screen GameObject")]
        public FsmOwnerDefault screenObject;

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

            StylyServiceForPlayMaker.Instance.VideoSeek(go, time.Value, (error) =>
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

                Finish();
            });
        }

        public override void Reset()
        {
            base.Reset();
            time = null;
            screenObject = null;
            errorEvent = null;
            errorString = null;
        }
    }
}

#endif