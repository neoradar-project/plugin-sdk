using NeoRadar.PluginSdk.Rendering;
using NeoRadar.PluginSdk.Theme;

namespace NeoRadar.SamplePlugin.Common;

/// <summary>The client's theme as drawing colours, swapped whole when the theme changes.</summary>
/// <remarks>Written by one job, read on the render thread: a frame compares Version and recolours only on a change.</remarks>
internal sealed class ThemeColours
{
    private ThemePalette? _palette;
    private volatile ThemeSnapshot _current = ThemeSnapshot.Initial;

    public ThemeSnapshot Current => _current;

    /// <returns>Whether the palette differed from the one held.</returns>
    public bool Update(ThemePalette palette)
    {
        if (palette == _palette) return false;
        _palette = palette;
        _current = new ThemeSnapshot(
            _current.Version + 1,
            Colour(palette.Surface),
            Colour(palette.SurfaceRaised),
            Colour(palette.Border),
            Colour(palette.Text),
            Colour(palette.TextMuted),
            Colour(palette.Accent),
            Colour(palette.Warning));
        return true;
    }

    private static RgbaColor Colour(ThemeColour colour) => RgbaColor.FromBytes(colour.R, colour.G, colour.B);
}
