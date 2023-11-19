#if PLAYMAKER

using System.Runtime.InteropServices;
using UnityEngine;
using STYLY;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    [ActionCategory("STYLY")]
    [Note("Open a web page in the web browser.\nFor WebGL platform, please invoke this action at the time of PointerDown event. Use 'UI POINTER DOWN' event in FSM or use EventTrigger component's PointerDown handler. Otherwise it will need one more click to open the url.")]
    [Tooltip("Open a web page in the web browser.\nFor WebGL platform, Please invoke this action at the time of PointerDown event. Use 'UI POINTER DOWN' event in FSM or use EventTrigger component's PointerDown handler. Otherwise it will need one more click to open the url.")]
    [HelpUrl("https://styly.cc/ja/tips/playmaker-link-styly/")]
    public class ApplicationOpenUrl : FsmStateAction
    {
        [RequiredField]
        public FsmString url;

        [Tooltip("アプリ内で開かず、標準ブラウザの起動を明示します。")]
        public FsmBool directOpen;

        [Tooltip("Event to send if there's an error before open url.")]
        public FsmEvent errorEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Error message if there's an error before open url.")]
        public FsmString errorString;

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(url.Value))
            {
                Finish();
                return;
            }
            StylyServiceForPlayMaker.Instance.OpenURL(url.Value, directOpen.Value, error =>
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
            Finish();
        }

        public override void Reset()
        {
            url = "";
            directOpen = false;
            errorEvent = null;
            errorString = null;
        }
    }
}

#endif