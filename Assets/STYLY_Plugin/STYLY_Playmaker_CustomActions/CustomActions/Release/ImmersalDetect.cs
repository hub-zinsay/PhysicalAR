#if PLAYMAKER
using STYLY;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// PlayMaker custom action to detect by Immersal.
    /// </summary>
    [ActionCategory("STYLY")]
    [Tooltip("Execute detecting by Immersal")]
    public class ImmersalDetect : FsmStateAction
    {
        [Tooltip("Interval of Localize.")] 
        public FsmFloat localizeInterval;

        [Tooltip("Whether to use BurstMode.BurstMode will be higher the load, but the first detection will be better.")]
        public FsmBool burstMode;

        [Tooltip("Map file (bytes file).")] 
        public TextAsset mapFile;

        [Tooltip("Event to send when the first Localize is successful.")]
        public FsmEvent detectEvent;

        [Tooltip("Event to send if there's an error on Localize.")]
        public FsmEvent errorEvent;

        [UIHint(UIHint.Variable)] [Tooltip("Error message if there's an error on Localize.")]
        public FsmString errorString;
        
        public override void OnEnter()
        {
            StylyServiceForPlayMaker.Instance.ImmersalDetect(localizeInterval.Value, burstMode.Value,mapFile,error =>
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
                Fsm.Event(detectEvent);
            });
        }
        
        public override void Reset()
        {
            base.Reset();
            localizeInterval = 2.0f;
            burstMode = true;
            mapFile = null;
            detectEvent = null;
            errorEvent = null;
            errorString = null;
        } 
    }
}
#endif
