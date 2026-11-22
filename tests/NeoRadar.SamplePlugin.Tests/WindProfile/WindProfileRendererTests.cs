using NeoRadar.SamplePlugin.Tests.Fakes;
using NeoRadar.SamplePlugin.WindProfile;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.WindProfile;

public sealed class WindProfileRendererTests
{
    [Fact]
    public void TurnedOffAndOn_KeepsItsObjects_AndDrawsNoSecondCopy()
    {
        FakeCoreApi core = new();
        core.AircraftFake.All.Add(new PluginSdk.Aircraft.Aircraft { Callsign = "BAW1", Position = new PluginSdk.Aircraft.Position { Latitude = 51.9, Longitude = -0.4, GroundSpeed = 250, Altitude = 9000 } });
        WindProfileRenderer renderer = new(core);

        (int before, int after) = ScreenReenable.Count(renderer, new RecordingGraphics());

        Assert.True(before > 0);
        Assert.Equal(before, after);
    }
}
