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
    [Tooltip("Set the projective texture mapping using the camera image.")]
    public class SetProjectionTextureMapping : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("ProjectionTextureMapping用を適用したいマテリアル")]
        public Material material;

        public FsmOwnerDefault gameObject;

        [Tooltip("Event to send if there's an error before scene transition.")]
        public FsmEvent errorEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Error message if there's an error before scene transition.")]
        public FsmString errorString;

        public override void OnEnter()
        {
            if (material == null || gameObject == null)
            {
                Finish();
                return;
            }

            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            StylyServiceForPlayMaker.Instance.SetProjectionTextureMapping(material, go, error =>
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
            gameObject = null;
        }
    }
}
#endif