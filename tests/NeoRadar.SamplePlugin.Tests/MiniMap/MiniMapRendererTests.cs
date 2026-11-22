using NeoRadar.SamplePlugin.MiniMap;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.MiniMap;

public sealed class MiniMapRendererTests
{
    [Fact]
    public void TurnedOffAndOn_KeepsItsObjects_AndDrawsNoSecondCopy()
    {
        FakeCoreApi core = new();
        core.AircraftFake.All.Add(new PluginSdk.Aircraft.Aircraft { Callsign = "BAW1", Position = new PluginSdk.Aircraft.Position { Latitude = 51.9, Longitude = -0.4, GroundSpeed = 250, Altitude = 9000 } });
        MiniMapRenderer renderer = new(core, new NeoRadar.SamplePlugin.Common.ThemeColours());

        (int before, int after) = ScreenReenable.Count(renderer, new RecordingGraphics());

        Assert.True(before > 0);
        Assert.Equal(before, after);
    }
}
