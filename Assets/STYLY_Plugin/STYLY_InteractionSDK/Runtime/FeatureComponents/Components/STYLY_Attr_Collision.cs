using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// 物体との衝突を検知するAttribute。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("STYLY/STYLY_Attr_Collision")]
    public class STYLY_Attr_Collision : FeatureComponentBase, ICollision
    {
        [SerializeField, FormerlySerializedAs("OnCollisionEnterEvent")]
        UnityEvent onCollisionEnterEvent;

        [SerializeField, FormerlySerializedAs("OnCollisionExitEvent")]
        UnityEvent onCollisionExitEvent;
        public UnityEvent OnCollisionEnterEvent { get => onCollisionEnterEvent; set => onCollisionEnterEvent = value; }
        public UnityEvent OnCollisionExitEvent { get => onCollisionExitEvent; set => onCollisionExitEvent = value; }
    }
}
