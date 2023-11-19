#if PLAYMAKER

using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// HTTPのPutを行うアクション
    /// セキュリティの関係上、styly.ccのドメインへの実行は不可
    /// ただし、.function.styly.ccを除く
    /// </summary>
    [ActionCategory("STYLY")]
    [Tooltip("Call Put Request")]
    public class PutHttpRequest : FsmStateAction
    {
        [RequiredField]
        public FsmString url;

        [Tooltip("The request header")]
        [CompoundArray("Headers", "Key", "Value")]
        public FsmString[] headerKeys;
        public FsmString[] headerValues;

        [Tooltip("The request body")]
        public FsmString requestBody;


        [ActionSection("Results")]

        [UIHint(UIHint.Variable)]
        [Tooltip("The response status code")]
        public FsmInt httpResponseCode;

        [UIHint(UIHint.Variable)]
        [Tooltip("The response body")]
        public FsmString httpResponseBody;

        [UIHint(UIHint.Variable)]
        public FsmString errorString;

        [ActionSection("Events")]

        [Tooltip("Event to send when the data has finished loading (progress = 1).")]
        public FsmEvent isDone;

        [Tooltip("Event to send if there was an error.")]
        public FsmEvent isError;
        
        private static readonly int defaultTimeout = 100;

        private IHttpRequester httpRequester;
        public override void Reset()
        {
            url = null;

            headerKeys = new FsmString[0];
            headerValues = new FsmString[0];
            requestBody = null;

            httpResponseCode = null;
            httpResponseBody = null;
            errorString = null;

            isDone = null;
            isError = null;
        }
        public override void OnEnter()
        {
            httpRequester = new RestrictHttpRequester(
                new HttpRequester(),
                new StylyDomainRestrictUrlAccessValidator()
            );

            if (url.Value == null)
            {
                errorString.Value = "Url is required.";
                Fsm.Event(isError);
                Finish();
                return;
            }

            var requestUrl = new Uri(url.Value);
            var headers = HttpRequestCustomActionUtil.BuildHeader(headerKeys, headerValues);
            var body = requestBody.Value;

            try
            {
                httpRequester.Put(
                    requestUrl,
                    headers,
                    body,
                    defaultTimeout,
                    (response) =>
                    {
                        var statusCode = (int)response.statusCode;
                        var text = response.text;
                        var error = response.error;

                        httpResponseCode.Value = statusCode;
                        httpResponseBody.Value = text;
                        errorString.Value = error;

                        if (string.IsNullOrEmpty(error))
                        {
                            Fsm.Event(isDone);
                            Finish();
                        }
                        else
                        {
                            Fsm.Event(isError);
                            Finish();
                        }
                    }
                );
            }
            catch (Exception e)
            {
                errorString.Value = e.Message;
                Fsm.Event(isError);
                Finish();
            }
        }

        public override void OnExit()
        {
            httpRequester?.Dispose();
            httpRequester = null;
        }
    }
}

#endif