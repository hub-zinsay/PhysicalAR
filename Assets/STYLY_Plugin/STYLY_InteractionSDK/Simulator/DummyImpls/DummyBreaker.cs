using System;
using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// Breakerダミー実装のImplSetupper。
    /// </summary>
    public class DummyBreakerSetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var ifComp = srcComponent as IBreaker;
            var impl = srcComponent.gameObject.AddComponent<DummyBreaker>();
            impl.OnDestroyEvent = ifComp.OnDestroyEvent;
        }
    }
    
    /// <summary>
    /// Breakerダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummyBreaker : MonoBehaviour
    {
        public UnityEvent OnDestroyEvent;

        private void OnCollisionEnter(Collision collision)
        {
            Breake(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            Breake(other.gameObject);
        }

        void Breake(GameObject breakableGO)
        {
            var breakable = breakableGO.GetComponent<DummyBreakable>();
            if (breakable)
            {
                // 破棄済みの場合は処理しない。
                if (breakable.IsBroken)
                {
                    return;
                }
                breakable.IsBroken = true;
                breakable.OnDestroyEvent.Invoke();
                Destroy(breakableGO);
                if( OnDestroyEvent!=null){ OnDestroyEvent.Invoke();}
            }
        }
    }
}