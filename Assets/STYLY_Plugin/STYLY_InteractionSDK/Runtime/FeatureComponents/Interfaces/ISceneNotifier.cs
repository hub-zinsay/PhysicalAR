using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponent SceneNotifierのInterface。
    /// </summary>
    public interface ISceneNotifier
    {
        UnityEvent OnStart { get; set; }
        UnityEvent OnMenuOpen { get; set; }
        UnityEvent OnMenuClose { get; set; }
        UnityEvent OnExit { get; set; }
    }
}