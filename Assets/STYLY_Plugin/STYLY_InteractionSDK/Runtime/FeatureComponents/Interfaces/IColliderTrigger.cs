using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponent ColliderTriggerのInterface。
    /// </summary>
    public interface IColliderTrigger
    {
        UnityEvent OnEnter { get; set; }

        UnityEvent OnExit { get; set; }

    }
}