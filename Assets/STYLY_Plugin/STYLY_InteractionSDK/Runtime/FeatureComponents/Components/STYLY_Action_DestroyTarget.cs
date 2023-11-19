using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// 指定した物体を破壊するAction。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("STYLY/STYLY_Action_DestroyTarget")]
    public class STYLY_Action_DestroyTarget : FeatureComponentBase, IDestroyTarget
    {
        [SerializeField]
        public GameObject target;
        public GameObject Target { get => target; set => target = value; }
        public UnityAction DestroyTargetListener { get; set; }
        
        public void DestroyTarget()
        {
            if( DestroyTargetListener!=null){ DestroyTargetListener.Invoke();}
        }

    }
}

