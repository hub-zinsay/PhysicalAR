#if PLAYMAKER

using STYLY;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    [ActionCategory("STYLY")]
    [Tooltip("Tests if Runtime is STYLY mobile.")]
    public class IsAppTypeMobile : FsmStateAction
    {
        public FsmEvent trueEvent;

        public FsmEvent falseEvent;

        [UIHint(UIHint.Variable)]
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
            return StylyServiceForPlayMaker.Instance.GetAppType() == StylyAppType.Mobile;
        }
    }
}

#endif