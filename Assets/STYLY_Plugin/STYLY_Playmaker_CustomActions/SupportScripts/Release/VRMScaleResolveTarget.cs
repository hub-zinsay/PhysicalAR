using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// VRMモデルのスケールを変更した際に、髪などの揺れものがおかしくなる問題を解決するコンポーネントです。
    /// VRMモデルのルートノードに付けてると、STYLY上での表示時に解決されます。
    /// 注意点として、このコンポーネントを付ける対象のスケールはx,y,zが同じである必要があります。
    /// </summary>
    public class VRMScaleResolveTarget : MonoBehaviour
    {
        void Start()
        {
            StylyServiceForPlayMaker.Instance.ResolveVRMScale(transform, transform.lossyScale.x);
        }
    }
}
