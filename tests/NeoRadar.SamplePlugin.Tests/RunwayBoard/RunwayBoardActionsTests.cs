using NeoRadar.PluginSdk.Airport;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.RunwayBoard;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.RunwayBoard;

public sealed class RunwayBoardActionsTests
{
    private readonly FakeCoreApi _core = new();
    private readonly DemoWritesGate _gate = new();
    private readonly RunwayBoardActions _actions;

    public RunwayBoardActionsTests()
    {
        _core.AirportFake.Configurations.Add(new AirportConfig("EGLL", ["27L"], ["27R"], AirportStatus.Active));
        _actions = new RunwayBoardActions(_core.AirportFake, _core.NavdataFake, _gate, new PluginEventBus(_core.LoggerFake));
    }

    [Fact]
    public void AnAirportWrite_BeforeDemoWrites_CallsNothing()
    {
        _actions.ToggleObserved();
        _actions.DeleteAirport();

        Assert.Empty(_core.AirportFake.Calls);
    }

    [Fact]
    public void AnAirportWrite_WithDemoWrites_ActsOnTheFirstAirport()
    {
        _gate.Set(true);

        _actions.ToggleObserved();

        Assert.Equal(["status EGLL Observed"], _core.AirportFake.Calls);
    }
}
