using System;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// Urlをバリデーションするためのインターフェース
    /// </summary>
    public interface IUrlValidator
    {
        bool isValid(Uri uri);
    }

    /// <summary>
    /// styly関連のドメインにHttpリクエストを拒絶するUrlValidator実装
    /// </summary>
    public class StylyDomainRestrictUrlAccessValidator : IUrlValidator
    {
        public bool isValid(Uri uri)
        {
            var host = uri.Host;

            // STYLYのAPI等にアクセスしないようにするため、styly.ccドメイン内は特定のサブドメインのみ許可する
            if (host.EndsWith("styly.cc"))
            {
                return host.EndsWith(".function.styly.cc");
            }

            return true;
        }
    }
}