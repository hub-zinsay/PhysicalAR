using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// 指定した物体を移動するAction。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("STYLY/STYLY_Action_Mover")]
    public sealed class STYLY_Action_Mover : FeatureComponentBase, IMover
    {

        [SerializeField] private Transform target;
        public Transform Target { get => target; set => target = value; }

        [SerializeField, Tooltip("Sec")]
        private float time;
        public float Time { get => time; set => time = value; }

        public UnityAction StartMoveListener { get; set; }
        public UnityEvent OnComplete { get => onComplete; set => onComplete = value; }

        [SerializeField, FormerlySerializedAs("OnComplete")]
        private UnityEvent onComplete;

        public void Move()
        {
            if( StartMoveListener!=null){ StartMoveListener.Invoke();}
        }

    }
}
