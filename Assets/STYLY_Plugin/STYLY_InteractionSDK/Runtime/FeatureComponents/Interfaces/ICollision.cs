using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponent ColliderTriggerのInterface。
    /// </summary>
    public interface ICollision
    {
        UnityEvent OnCollisionEnterEvent { get; set; }

        UnityEvent OnCollisionExitEvent { get; set; }
    }
}