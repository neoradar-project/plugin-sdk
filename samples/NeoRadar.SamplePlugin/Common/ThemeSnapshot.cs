using NeoRadar.PluginSdk.Rendering;

namespace NeoRadar.SamplePlugin.Common;

/// <summary>One theme's colours, never changed once made; Version counts the themes seen.</summary>
internal sealed record ThemeSnapshot(
    int Version,
    RgbaColor Surface,
    RgbaColor SurfaceRaised,
    RgbaColor Border,
    RgbaColor Text,
    RgbaColor TextMuted,
    RgbaColor Accent,
    RgbaColor Warning)
{
    /// <summary>A dark default until the first palette is read.</summary>
    public static ThemeSnapshot Initial { get; } = new(
        0,
        new RgbaColor(0.05f, 0.05f, 0.05f),
        new RgbaColor(0.10f, 0.11f, 0.12f),
        new RgbaColor(0.32f, 0.40f, 0.45f),
        new RgbaColor(0.95f, 0.95f, 0.95f),
        new RgbaColor(0.60f, 0.62f, 0.64f),
        new RgbaColor(0.18f, 0.78f, 0.88f),
        new RgbaColor(1f, 0.65f, 0.05f));
}
