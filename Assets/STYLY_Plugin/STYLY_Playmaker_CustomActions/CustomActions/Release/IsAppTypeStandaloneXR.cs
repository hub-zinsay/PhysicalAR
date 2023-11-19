#if PLAYMAKER

using STYLY;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
namespace HutongGames.PlayMaker.Actions.STYLY
{
    [ActionCategory("STYLY")]
    [Tooltip("Check if STYLY app type is StandaloneXR (like Meta Quest. XR devices which does not require PC)")]
    [HelpUrl("Check if STYLY app type is StandaloneXR (like Meta Quest. XR devices which does not require PC)")]
    public class IsAppTypeStandaloneXR : FsmStateAction
    {
        [Tooltip("Event to send if the Bool variable is True.")]
        public FsmEvent trueEvent;

        [Tooltip("Event to send if the Bool variable is False.")]
        public FsmEvent falseEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a bool variable.")]
        public FsmBool store;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset()
        {
            store = null;
            trueEvent = null;
            falseEvent = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            Do();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            Do();
        }

        private void Do()
        {
            var result = IsAppTypeCorrect();
            if (store != null)
            {
                store.Value = result;
            }

            Fsm.Event(result ? trueEvent : falseEvent);
        }

        private bool IsAppTypeCorrect()
        {
            return StylyServiceForPlayMaker.Instance.GetAppType() == StylyAppType.StandaloneXR;
        }
    }
}
// ReSharper restore MemberCanBePrivate.Global

#endif
