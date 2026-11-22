using NeoRadar.PluginSdk.Theme;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeThemeApi : IThemeApi
{
    public int Reads { get; private set; }

    public ThemePalette Palette { get; set; } = new(
        new ThemeColour(10, 10, 10),
        new ThemeColour(30, 30, 30),
        new ThemeColour(45, 45, 45),
        new ThemeColour(80, 80, 80),
        new ThemeColour(230, 230, 230),
        new ThemeColour(150, 150, 150),
        new ThemeColour(0, 120, 215),
        new ThemeColour(40, 180, 90),
        new ThemeColour(230, 160, 20),
        new ThemeColour(210, 50, 50));

    public ThemePalette GetPalette()
    {
        Reads++;
        return Palette;
    }
}
