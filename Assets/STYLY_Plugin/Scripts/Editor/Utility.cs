using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace STYLY.Uploader
{
	public class Utility
	{
        public static bool IsUrl(string input)
        {
            try
            {
                var uri = new Uri(input);
                if (uri.Scheme != "http" && uri.Scheme != "https")
                {
                    return false;
                }

                return Uri.IsWellFormedUriString(input, UriKind.Absolute);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsVersionString(string version)
        {
            try
            {
                new System.Version(version);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// このメソッドを直接呼んだCSファイルを格納するフォルダのパスを、Assetフォルダからの相対パスで取得する。
        /// ※パスを知りたいフォルダ直下にあるCSファイルからこのメソッドを直接呼ぶ必要があります。
        /// スタックトレースからファイルパスを抽出しているため、直接呼ばないとスタックトレースのフレームがズレ、正常に動作しません。
        /// </summary>
        /// <returns>フォルダパス</returns>
        public static string GetFolderRelativePathFromAsset()
        {
            // 呼び出し元CSファイルのフルパスを取得する。
            var scriptFullPath = new System.Diagnostics.StackTrace(true)
                // スタックトレースの1番目を取得。1番目には呼び出し元のファイルパスが記載されている。0番目はUtility.cs。
                .GetFrame(1).GetFileName();
            
            // Assetフォルダの親フォルダのフルパスを取得する。
            var projFullPath = Path.GetDirectoryName(Application.dataPath);
            
            // Assetフォルダから呼び出し元CSファイルへの相対パスを抽出する。
            var scriptRelativePath = scriptFullPath.Substring(projFullPath.Length + 1);
            
            // 呼び出し元CSファイルを格納するフォルダの、Assetフォルダからの相対パス
            return Path.GetDirectoryName(scriptRelativePath);
        }
    }
}