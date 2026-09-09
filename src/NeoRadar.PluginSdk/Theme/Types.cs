namespace NeoRadar.PluginSdk.Theme;

/// <summary>One opaque theme colour, 8 bits per channel.</summary>
/// <param name="R">Red channel.</param>
/// <param name="G">Green channel.</param>
/// <param name="B">Blue channel.</param>
public readonly record struct ThemeColour(byte R, byte G, byte B);

/// <summary>
/// The colours of the client's active theme, so plugin-drawn surfaces and windows match it.
/// </summary>
/// <param name="MapBackground">The radar ground the plugin draws over.</param>
/// <param name="Surface">Window and panel body.</param>
/// <param name="SurfaceRaised">Cells, rows, buttons.</param>
/// <param name="Border">Separators and element outlines.</param>
/// <param name="Text">Body text.</param>
/// <param name="TextMuted">De-emphasised text.</param>
/// <param name="Accent">The theme accent.</param>
/// <param name="Success">Colour for a completed or healthy state.</param>
/// <param name="Warning">Colour for a state needing attention.</param>
/// <param name="Danger">Colour for a failed or hazardous state.</param>
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
