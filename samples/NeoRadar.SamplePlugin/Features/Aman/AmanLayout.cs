using System;
using NeoRadar.PluginSdk.Rendering;

namespace NeoRadar.SamplePlugin.Features.Aman;

internal static class AmanLayout
{
    public const float TopBarHeight = 50f;
    public const float TopBarRow1Height = 22f;
    public const float TopBarRow2Height = 24f;
    public const float TopBarSeparatorY = TopBarRow1Height + 2f;
    public const float FooterHeight = 18f;
    public const float ScrollbarWidth = 16f;
    public const float ScaleWidth = 60f;
    public const float ScaleMargin = 30f;
    public const float Tick1Min = 5f;
    public const float Tick5Min = 10f;
    public const float TitleBadgeHeight = 18f;
    public const float TitleBadgeWidth = 100f;
    public const float TopBarFontSize = 11f;
    public const float TickFontSize = 10f;
    public const float TitleFontSize = 11f;
    public const float LabelFontSize = 11f;
    public const float FooterFontSize = 10f;
    public const float HourglassRadius = 4f;
    public const float DotRadius = 3f;
    public const float TagLineHeight = 14f;
    public const float TagSeparation = 3f;

    public const float ScrollOffsetRange = 60f;
    public const int HistoryMinutes = 5;
    public const int FutureMinutes = 60;

    public static readonly string[] TimelineIds = { "EGLL 27L", "EGLL 27R" };
    public static readonly string[] RunwayModes = { "27L", "27R", "09L", "09R" };
    public static readonly bool[] RunwayActive = { true, true, false, false };
    public static readonly string[] StatusKinds = { "ATC", "MET", "NTP", "FPL" };

    public readonly record struct TagSpec(string Source, int Width);
    public static readonly TagSpec[] TagLayout =
    {
        new("callsign", 8),
        new("runway", 4),
        new("type", 5),
        new("wake", 1),
        new("ttg", 4),
        new("tbehind", 6),
        new("dist", 4),
    };
    public const int TotalLabelChars = 8 + 4 + 5 + 1 + 4 + 6 + 4 + 6;

    public enum ButtonId { NonSeq, Tlm, Departures, ScrollHandle }

    public readonly record struct TimelineColumn(int Index, float Left, float Width, string Id)
    {
        public float ScaleX => Left + 4f;
        public float ScaleRight => ScaleX + ScaleWidth;
        public float LabelLeft => ScaleRight + 12f;
        public float LabelRight => Left + Width - 4f;
        public float LabelMaxWidth => MathF.Max(60f, LabelRight - LabelLeft);
    }

    public static bool ColorEqual(RgbaColor a, RgbaColor b)
        => a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
}
