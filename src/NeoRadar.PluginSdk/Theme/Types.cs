namespace NeoRadar.PluginSdk.Theme;

/// <summary>
/// Represents one opaque theme colour with 8 bits per channel.
/// </summary>
/// <param name="R">The red channel.</param>
/// <param name="G">The green channel.</param>
/// <param name="B">The blue channel.</param>
public readonly record struct ThemeColour(byte R, byte G, byte B);

/// <summary>
/// Represents the colours of the client's active theme.
/// </summary>
/// <param name="MapBackground">The radar background the plugin draws over.</param>
/// <param name="Surface">The window and panel body.</param>
/// <param name="SurfaceRaised">Cells, rows and buttons.</param>
/// <param name="Border">Separators and element outlines.</param>
/// <param name="Text">Body text.</param>
/// <param name="TextMuted">De-emphasised text.</param>
/// <param name="Accent">The theme accent.</param>
/// <param name="Success">A completed or healthy state.</param>
/// <param name="Warning">A state that needs attention.</param>
/// <param name="Danger">A failed or hazardous state.</param>
public sealed record ThemePalette(
    ThemeColour MapBackground,
    ThemeColour Surface,
    ThemeColour SurfaceRaised,
    ThemeColour Border,
    ThemeColour Text,
    ThemeColour TextMuted,
    ThemeColour Accent,
    ThemeColour Success,
    ThemeColour Warning,
    ThemeColour Danger);
