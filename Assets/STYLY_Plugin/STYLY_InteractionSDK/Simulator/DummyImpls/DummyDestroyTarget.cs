using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEngine;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// DestroyTargetダミー実装のImplSetupper。
    /// </summary>
    public class DummyDestroyTargetSetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var ifComp = srcComponent as IDestroyTarget;
            var impl = srcComponent.gameObject.AddComponent<DummyDestroyTarget>();
            impl.target = ifComp.Target;
            ifComp.DestroyTargetListener = impl.DestroyTarget;
        }
    }
    
    /// <summary>
    /// DestroyTargetダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummyDestroyTarget : MonoBehaviour
    {
        public GameObject target;

        public void DestroyTarget()
        {
            Debug.Log("DestoryTarget");
            Destroy(target);
        }
    }
}

