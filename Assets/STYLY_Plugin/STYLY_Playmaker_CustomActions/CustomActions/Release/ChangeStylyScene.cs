#if PLAYMAKER

using System;
using UnityEngine;
using STYLY;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// PlayMaker custom action to change styly scene.
    /// 
    /// The destination scene should be specified via sceneId property.
    /// If it succeed, scene change will occur, and FINISHED event will be sent just before the scene transition.
    /// ErrorEvent will be sent if there's an error before going to the scene,
    /// and also ErrorString will be set if it occurs.
    /// </summary>
    [ActionCategory("STYLY")]
    [Tooltip("Change Styly Scene")]
    [HelpUrl("https://styly.cc/manual/customaction-kotauchisunsun-change-styly-scene/")]
    public class ChangeStylyScene : FsmStateAction
    {
        [Tooltip("Styly Scene Id. Needs to be GUID format, like 1234abc-1234-abcd-ef12-012345abcdef")]
        public FsmString sceneId;

        [Tooltip("Event to send if there's an error before scene transition.")]
        public FsmEvent errorEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Error message if there's an error before scene transition.")]
        public FsmString errorString;

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(sceneId.Value))
            {
                Finish();
                return;
            }
            StylyServiceForPlayMaker.Instance.ChangeStylyScene(sceneId.Value, error =>
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
            sceneId = null;
            errorEvent = null;
            errorString = null;
        } 
    }
}
#endif
