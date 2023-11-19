using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// Collisiionダミー実装のImplSetupper。
    /// </summary>
    public class DummyCollisionSetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var ifComp = srcComponent as ICollision;
            var impl = srcComponent.gameObject.AddComponent<DummyCollision>();
            impl.OnCollisionEnterEvent = ifComp.OnCollisionEnterEvent;
            impl.OnCollisionExitEvent = ifComp.OnCollisionExitEvent;
        }
    }

    /// <summary>
    /// Collisiionダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummyCollision : MonoBehaviour
    {
        public UnityEvent OnCollisionEnterEvent;

        public UnityEvent OnCollisionExitEvent;

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("OnCollisionEnter:" + collision.gameObject.name);
            OnCollisionEnterEvent.Invoke();
        }

        private void OnCollisionExit(Collision collision)
        {
            Debug.Log("OnCollisionExit:" + collision.gameObject.name);
            OnCollisionExitEvent.Invoke();
        }
    }

}
