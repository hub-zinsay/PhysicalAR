#if PLAYMAKER

using STYLY;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// GLTFをランタイムロード
    /// </summary>
    [ActionCategory("STYLY")]
    public class GLTFLoad : FsmStateAction
    {
        /// <summary>
        /// 生成したオブジェクトの親となるオブジェクト
        /// </summary>
        public FsmOwnerDefault parentObject;

        [Tooltip("URL of the load source")] 
        public FsmString sourceUrl;

        [Tooltip("When load completed whether to active")]
        public FsmBool activeOnStart;

        [Tooltip("Whether to generate collider")]
        public FsmBool generateCollider;

        [Tooltip("Event to send if there's an error on loading.")]
        public FsmEvent errorEvent;

        [UIHint(UIHint.Variable)] 
        [Tooltip("Error message if there's an error on loading.")]
        public FsmString errorString;

        [Tooltip("Simulate an situation of error on Unity")]
        public bool dummyErrorOnUnity;

        [ActionSection("Results")] 
        [UIHint(UIHint.Variable)] 
        [Tooltip("Result on loading glb")]
        public FsmGameObject storeGameObject;

        public override void OnEnter()
        {
            if (parentObject == null) return;
            var parent = Fsm.GetOwnerDefaultTarget(parentObject);
            
            if (Application.isEditor)
            {
                //エラーをシミュレートする
                if (dummyErrorOnUnity)
                {
                    if (errorString != null)
                    {
                        errorString.Value = "Error simulation";
                    }

                    Fsm.Event(errorEvent);
                    Finish();
                    return;
                }
            }

            //ロード結果を取得。内部でロード完了を待機してからコールバック実行。
            StylyServiceForPlayMaker.Instance
                .GLTFLoad(parent, sourceUrl.Value, activeOnStart.Value, generateCollider.Value, (result, error) =>
                {
                    //エラー時の処理
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

                    if (result != null)
                    {
                        storeGameObject.Value = result;
                        Finish();
                    }
                });
        }

        public override void Reset()
        {
            base.Reset();
            sourceUrl = null;
            activeOnStart = true;
            generateCollider = false;
            errorEvent = null;
            errorString = null;
            storeGameObject = null;
            parentObject = null;
            dummyErrorOnUnity = false;
        }
    }
}
#endif