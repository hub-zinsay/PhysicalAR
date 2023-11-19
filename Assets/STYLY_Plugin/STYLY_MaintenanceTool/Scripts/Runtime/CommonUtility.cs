using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Debug = UnityEngine.Debug;

namespace STYLY.MaintenanceTool.Utility
{

    /// <summary>
    /// 共通Utilityクラス
    /// </summary>
    public class CommonUtility
    {
        /// <summary>
        /// リトライの制限時間
        /// 長時間かかる処理はADBコマンドにはないはずなので、短い時間にする。問題があれば見直す。
        /// kill-serverコマンド実行時にタイムアウトすることがしばしばある。
        /// </summary>
        private static readonly TimeSpan timeout = TimeSpan.FromSeconds(10);

        /// <summary>リトライ間隔（ミリ秒）</summary>
        private static readonly int interval = 100;

        private Process process;
        private StringBuilder outputStringBuilder = null;
        private StringBuilder errorStringBuilder = null;

        public int RunExternalProcessSync(string command, string[] argsArray, out string outputString, out string errorString)
        {
            string args = string.Join(" ", argsArray);
            Debug.Log("command:" + command + " " + args);

            int exitCode = -1;
            var startInfo = new ProcessStartInfo(command, args);

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = false;            // 入力を読み取り不可.
            startInfo.CreateNoWindow = true;

            outputStringBuilder = new StringBuilder("");
            errorStringBuilder = new StringBuilder("");

            process = Process.Start(startInfo);
            process.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceivedHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(ErrorDataReceivedHandler);

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            try
            {
                var maxRetryNum = (int)(timeout.TotalMilliseconds / interval);
                var retryCount = 0;
                
                while (!process.WaitForExit(interval))
                {
                    Debug.Log("Waiting...");
                
                    // 制限時間を超えたら子プロセスを終了させる。（無限リトライさせない）
                    if (retryCount++ >= maxRetryNum)
                    {
                        process.Kill();
                        throw new TimeoutException("Timeout: " + command + " " + args);
                    }
                }

                // Timeout時はExitCodeはないので、ここを通ってはならない
                exitCode = process.ExitCode;
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("exception {0}", e.Message);
            }
            
            // 全ての標準出力情報を出力する。。
            process.WaitForExit();
            process.Close();

            outputString = outputStringBuilder.ToString();
            errorString = errorStringBuilder.ToString();

            outputStringBuilder = null;
            errorStringBuilder = null;

            return exitCode;
        }

        private void OutputDataReceivedHandler(object sendingProcess, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                outputStringBuilder.Append(args.Data);
                outputStringBuilder.Append(Environment.NewLine);
            }
        }

        private void ErrorDataReceivedHandler(object sendingProcess, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                errorStringBuilder.Append(args.Data);
                errorStringBuilder.Append(Environment.NewLine);
            }
        }

        public static string FixPathString(string srcPath)
        {
            srcPath = srcPath.Replace('/', Path.DirectorySeparatorChar);
            srcPath = srcPath.Replace('\\', Path.DirectorySeparatorChar);

            return srcPath;
        }
   }
}