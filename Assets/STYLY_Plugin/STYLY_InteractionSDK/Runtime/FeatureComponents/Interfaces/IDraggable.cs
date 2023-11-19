using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponent DraggableのInterface。
    /// </summary>
    public interface IDraggable
    {
        UnityEvent OnBeginDragging { get; set; }
        UnityEvent OnEndDragging { get; set; }
    }
}