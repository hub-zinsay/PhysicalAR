using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// ColliderTriggerダミー実装のImplSetupper。
    /// </summary>
    public class DummyColliderTriggerSetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var ifComp = srcComponent as IColliderTrigger;
            var impl = srcComponent.gameObject.AddComponent<DummyColliderTrigger>();
            impl.OnEnter = ifComp.OnEnter;
            impl.OnExit = ifComp.OnExit;
        }
    }
    
    /// <summary>
    /// ColliderTriggerダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummyColliderTrigger : MonoBehaviour
    {
        public UnityEvent OnEnter;
        public UnityEvent OnExit;
        
        private void OnTriggerEnter(Collider other)
        {
            OnEnter.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            OnExit.Invoke();
        }
    }

}
