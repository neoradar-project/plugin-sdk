using NeoRadar.SamplePlugin.StatusBadge;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.StatusBadge;

public sealed class StatusBadgeRendererTests
{
    [Fact]
    public void TurnedOffAndOn_KeepsItsObjects_AndDrawsNoSecondCopy()
    {
        FakeCoreApi core = new();
        core.AircraftFake.All.Add(new PluginSdk.Aircraft.Aircraft { Callsign = "BAW1", Position = new PluginSdk.Aircraft.Position { Latitude = 51.9, Longitude = -0.4, GroundSpeed = 250, Altitude = 9000 } });
        StatusBadgeRenderer renderer = new(core, new NeoRadar.SamplePlugin.Common.ThemeColours());

        (int before, int after) = ScreenReenable.Count(renderer, new RecordingGraphics());

        Assert.True(before > 0);
        Assert.Equal(before, after);
    }

    [Fact]
    public void Refresh_FromAClientThread_TouchesNoGraphics_AndTheNextFrameShowsTheCount()
    {
        FakeCoreApi core = new();
        StatusBadgeRenderer renderer = new(core, new NeoRadar.SamplePlugin.Common.ThemeColours());
        RecordingGraphics graphics = new();
        renderer.OnFrameUpdate(graphics, Frame());
        graphics.ResetMutations();

        core.AircraftFake.All.Add(new PluginSdk.Aircraft.Aircraft { Callsign = "BAW1" });
        renderer.Refresh();

        Assert.Equal(0, graphics.Mutations);
        renderer.OnFrameUpdate(graphics, Frame());
        Assert.NotEqual(0, graphics.Mutations);
    }

    [Fact]
    public void AnIdleFrame_AsksTheHostNothing_AndChangesNothing()
    {
        FakeCoreApi core = new();
        StatusBadgeRenderer renderer = new(core, new NeoRadar.SamplePlugin.Common.ThemeColours());
        RecordingGraphics graphics = new();
        renderer.Refresh();
        renderer.OnFrameUpdate(graphics, Frame());
        int reads = core.AircraftFake.GetAllCalls;
        graphics.ResetMutations();

        renderer.OnFrameUpdate(graphics, Frame());

        Assert.Equal(reads, core.AircraftFake.GetAllCalls);
        Assert.Equal(0, graphics.Mutations);
    }

    private static PluginSdk.Rendering.FrameInfo Frame() => new(0.016, 1, null, new System.Numerics.Vector2(800, 600));

    [Fact]
    public void AThemeSwitch_RecoloursTheBadge_OnTheNextFrame_AndAnIdleFrameDoesNothing()
    {
        FakeCoreApi core = new();
        NeoRadar.SamplePlugin.Common.ThemeColours colours = new();
        StatusBadgeRenderer renderer = new(core, colours);
        RecordingGraphics graphics = new();
        renderer.OnFrameUpdate(graphics, Frame());
        renderer.OnFrameUpdate(graphics, Frame());
        graphics.ResetMutations();

        colours.Update(core.ThemeFake.Palette with { Text = new PluginSdk.Theme.ThemeColour(1, 2, 3) });
        renderer.OnFrameUpdate(graphics, Frame());

        Assert.Contains(PluginSdk.Rendering.RgbaColor.FromBytes(1, 2, 3), graphics.Live.Values);
        graphics.ResetMutations();
        renderer.OnFrameUpdate(graphics, Frame());
        Assert.Equal(0, graphics.Mutations);
    }
}
