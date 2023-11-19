#if PLAYMAKER

using System;
using UnityEngine;
using STYLY;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// PlayMaker custom action to stop video with video player
    /// 
    /// If it succeed FINISHED event will be sent.
    /// ErrorEvent will be sent if there's an error.
    /// also ErrorString will be set if an error occurs.
    /// </summary>
    [ActionCategory("STYLY")]
    [Tooltip("Set volum of video. The video player must be initialized beforehand.")]
    public class VideoSetVolume : FsmStateAction
    {
        [Tooltip("Screen GameObject")]
        public FsmOwnerDefault screenObject;

        [Tooltip("Audio volume.")]
        public FsmFloat volume = 1.0f;

        [Tooltip("Event to send if there's an error initializing video")]
        public FsmEvent errorEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Error message if there's an error initializing video")]
        public FsmString errorString;

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(screenObject);
            if (go == null)
            {
                Finish();
                return;
            }
            StylyServiceForPlayMaker.Instance.VideoSetVolume(go, volume.Value, error =>
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
            errorEvent = null;
            errorString = null;
            volume = 1.0f;
        }
    }
}
#endif
