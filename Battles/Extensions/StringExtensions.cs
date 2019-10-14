using System;

namespace Battles.Extensions
{
    public static class StringExtensions
    {
        public static string[] DefaultSplit(this string @this) =>
            @this.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

        public static string DefaultJoin(this string[] @this) =>
            string.Join("|", @this);

        public static string GetFileName(this string @this) =>
            @this.Substring(@this.LastIndexOf('/') + 1);
    }
}
