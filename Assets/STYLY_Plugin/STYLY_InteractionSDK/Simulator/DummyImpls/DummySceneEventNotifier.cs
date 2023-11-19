using System.Collections;
using System.Collections.Generic;
using STYLY.Interaction.SDK.V1;
using UnityEngine;
using UnityEngine.Events;

namespace STYLY.Interaction.SDK.Dummy
{
    /// <summary>
    /// SceneEventNotifieダミー実装のImplSetupper。
    /// </summary>
    public class DummySceneEventNotifierSetupper : IImplSetupper
    {
        public void Setup(Component srcComponent)
        {
            var ifComp = srcComponent as ISceneNotifier;
            var impl = srcComponent.gameObject.AddComponent<DummySceneEventNotifier>();
            impl.OnExit = ifComp.OnExit;
            impl.OnStart = ifComp.OnStart;
            impl.OnMenuClose = ifComp.OnMenuClose;
            impl.OnMenuOpen = ifComp.OnMenuOpen;
        }
    }

    /// <summary>
    /// SceneEventNotifieダミー実装。
    /// </summary>
    [AddComponentMenu("Scripts/DummyScript")]
    public class DummySceneEventNotifier : MonoBehaviour
    {
        public UnityEvent OnStart;

        public UnityEvent OnMenuOpen;

        public UnityEvent OnMenuClose;

        public UnityEvent OnExit;


        // Use this for initialization
        void Start()
        {
            OnStart.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            // TODO:メニューのオープン/クローズ検出
        }

        private void OnDestroy()
        {
            OnExit.Invoke();
        }
    }

}
