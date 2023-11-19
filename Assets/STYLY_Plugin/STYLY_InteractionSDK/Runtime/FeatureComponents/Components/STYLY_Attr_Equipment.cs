using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// 物体をコントローラーに装備可能にするAttribute。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("STYLY/STYLY_Attr_Equipment")]
    public class STYLY_Attr_Equipment : FeatureComponentBase, IEquipment
    {
        [SerializeField, FormerlySerializedAs("OnUse")]
        UnityEvent onUse;

        [SerializeField, FormerlySerializedAs("OnUseEnd")]
        UnityEvent onUseEnd;

        [SerializeField, FormerlySerializedAs("OnEquip")]
        UnityEvent onEquip;

        [SerializeField, FormerlySerializedAs("OnDrop")]
        UnityEvent onDrop;

        public UnityEvent OnUse { get => onUse; set=> onUse = value; }
        public UnityEvent OnUseEnd { get => onUseEnd; set => onUseEnd = value; }
        public UnityEvent OnEquip { get => onEquip; set => onEquip = value; }
        public UnityEvent OnDrop { get => onDrop; set => onDrop = value; }
    }
}
