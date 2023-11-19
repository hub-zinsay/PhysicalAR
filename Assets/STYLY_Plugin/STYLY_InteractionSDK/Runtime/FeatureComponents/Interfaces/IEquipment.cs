using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponent EquipmentのInterface。
    /// </summary>
    public interface IEquipment
    {
        UnityEvent OnUse { get; set; }
        UnityEvent OnUseEnd { get; set; }
        UnityEvent OnEquip { get; set; }
        UnityEvent OnDrop { get; set; }
    }
}