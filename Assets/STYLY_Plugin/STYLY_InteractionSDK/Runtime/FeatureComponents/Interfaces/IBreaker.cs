using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponent BreakerのInterface。
    /// </summary>
    public interface IBreaker
    {
        UnityEvent OnDestroyEvent { get; set; }
    }
}