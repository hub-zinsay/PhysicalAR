using System;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEngine;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// ダミー実装のImplSetupperProvider。
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class DummyImplSetupperProvider : MonoBehaviour
    {
        private Dictionary<Type, IImplSetupper> interfaceBindDictionary = new Dictionary<Type, IImplSetupper>()
        {
            {typeof(STYLY_Attr_Collision), new DummyCollisionSetupper()},
            {typeof(STYLY_Attr_Breaker), new DummyBreakerSetupper()},
            {typeof(STYLY_Attr_Breakable), new DummyBreakableSetupper()},
            {typeof(STYLY_Attr_Draggable), new DummyDraggableSetupper()},
            {typeof(STYLY_Attr_Equipment), new DummyEquipmentetupper()},
            {typeof(STYLY_Action_Mover), new DummyMoverSetupper()},
            {typeof(STYLY_Action_Spawner), new DummySpawnerSetupper()},
            {typeof(STYLY_Action_Timer), new DummyTimerSetupper()},
            {typeof(STYLY_Attr_ColliderTrigger), new DummyColliderTriggerSetupper()},
            {typeof(STYLY_Action_DestroyTarget), new DummyDestroyTargetSetupper()},
        };

        private void Awake()
        {
            ImplBinder.Instance.Init(GetSetupper);
        }
        
        IImplSetupper GetSetupper(Type type)
        {
            IImplSetupper setupper;
            if (interfaceBindDictionary.TryGetValue(type, out setupper))
            {
                return setupper;
            }
            else
            {
                return null;
            }
        }

    }
}