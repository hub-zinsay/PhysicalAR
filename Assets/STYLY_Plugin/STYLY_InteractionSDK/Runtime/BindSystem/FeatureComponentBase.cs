using UnityEngine;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponentの抽象基底クラス。
    /// OnEnableにて派生クラスの実装部バインドを実行する。
    /// </summary>
    public abstract class FeatureComponentBase : MonoBehaviour
    {
        private bool isBinded = false;

        private void OnEnable()
        {
            BindImpl();
        }

        public void BindImpl()
        {
            if (isBinded) { return; }

            // 実装部のバインドを実行
            ImplBinder.Instance.Bind(this);
            isBinded = true;
        }

    }
}