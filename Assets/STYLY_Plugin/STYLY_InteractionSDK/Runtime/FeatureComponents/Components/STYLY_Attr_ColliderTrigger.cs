using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// 物体の侵入を検知するAttribute。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("STYLY/STYLY_Attr_ColliderTrigger")]
    public class STYLY_Attr_ColliderTrigger : FeatureComponentBase, IColliderTrigger
    {
        [SerializeField, FormerlySerializedAs("OnEnter")]
        UnityEvent onEnter;

        [SerializeField, FormerlySerializedAs("OnExit")]
        UnityEvent onExit;

        public UnityEvent OnEnter { get => onEnter; set => onEnter = value; }
        public UnityEvent OnExit { get => onExit; set => onExit = value; }

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }
    }
}
