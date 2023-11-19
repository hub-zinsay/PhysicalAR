using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEngine;
using UnityEngine.UI;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// Spawnerダミー実装のImplSetupper。
    /// </summary>
    public class DummySpawnerSetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var ifComp = srcComponent as ISpawner;
            var impl = srcComponent.gameObject.AddComponent<DummySpawner>();
            impl.prefab = ifComp.Prefab;
            impl.spawnPosition = ifComp.SpawnPosition;
            impl.spawnTransform = ifComp.SpawnTransform;
            impl.velocity = ifComp.Velocity;
            ifComp.SpawnListener += impl.Spawn;
        }
    }
    
    /// <summary>
    /// Spawnerダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummySpawner : MonoBehaviour
    {
        public GameObject prefab;

        public Vector3 spawnPosition;

        public Transform spawnTransform;

        public float velocity;

        public void Spawn()
        {
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            if(spawnTransform != null )
            {
                position = spawnTransform.position;
                rotation = spawnTransform.rotation;
            }
            else
            {
                position = transform.position +
                    transform.right * spawnPosition.x +
                    transform.up * spawnPosition.y +
                    transform.forward * spawnPosition.z;

                rotation = transform.rotation;
            }

            var spawnedGO = GameObject.Instantiate(prefab, position, rotation);
            spawnedGO.transform.localScale = prefab.transform.lossyScale;
            spawnedGO.SetActive(true);
            spawnedGO.name = prefab.name;

            var rigidBody = spawnedGO.GetComponent<Rigidbody>();
            if (rigidBody)
            {
                if (spawnTransform != null)
                {
                    rigidBody.velocity = spawnTransform.forward * velocity;
                }
                else
                {
                    rigidBody.velocity = transform.forward * velocity;
                }
            }
        }
    }
}
