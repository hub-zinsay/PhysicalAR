using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// 物体をドラッグ可能にするAttribute。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("STYLY/STYLY_Attr_Draggable")]
    public class STYLY_Attr_Draggable : FeatureComponentBase, IDraggable
    {
        [SerializeField, FormerlySerializedAs("OnBeginDragging")]
        UnityEvent onBeginDragging;
        [SerializeField, FormerlySerializedAs("OnEndDragging")]
        UnityEvent onEndDragging;
        
        public UnityEvent OnBeginDragging { get => onBeginDragging; set => onBeginDragging = value; }
        public UnityEvent OnEndDragging { get => onEndDragging; set => onEndDragging = value; }
    }
}