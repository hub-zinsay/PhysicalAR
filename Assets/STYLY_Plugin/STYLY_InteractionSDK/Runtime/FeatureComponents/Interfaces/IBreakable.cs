using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    // FeatureComponent BreakableのInterface。
    /// </summary>
    public interface IBreakable
    {
        UnityEvent OnDestroyEvent { get; set; }
    }
}