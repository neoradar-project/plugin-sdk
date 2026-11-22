using NeoRadar.PluginSdk.Rendering;
using NeoRadar.PluginSdk.Theme;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Common;

public sealed class ThemeColoursTests
{
    [Fact]
    public void ANewPalette_MakesANewVersion_AndTheSamePaletteDoesNot()
    {
        ThemeColours colours = new();
        ThemePalette palette = new FakeThemeApi().Palette;

        Assert.True(colours.Update(palette));
        int version = colours.Current.Version;
        Assert.False(colours.Update(palette with { }));
        Assert.Equal(version, colours.Current.Version);

        Assert.True(colours.Update(palette with { Text = new ThemeColour(0, 0, 0) }));
        Assert.Equal(version + 1, colours.Current.Version);
        Assert.Equal(RgbaColor.FromBytes(0, 0, 0), colours.Current.Text);
    }
}
