using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponent MoverのInterface。
    /// </summary>
    public interface IMover
    {
        Transform Target { get; set; }
        float Time { get; set; }
        void Move();
        UnityAction StartMoveListener { get; set; }
        UnityEvent OnComplete { get; set; }
    }
}
