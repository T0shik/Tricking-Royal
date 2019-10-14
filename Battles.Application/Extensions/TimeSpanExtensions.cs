using System;

namespace Battles.Application.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ConvertTimeSpan(this TimeSpan t, string ext) =>
            t.Days > 0 ? $"{t.Days} Days {ext}"
            : t.Hours > 0 ? $"{t.Hours} Hours {ext}"
            : t.Minutes > 0 ? $"{t.Minutes} Minutes {ext}"
            : t.Seconds > 0 ? $"{t.Seconds} Seconds {ext}"
            : "Expired";
    }
}