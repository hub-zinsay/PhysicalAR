using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// 指定したPrefabをインスタンス化するAction。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("STYLY/STYLY_Action_Spawner")]
    public class STYLY_Action_Spawner : FeatureComponentBase, ISpawner
    {
        [SerializeField]
        GameObject prefab;

        [SerializeField]
        Vector3 spawnPosition;

        [SerializeField]
        Transform spawnTransform;

        [SerializeField]
        float velocity;

        public GameObject Prefab { get => prefab; set => prefab = value; }
        public Vector3 SpawnPosition { get => spawnPosition; set=> spawnPosition = value; }
        public Transform SpawnTransform { get => spawnTransform; set => spawnTransform = value; }
        public float Velocity { get => velocity; set=> velocity = value; }
        public UnityAction SpawnListener { get; set; }
        
        public void Spawn()
        {
            if(SpawnListener!=null){ SpawnListener.Invoke();}
        }

    }
}
