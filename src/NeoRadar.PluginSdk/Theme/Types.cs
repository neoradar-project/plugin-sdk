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
/// <param name="MapBackground">The colour of the radar background the plugin draws over.</param>
/// <param name="Surface">The colour of window and panel bodies.</param>
/// <param name="SurfaceRaised">The colour of cells, rows and buttons.</param>
/// <param name="Border">The colour of separators and element outlines.</param>
/// <param name="Text">The colour of body text.</param>
/// <param name="TextMuted">The colour of de-emphasised text.</param>
/// <param name="Accent">The theme's accent colour.</param>
/// <param name="Success">The colour of a completed or healthy state.</param>
/// <param name="Warning">The colour of a state that needs attention.</param>
/// <param name="Danger">The colour of a failed or hazardous state.</param>
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
