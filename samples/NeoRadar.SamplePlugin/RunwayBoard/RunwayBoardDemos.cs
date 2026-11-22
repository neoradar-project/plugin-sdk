using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.RunwayBoard.Events;
using NeoRadar.SamplePlugin.Showcase;

namespace NeoRadar.SamplePlugin.RunwayBoard;

/// <summary>The airports page's demos and navaid lookup, and its readout of the runway board.</summary>
internal sealed class RunwayBoardDemos : IShowcaseDemos
{
    private readonly IRunwayBoard _runways;
    private readonly IPluginEventBus _bus;
    private readonly ShowcaseWindow _window;

    public RunwayBoardDemos(IRunwayBoard runways, IPluginEventBus bus, ShowcaseWindow window)
    {
        _runways = runways;
        _bus = bus;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("RefreshRunways", _runways.Refresh),
        new("ToggleCentrelines", _runways.ToggleCentrelines),
        new("ActivateRunway", _runways.ActivateDepartureRunway),
        new("DeactivateRunway", _runways.DeactivateDepartureRunway),
        new("BatchRunways", _runways.SwapRunwaysInOneBatch),
        new("ToggleObserved", _runways.ToggleObserved),
        new("DeleteAirport", _runways.DeleteAirport),
        new ShowcaseDemo("LookUpNavaid", _runways.LookUpNavaid),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [_bus.Subscribe<RunwayBoardRefreshed>(e => _window.ShowReadout(ShowcaseGuide.Airports, e.Lines))];
}
