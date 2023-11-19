using System;
using System.Threading;
using UnityEngine;

namespace STYLY.Uploader
{
    /// <summary>
    /// リトライ機能用ユーティリティクラス
    /// </summary>
    public static class RetryUtil
    {
        /// <summary>
        /// 任意のFuncをリトライする。
        /// Funcがtrueを返すか、リトライ回数がretryLimitに達するとリトライが終了する。
        /// </summary>
        /// <param name="func">実行対象Func</param>
        /// <param name="retryLimit">最大リトライ回数</param>
        /// <param name="sleepMillisec">リトライの間のスリープ時間（ミリ秒）</param>
        /// <returns>funcが最終的に成功したかどうか</returns>
        public static bool Retry(Func<bool> func, int retryLimit, int sleepMillisec)
        {
            int retryCount = 0;
            while(true)
            {
                if (func())
                {
                    return true;
                }
                
                if (retryCount >= retryLimit)
                {
                    Debug.Log($"Retry count({retryCount}) reached limit({retryLimit}).");
                    return false;
                }

                retryCount++;
                Debug.Log($"Retry {retryCount} ... ");
                Thread.Sleep(sleepMillisec);
            }
        }
    }
}