using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// Breakableダミー実装のImplSetupper。
    /// </summary>
    public class DummyBreakableSetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var component = srcComponent as Component;
            var ifComp = srcComponent as IBreakable;
            var impl = component.gameObject.AddComponent<DummyBreakable>();
            impl.OnDestroyEvent = ifComp.OnDestroyEvent;
        }
    }

    /// <summary>
    /// Breakableダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummyBreakable : MonoBehaviour
    {
        /// <summary>
        /// 破棄済みプロパティ
        /// Breakerは破棄した場合にこの値をtrueにセットする。
        /// また、既に破棄済みの場合にはOnDestroyEventを呼び出さない事
        /// </summary>
        public bool IsBroken { get; set; } = false;
        public UnityEvent OnDestroyEvent;
    }
}