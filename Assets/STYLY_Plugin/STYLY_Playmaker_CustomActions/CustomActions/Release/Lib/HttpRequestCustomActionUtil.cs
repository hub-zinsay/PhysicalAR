#if PLAYMAKER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    public class HttpRequestCustomActionUtil
    {
        private HttpRequestCustomActionUtil() { }

        public static Dictionary<string, string> BuildHeader(FsmString[] headerKeys, FsmString[] headerValues)
        {
            var headers = new Dictionary<string, string>();
            int i = 0;
            foreach (FsmString headerKey in headerKeys)
            {
                headers.Add(headerKey.Value, headerValues[i].Value);
                i++;
            }

            return headers;
        }
    }
}
#endif