using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace STYLY.Uploader
{
    /// <summary>
    /// コンソールアプリを起動するためのユーティリティ関数の置き場所。
    /// ユースケースごとに別々の関数作ってください。
    /// </summary>
    public static class TerminalCommand
    {
        public static (string StdOut, string StdErr) RunBashFile(string filePath, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "/bin/sh",
                Arguments = $"{filePath} {arguments}",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            var process = Process.Start(startInfo);
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return (stdout, stderr);
        }

        public static bool ValidateOutput(string appPath, string expectedOutputPrefix)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = appPath,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            var process = Process.Start(startInfo);
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(stderr))
            {
                Debug.LogError(stderr);
                return false;
            }

            if (!stdout.StartsWith(expectedOutputPrefix))
            {
                Debug.Log($"Unexpected output: {stdout}");
                return false;
            }

            return true;
        }
    }
}