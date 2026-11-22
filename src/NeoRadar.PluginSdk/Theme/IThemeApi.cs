namespace NeoRadar.PluginSdk.Theme;

/// <summary>
/// Provides the client's theme colours so plugin windows and surfaces follow a theme switch.
/// </summary>
public interface IThemeApi
{
    /// <summary>
    /// Gets the client's current palette.
    /// </summary>
    /// <remarks>A theme switch raises no plugin event, so re-read the palette on each draw.</remarks>
    ThemePalette GetPalette();
}
