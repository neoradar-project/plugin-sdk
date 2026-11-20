namespace NeoRadar.PluginSdk.Theme;

/// <summary>
/// Provides the client's theme colours so plugin windows and surfaces follow a theme switch.
/// </summary>
public interface IThemeApi
{
    /// <summary>
    /// Gets the client's current palette.
    /// </summary>
    /// <remarks>Read at call time, so re-read it on <see cref="BasePlugin.OnActiveProfileChanged"/>.</remarks>
    ThemePalette GetPalette();
}
