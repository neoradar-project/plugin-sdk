using NeoRadar.PluginSdk.Theme;
using Xunit;

namespace NeoRadar.PluginSdk.Tests;

/// <summary>
/// The palette is a plain value the host fills in, so the slots and their order are the contract.
/// </summary>
public sealed class ThemePaletteTests
{
    [Fact]
    public void Palette_RoundTripsEverySlotInOrder()
    {
        ThemePalette palette = new(
            new ThemeColour(1, 2, 3),
            new ThemeColour(4, 5, 6),
            new ThemeColour(7, 8, 9),
            new ThemeColour(10, 11, 12),
            new ThemeColour(13, 14, 15),
            new ThemeColour(16, 17, 18),
            new ThemeColour(19, 20, 21),
            new ThemeColour(22, 23, 24),
            new ThemeColour(25, 26, 27),
            new ThemeColour(28, 29, 30));

        Assert.Equal(new ThemeColour(1, 2, 3), palette.MapBackground);
        Assert.Equal(new ThemeColour(4, 5, 6), palette.Surface);
        Assert.Equal(new ThemeColour(7, 8, 9), palette.SurfaceRaised);
        Assert.Equal(new ThemeColour(10, 11, 12), palette.Border);
        Assert.Equal(new ThemeColour(13, 14, 15), palette.Text);
        Assert.Equal(new ThemeColour(16, 17, 18), palette.TextMuted);
        Assert.Equal(new ThemeColour(19, 20, 21), palette.Accent);
        Assert.Equal(new ThemeColour(22, 23, 24), palette.Success);
        Assert.Equal(new ThemeColour(25, 26, 27), palette.Warning);
        Assert.Equal(new ThemeColour(28, 29, 30), palette.Danger);
    }

    [Fact]
    public void ThemeColour_CarriesTheChannelsItWasGiven()
    {
        ThemeColour colour = new(0, 128, 255);

        Assert.Equal((byte)0, colour.R);
        Assert.Equal((byte)128, colour.G);
        Assert.Equal((byte)255, colour.B);
        Assert.Equal(new ThemeColour(0, 128, 255), colour);
    }
}
