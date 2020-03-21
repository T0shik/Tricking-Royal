using System;

namespace Battles.Extensions
{
    public static class DateTimeExtensions
    {
        public static string GetFinishTime(this DateTime @this) =>
            @this.ToString("dddd, dd MMMM yyyy, HH:mm");

        public static string GetKeyTime(this DateTime @this) =>
            @this.ToString("yyyy-MM-dd-HH-mm-ss");
    }
}
