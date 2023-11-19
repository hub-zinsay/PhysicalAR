using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponent SpawnerのInterface。
    /// </summary>
    public interface ISpawner
    {
        GameObject Prefab { get; set; }
        Vector3 SpawnPosition { get; set; }
        Transform SpawnTransform { get; set; }
        float Velocity { get; set; }
        UnityAction SpawnListener { get; set; }
    }
}