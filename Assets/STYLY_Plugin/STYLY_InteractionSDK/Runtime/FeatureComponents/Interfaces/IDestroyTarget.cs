using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponent DestroyTargetのInterface。
    /// </summary>
    public interface IDestroyTarget
    { 
        GameObject Target { get; set; }
        UnityAction DestroyTargetListener { get; set; }
    }
}