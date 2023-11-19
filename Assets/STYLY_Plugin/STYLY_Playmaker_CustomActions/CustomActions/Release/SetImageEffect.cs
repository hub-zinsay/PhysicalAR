#if PLAYMAKER

using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using STYLY;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    [ActionCategory("STYLY")]
    [Tooltip("Set the image effect to the camera except one for UI.")]
    [HelpUrl("https://styly.cc/ja/tips/set-image-effect-custom-action/")]
    public class SetImageEffect : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("ImageEffect用シェーダーを使用したマテリアル")]
        public Material material;

        [Tooltip("Event to send if there's an error before scene transition.")]
        public FsmEvent errorEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Error message if there's an error before scene transition.")]
        public FsmString errorString;

        public override void OnEnter()
        {
            if (material == null)
            {
                Finish();
                return;
            }

            StylyServiceForPlayMaker.Instance.SetImageEffect(material, Owner, error =>
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
            errorEvent = null;
            errorString = null;
            material = null;
        }
    }
}
#endif