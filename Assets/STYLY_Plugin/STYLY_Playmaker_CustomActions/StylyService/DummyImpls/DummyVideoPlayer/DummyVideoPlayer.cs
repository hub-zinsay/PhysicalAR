using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STYLY
{
    /// <summary>
    /// ダミービデオプレイヤー
    /// アップロード前にUnity Editor上で確認するためのプレイヤー
    /// 実機上で動かすものではない。
    /// UnityのVideoPlayerで動かす。
    /// STYLYの本番の動きとは違うので、エラー時は画像に「STYLYアプリとは挙動が違います」といった文言を表示している。
    /// </summary>
    public class DummyVideoPlayer : MonoBehaviour
    {
        /// <summary>
        /// エラー時にMaterialにセットするテクスチャ
        /// </summary>
        private Texture hlsErrorTexture;
        private Texture generalErrorTexture;

        /// <summary>
        /// VideoPlayerから返されたエラー
        /// PopError() メソッドで取得することで、取得とクリアを同時に行える。
        /// </summary>
        private string receivedError;
        
        /// <summary>
        /// 初期化 (Video Init) 中かどうか
        /// </summary>
        private bool initializing;

        private VideoPlayer player;
        private AudioSource audioSource;

        /// <summary>
        /// STYLYアプリとの差異をアナウンスするためにログに出す文字列
        /// </summary>
        private const string noticeLogString =
            "**** Notice **** The behaviour of video player on Unity Editor is different from STYLY application. "
            + "Please verify the video with STYLY application.";

        private void OnReceiveError(string error)
        {
            receivedError = error;
        }

        /// <summary>
        /// get error and clear error state.
        /// </summary>
        /// <returns></returns>
        private string PopError()
        {
            var error = receivedError;
            receivedError = null;
            return error;
        }

        private bool HasError => receivedError != null;

        private void Awake()
        {
            var resources = LoadResources();
            if (resources != null)
            {
                hlsErrorTexture = resources.HlsErrorTexture;
                generalErrorTexture = resources.GeneralErrorTexture;
            }
        }

        public void Init(VideoParams videoParams, Action<Exception> onFinished)
        {
            if (initializing)
            {
                var msg = "Video Player can't be initialized when already initializing.";
                Debug.LogError(msg);
                onFinished?.Invoke(new Exception(msg));
                return;
            }
            StartCoroutine(InitCoroutine(videoParams, onFinished));
        }

        /// <summary>
        /// remove existing component if exists, and add new component.
        /// </summary>
        /// <param name="go">target GameObject</param>
        /// <typeparam name="T">type of component</typeparam>
        /// <returns></returns>
        T AddNewComponent<T>(GameObject go) where T : Component
        {
            var existingComponent = go.GetComponent<T>();
            if (existingComponent != null)
            {
                DestroyImmediate(existingComponent);
            }
            return go.AddComponent<T>();
        }

        private IEnumerator InitCoroutine(VideoParams videoParams,
            Action<Exception> onFinished)
        {
            initializing = true;
            Debug.LogWarning(noticeLogString);
            try
            {
                receivedError = null;
                audioSource = AddNewComponent<AudioSource>(gameObject);
                audioSource.volume = videoParams.volume;
                player = AddNewComponent<VideoPlayer>(gameObject);
                player.isLooping = videoParams.loop;
                player.errorReceived += (_, err) => OnReceiveError(err);
                player.playOnAwake = false;
                player.url = videoParams.uri;

                var renderer = gameObject.GetComponent<MeshRenderer>();
                if (renderer == null || renderer.material == null)
                {
                    yield return ClearComponents();
                    var msg = "Video Player needs a MeshRenderer and a material. GameObject:" + gameObject.name;
                    Debug.LogError(msg);
                    onFinished?.Invoke(new Exception(msg));
                    yield break;
                }

                renderer.material.mainTexture = player.texture;

                // check HLS (not supported on dummy player)
                if (IsHlsUrl(videoParams.uri))
                {
                    Debug.Log("hlsErrorTexture:" + hlsErrorTexture);
                    renderer.material.mainTexture = hlsErrorTexture;
                    yield return ClearComponents();
                    var msg = "Video Player error: STYLY application supports HLS video, but it does not work on Unity Editor. Please verify with STYLY application.";
                    Debug.LogError(msg);
                    onFinished?.Invoke(new Exception(msg));
                    yield break;
                }

                player.Prepare();
                yield return new WaitUntil(() => (player.isPrepared || HasError));
                var error = PopError();
                if (error != null)
                {
                    renderer.material.mainTexture = generalErrorTexture;
                    yield return ClearComponents();
                    var msg = "Video Player error:" + error;
                    Debug.LogError(msg);
                    onFinished?.Invoke(new Exception(msg));
                    yield break;
                }

                if (videoParams.autoPlay)
                {
                    player.Play();
                }

                onFinished?.Invoke(null);
            }
            finally
            {
                initializing = false;
            }
        }

        /// <summary>
        /// Initで追加したコンポーネントを削除する
        /// </summary>
        /// <returns></returns>
        private IEnumerator ClearComponents()
        {
            if (player != null)
            {
                var playerToDestroy = player;
                player = null;
                // VideoPlayerのエラー発生後即DestroyするとUnity 2019.3.6f1 (Editor) でクラッシュするので、少し時間をあける
                yield return new WaitForSeconds(1.0f);
                Destroy(playerToDestroy);
                yield return new WaitForSeconds(1.0f);
            }
            if (audioSource != null)
            {
                Destroy(audioSource);
                audioSource = null;
            }
        }

        public void Play(Action<Exception> onFinished)
        {
            if (CheckPreparedOrError("Video Play", onFinished))
            {
                player.Play();
            }
            onFinished?.Invoke(null);
        }
        
        public void Stop(Action<Exception> onFinished)
        {
            if (CheckPreparedOrError("Video Stop", onFinished))
            {
                player.Stop();
            }
            onFinished?.Invoke(null);
        }

        public void Pause(Action<Exception> onFinished)
        {
            if (CheckPreparedOrError("Video Pause", onFinished))
            {
                player.Pause();
            }
            onFinished?.Invoke(null);
        }

        public void SetVolume(float volume, Action<Exception> onFinished)
        {
            if (CheckPreparedOrError("Video Set Volume", onFinished))
            {
                audioSource.volume = volume;
            }
            onFinished?.Invoke(null);
        }

        /// <summary>
        /// VideoPlayerが保持しているClipの長さを返す。
        /// </summary>
        /// <returns></returns>
        public void GetDuration(Action<float, Exception> onFinished) 
        {
            if (CheckPreparedOrError("Video Get Duration", (ex) => onFinished(0f, ex)))
            {
                onFinished((float)player.length, null);
            }
        }

        /// <summary>
        /// Videoの現在の再生時点を秒単位で返す。
        /// </summary>
        /// <returns></returns>
        public void GetCurrentTime(Action<float, Exception> onFinished)
        {
            if (CheckPreparedOrError("Video Get Current Time", (ex) => onFinished(0f, ex)))
            {
                onFinished((float)player.time, null);
            }
        }

        /// <summary>
        /// Videoが現在再生中かどうかを返す。
        /// </summary>
        /// <param name="onFinished"></param>
        public void IsPlaying(Action<bool, Exception> onFinished) 
        {
            if (CheckPreparedOrError("Video Is Playing", (ex) => onFinished(false, ex)))
            {
                onFinished(player.isPlaying, null);
            }
        }

        /// <summary>
        /// Videoを指定の時間までシークする。
        /// </summary>
        /// <param name="time"></param>
        /// <param name="onFinished"></param>
        public void Seek(float time, Action<Exception> onFinished) 
        {
            if (CheckPreparedOrError("Video Seek", onFinished))
            {
                player.time = time;
            }
            onFinished?.Invoke(null);
        }

        /// <summary>
        /// 再生準備が完了しているかチェックし、そうでなければエラー処理をする
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="onFinished"></param>
        /// <returns>再生準備完了かどうか</returns>
        private bool CheckPreparedOrError(string actionName, Action<Exception> onFinished)
        {
            if (initializing)
            {
                var msg = $"{actionName}: can't be executed when initializing.";
                Debug.LogError(msg);
                onFinished?.Invoke(new Exception(msg));
                return false;
            }
            if (player == null)
            {
                var msg = $"{actionName}: should be called after Video Init.";
                Debug.LogError(msg);
                onFinished?.Invoke(new Exception(msg));
                return false;
            }
            return true;
        }

        /// <summary>
        /// ビデオがHLSのURLかどうかを判定する
        /// パスが.m3u8拡張子かどうかで判断する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool IsHlsUrl(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return false;
            }
            var path = new Uri(url).AbsolutePath;
            return (path.ToLower().EndsWith(".m3u8"));
        }

        /// <summary>
        /// テクスチャ等のリソースをDummyVideoPlayerResources.assetからロードする
        /// </summary>
        /// <returns></returns>
        private DummyVideoPlayerResources LoadResources()
        {
#if UNITY_EDITOR
            var guids = AssetDatabase.FindAssets("t:ScriptableObject DummyVideoPlayerResources");
            if (guids.Length != 1)
            {
                Debug.LogError("DummyVideoPlayerResources not found.");
                return null;
            }
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var resources = AssetDatabase.LoadAssetAtPath<DummyVideoPlayerResources>(path);
            return resources;
#else
            return null;
#endif
        }
    }
}