#if PLAYMAKER

using STYLY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.STYLY
{ 
    [ActionCategory("STYLY")]
    [Tooltip("ARカメラの映像をテクスチャとして取得する")]
    public class GetARCameraImage : FsmStateAction
    {
        [ActionSection("Results")]
        public FsmTexture StoreValue;

        public override void OnEnter()
        {
            StylyServiceForPlayMaker.Instance.GetARCameraImage(texture => 
            {
                if(texture == null)
                {
                    StoreValue.Value = null;
                    Finish();
                }

                StoreValue.Value = texture;
                Finish();
            });
        }

        public override void Reset()
        {
            base.Reset();
            StoreValue = null;
        }
    }
}

#endif