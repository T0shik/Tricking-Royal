﻿using Battles.Enums;

namespace Battles.Extensions
{
    public static class EnumExtensions
    {
        public static string GetString(this Mode @this) =>
            @this == Mode.OneUp ? "One Up"
            : @this == Mode.ThreeRoundPass ? "Three Round Pass"
            : @this == Mode.CopyCat ? "Copy Cat"
            : @this == Mode.Trick ? "Trick"
            : "";

        public static string GetString(this Surface @this) =>
            @this == Surface.Any ? "Any"
            : @this == Surface.SprungFloor ? "Sprung Floor"
            : @this == Surface.Grass ? "Grass"
            : @this == Surface.Concrete ? "Concrete"
            : @this == Surface.Trampoline ? "Trampoline"
            : @this == Surface.Tumbling ? "Tumbling Track"
            : "";

        public static string GetString(this Status @this) =>
            @this == Status.Open ? "Open"
            : @this == Status.Active ? "Active"
            : @this == Status.Complete ? "Complete"
            : @this == Status.Pending ? "Pending"
            : @this == Status.Invite ? "Invite"
            : "";
    }
}