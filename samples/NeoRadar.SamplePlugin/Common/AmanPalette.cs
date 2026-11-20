using System;
using NeoRadar.PluginSdk.Rendering;

namespace NeoRadar.SamplePlugin.Common;

internal static class AmanPalette
{
    public static readonly RgbaColor WindowBg = new(40 / 255f, 40 / 255f, 40 / 255f, 1f);
    public static readonly RgbaColor PanelBg = new(56 / 255f, 56 / 255f, 56 / 255f, 1f);
    public static readonly RgbaColor SeparatorLine = new(70 / 255f, 70 / 255f, 70 / 255f, 1f);

    public static readonly RgbaColor ScaleBg = new(100 / 255f, 100 / 255f, 100 / 255f, 1f);
    public static readonly RgbaColor ScaleBorder = new(200 / 255f, 200 / 255f, 200 / 255f, 1f);
    public static readonly RgbaColor ScalePastBg = new(75 / 255f, 75 / 255f, 75 / 255f, 1f);

    public static readonly RgbaColor TitleBadgeBg = new(60 / 255f, 60 / 255f, 60 / 255f, 1f);
    public static readonly RgbaColor TitleBadgeFg = new(1f, 1f, 1f, 1f);

    public static readonly RgbaColor LabelArrivalFg = new(1f, 1f, 1f, 1f);
    public static readonly RgbaColor LabelDepartureBg = new(131 / 255f, 152 / 255f, 155 / 255f, 1f);
    public static readonly RgbaColor LabelDepartureFg = new(0f, 0f, 0f, 1f);
    public static readonly RgbaColor LabelHoverBg = new(150 / 255f, 150 / 255f, 150 / 255f, 1f);
    public static readonly RgbaColor LabelLineOk = new(1f, 1f, 1f, 1f);
    public static readonly RgbaColor LabelLineFlagged = new(150 / 255f, 150 / 255f, 150 / 255f, 1f);

    public static readonly RgbaColor StatusOk = new(45 / 255f, 185 / 255f, 45 / 255f, 1f);
    public static readonly RgbaColor StatusLoading = new(220 / 255f, 180 / 255f, 0 / 255f, 1f);
    public static readonly RgbaColor StatusError = new(190 / 255f, 60 / 255f, 60 / 255f, 1f);
    public static readonly RgbaColor StatusInactive = new(80 / 255f, 80 / 255f, 80 / 255f, 1f);

    public static readonly RgbaColor WakeLight = new(1f, 165 / 255f, 0f, 1f);
    public static readonly RgbaColor WakeHeavy = new(1f, 1f, 0f, 1f);

    public static readonly RgbaColor TextWhite = new(1f, 1f, 1f, 1f);
    public static readonly RgbaColor TextGrey = new(140 / 255f, 140 / 255f, 140 / 255f, 1f);
    public static readonly RgbaColor TextDim = new(180 / 255f, 180 / 255f, 180 / 255f, 1f);

    public static readonly RgbaColor NonSeqAlertBg = new(1f, 1f, 0f, 1f);
    public static readonly RgbaColor NonSeqAlertFg = new(0f, 0f, 0f, 1f);

    public static readonly RgbaColor TlmBarTop = new(75 / 255f, 243 / 255f, 98 / 255f, 1f);
    public static readonly RgbaColor TlmBarBottom = new(45 / 255f, 123 / 255f, 56 / 255f, 1f);

    public static readonly RgbaColor ScrollbarTrack = new(45 / 255f, 45 / 255f, 45 / 255f, 1f);
    public static readonly RgbaColor ScrollbarHandle = new(1f, 1f, 1f, 0.16f);
    public static readonly RgbaColor ScrollbarHandleBorder = new(1f, 1f, 1f, 0.55f);

    public const float MonoCharWidth = 6.5f;

    public static string FormatHhMm(DateTimeOffset t) => $"{t.UtcDateTime:HH:mm}";
    public static string FormatHhMmSs(DateTimeOffset t) => $"{t.UtcDateTime:HH:mm:ss}";
    public static string FormatMm(DateTimeOffset t) => $"{t.UtcDateTime:mm}";

    public static RgbaColor WakeColor(char wake) => wake switch
    {
        'L' => WakeLight,
        'H' or 'J' => WakeHeavy,
        _ => LabelArrivalFg,
    };
}
