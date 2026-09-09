namespace NeoRadar.PluginSdk.Theme;

/// <summary>
/// The client's colours. A plugin reads them rather than hard-coding RGB, so its windows and its
/// drawn surfaces follow a theme switch.
/// </summary>
public interface IThemeApi
{
    /// <summary>
    /// The client's current palette, read at call time. Re-read on
    /// <see cref="BasePlugin.OnActiveProfileChanged"/>.
    /// </summary>
    ThemePalette GetPalette();
}
