using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// DummyVideoPlayerで利用するリソースを与えるアセット
    /// DummyVideoPlayerResources.asset として保存し、DummyVideoPlayerから参照する。
    ///
    /// アセット作成時は以下のようなアトリビュートをつけると良い
    /// [CreateAssetMenu(fileName = "DummyVideoPlayerResources", menuName = "STYLY/DummyVideoPlayerResources", order = 1)]
    /// </summary>
    public class DummyVideoPlayerResources : ScriptableObject
    {
#pragma warning disable 649
        [Tooltip("texture that will be shown on playing HLS video on DummyVideoPlayer")]
        [SerializeField]
        private Texture hlsErrorTexture;
#pragma warning restore 649

        public Texture HlsErrorTexture => hlsErrorTexture;

#pragma warning disable 649
        [Tooltip("texture that will be shown on error at initializing video on DummyVideoPlayer")]
        [SerializeField]
        private Texture generalErrorTexture;
#pragma warning restore 649

        public Texture GeneralErrorTexture => generalErrorTexture;
    }
}
