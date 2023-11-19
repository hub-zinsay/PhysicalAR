using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace STYLY.Uploader
{
    /// <summary>
    /// AzCopy実行ファイルのセットアップ担当。
    /// AzCopy周りのユースケース全てこのクラスに移動してもいいかも。
    /// </summary>
    public static class AzcopyExecutableFileInstaller
    {
        public static bool Install(out string error)
        {
            try
            {
#if UNITY_EDITOR_WIN
                return InstallWin(out error);
#elif UNITY_EDITOR_OSX
                return InstallOsx(out error);
#endif
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                error = e.Message;
                return false;
            }
        }

        static bool InstallWin(out string error)
        {
            if (File.Exists(Config.AzcopyPathWindows))
            {
                error = null;
                return true;
            }

            error = "AzCopy file not found";
            return false;
        }

        static bool InstallOsx(out string error)
        {
            var (stdout, stderr) = TerminalCommand.RunBashFile(
                Config.AzcopyShPathMac, $"'{Config.AzcopyPkgPathMac}'");

            Debug.Log(stdout);

            if (!string.IsNullOrEmpty(stderr))
            {
                error = stderr;
                return false;
            }

            var homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var tmpAzcopyPath = Path.Combine(homePath, "Applications", "azcopy");
            var dirName = new FileInfo(Config.AzcopyPathMac).DirectoryName;
            // create directory if not exists
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            // delete file if already exists
            if (File.Exists(Config.AzcopyPathMac))
            {
                File.Delete(Config.AzcopyPathMac);
            }
            File.Move(tmpAzcopyPath, Config.AzcopyPathMac);
            AssetDatabase.Refresh();

            error = null;
            return true;
        }

        public static bool Validate()
        {
            return File.Exists(Config.GetInternalAzcopyPath());
        }
    }
}