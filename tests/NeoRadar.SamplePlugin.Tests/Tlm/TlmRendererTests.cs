using NeoRadar.SamplePlugin.Tests.Fakes;
using NeoRadar.SamplePlugin.Tlm;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Tlm;

public sealed class TlmRendererTests
{
    [Fact]
    public void TurnedOffAndOn_KeepsItsObjects_AndDrawsNoSecondCopy()
    {
        FakeCoreApi core = new();
        core.AircraftFake.All.Add(new PluginSdk.Aircraft.Aircraft { Callsign = "BAW1", Position = new PluginSdk.Aircraft.Position { Latitude = 51.9, Longitude = -0.4, GroundSpeed = 250, Altitude = 9000 } });
        TlmRenderer renderer = new(core);

        (int before, int after) = ScreenReenable.Count(renderer, new RecordingGraphics());

        Assert.True(before > 0);
        Assert.Equal(before, after);
    }
}
