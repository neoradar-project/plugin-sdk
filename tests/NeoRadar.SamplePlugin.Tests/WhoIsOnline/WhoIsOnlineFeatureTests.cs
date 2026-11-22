using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Tests.Fakes;
using NeoRadar.SamplePlugin.WhoIsOnline;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.WhoIsOnline;

public sealed class WhoIsOnlineFeatureTests
{
    [Fact]
    public void Shutdown_StopsTheRefreshJob_ItsInitializeScheduled()
    {
        FakeCoreApi core = new();
        FakeTimedTaskScheduler scheduler = new();
        WhoIsOnlineFeature feature = new(core.ControllerFake, core.FsdFake, core.ChatFake, scheduler, new PluginEventBus(core.LoggerFake));
        feature.Initialize();

        feature.Shutdown();

        Assert.True(Assert.Single(scheduler.Jobs).Disposed);
    }
}
