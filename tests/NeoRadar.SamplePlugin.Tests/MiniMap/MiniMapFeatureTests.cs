using NeoRadar.SamplePlugin.Alerts.Events;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.MiniMap;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.MiniMap;

public sealed class MiniMapFeatureTests
{
    [Fact]
    public void AfterShutdown_AFollowedSelection_NoLongerMovesTheMap()
    {
        FakeCoreApi core = new();
        PluginEventBus bus = new(core.LoggerFake);
        MiniMapFeature feature = new(core, HostSurfaces.FromHost(core), new ThemeColours(), bus);
        feature.Initialize();
        feature.Shutdown();
        core.RenderingFake.Interface.Centred.Clear();

        bus.Publish(new SelectedAircraftFollowed("BAW1", 51.5, -0.5));

        Assert.Empty(core.RenderingFake.Interface.Centred);
    }
}
