using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// 物体を破壊可能とするAttribute。
    /// STYLY_Attr_Breakerが貼られたGameObjectとの衝突により破壊される。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("STYLY/STYLY_Attr_Breakable")]
    public class STYLY_Attr_Breakable : FeatureComponentBase, IBreakable    {

        [SerializeField]
        UnityEvent onDestroyEvent;
        
        public UnityEvent OnDestroyEvent { get => onDestroyEvent; set => onDestroyEvent = value; }
    }
}