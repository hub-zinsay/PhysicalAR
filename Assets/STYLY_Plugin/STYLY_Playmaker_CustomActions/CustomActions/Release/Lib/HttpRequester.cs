using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// HttpのRequestのためのインターフェース
    /// </summary>
    public interface IHttpRequester : IDisposable
    {
        void Get(Uri uri, Dictionary<string, string> headers, int timeout, Action<HttpResponse> action);
        void Post(Uri uri, Dictionary<string, string> headers, string body, int timeout, Action<HttpResponse> action);
        void Put(Uri uri, Dictionary<string, string> headers, string body, int timeout, Action<HttpResponse> action);
        void Delete(Uri uri, Dictionary<string, string> headers, string body, int timeout, Action<HttpResponse> action);
    }

    /// <summary>
    /// HttpのResponseを簡単に扱うためのデータ構造
    /// </summary> 
    public class HttpResponse
    {
        public long statusCode { get; private set; }
        public string text { get; private set; }
        public string error { get; private set; }

        public HttpResponse(long statusCode, string text, string error)
        {
            this.statusCode = statusCode;
            this.text = text;
            this.error = error;
        }
    }

    /// <summary>
    /// UnityWebRequestをベースに作成したHttpRequester実装
    /// </summary>
    public class HttpRequester : IHttpRequester
    {
        private UnityWebRequest unityWebRequest;

        private UnityWebRequestAsyncOperation asyncOperation;
        private Action<AsyncOperation> asyncOperationAction;

        private Action<AsyncOperation> MakeAsyncOperationAction(UnityWebRequest unityWebRequest, Action<HttpResponse> action)
        {
            return (op) =>
            {
                action.Invoke(new HttpResponse(
                    unityWebRequest.responseCode,
                    unityWebRequest.downloadHandler.text,
                    unityWebRequest.error
                ));
            };
        }

        private void SetHeader(UnityWebRequest unityWebRequest, Dictionary<string, string> headers)
        {
            foreach (var item in headers)
            {
                unityWebRequest.SetRequestHeader(item.Key, item.Value);
            }
        }

        /// <summary>
        /// UnityWebRequestのSendRequestを呼び出す。
        /// その際に、AsyncOperationをセットアップし、クラスのメンバー変数として保持する。
        /// asyncOperation,asyncOperationActionはDisposeする際に必要となる。
        /// </summary>
        private void SendRequest(UnityWebRequest unityWebRequest, Action<HttpResponse> action)
        {
            asyncOperation = unityWebRequest.SendWebRequest();
            asyncOperationAction = MakeAsyncOperationAction(unityWebRequest, action);
            asyncOperation.completed += asyncOperationAction;
        }

        private UploadHandler MakeUploadHandler(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                return null;
            }

            var requestBody = System.Text.Encoding.GetEncoding("utf-8").GetBytes(body);
            return new UploadHandlerRaw(requestBody);
        }

        private void Execute(string method, Uri uri, Dictionary<string, string> headers, UploadHandler uploadHandler, int timeout, Action<HttpResponse> action)
        {
            var downloadHandler = new DownloadHandlerBuffer();
            unityWebRequest = new UnityWebRequest(uri, method, downloadHandler, uploadHandler);
            unityWebRequest.timeout = timeout;

            SetHeader(unityWebRequest, headers);
            SendRequest(unityWebRequest, action);
        }

        public void Get(Uri uri, Dictionary<string, string> headers, int timeout, Action<HttpResponse> action)
        {
            Execute("GET", uri, headers, null, timeout, action);
        }

        public void Post(Uri uri, Dictionary<string, string> headers, string body, int timeout, Action<HttpResponse> action)
        {
            var uploadHandler = MakeUploadHandler(body);
            Execute("POST", uri, headers, uploadHandler, timeout, action);
        }

        public void Put(Uri uri, Dictionary<string, string> headers, string body, int timeout, Action<HttpResponse> action)
        {
            var uploadHandler = MakeUploadHandler(body);
            Execute("PUT", uri, headers, uploadHandler, timeout, action);
        }

        public void Delete(Uri uri, Dictionary<string, string> headers, string body, int timeout, Action<HttpResponse> action)
        {
            var uploadHandler = MakeUploadHandler(body);
            Execute("DELETE", uri, headers, uploadHandler, timeout, action);
        }

        public void Dispose()
        {
            if (asyncOperation != null && asyncOperationAction != null)
            {
                asyncOperation.completed -= asyncOperationAction;
            }
            unityWebRequest?.Dispose();

            asyncOperation = null;
            asyncOperationAction = null;
            unityWebRequest = null;
        }
    }
}