using NeoRadar.SamplePlugin.NonSeq;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.NonSeq;

public sealed class NonSeqRendererTests
{
    [Fact]
    public void TurnedOffAndOn_KeepsItsObjects_AndDrawsNoSecondCopy()
    {
        FakeCoreApi core = new();
        core.AircraftFake.All.Add(new PluginSdk.Aircraft.Aircraft { Callsign = "BAW1", Position = new PluginSdk.Aircraft.Position { Latitude = 51.9, Longitude = -0.4, GroundSpeed = 250, Altitude = 9000 } });
        NonSeqRenderer renderer = new(core);

        (int before, int after) = ScreenReenable.Count(renderer, new RecordingGraphics());

        Assert.True(before > 0);
        Assert.Equal(before, after);
    }
}
