using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// 物体を破壊するAttribute。
    /// STYLY_Attr_Breakableが貼られたGameObjectを破壊する事が出来る。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("STYLY/STYLY_Attr_Breaker")]
    public class STYLY_Attr_Breaker : FeatureComponentBase, IBreaker
    {
        [SerializeField, FormerlySerializedAs("OnDestroyEvent")]
        UnityEvent onDestroyEvent;
        public UnityEvent OnDestroyEvent { get => onDestroyEvent; set => onDestroyEvent = value; }
    }
}
