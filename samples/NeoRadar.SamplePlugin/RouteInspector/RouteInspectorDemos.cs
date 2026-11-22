using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.RouteInspector.Events;
using NeoRadar.SamplePlugin.Showcase;

namespace NeoRadar.SamplePlugin.RouteInspector;

/// <summary>The flight plans page's demo, and its readout of the inspected route.</summary>
internal sealed class RouteInspectorDemos : IShowcaseDemos
{
    private readonly IRouteInspector _route;
    private readonly IPluginEventBus _bus;
    private readonly ShowcaseWindow _window;

    public RouteInspectorDemos(IRouteInspector route, IPluginEventBus bus, ShowcaseWindow window)
    {
        _route = route;
        _bus = bus;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("InspectRoute", _route.InspectSelected),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [_bus.Subscribe<RouteInspected>(e => _window.ShowReadout(ShowcaseGuide.FlightPlans, e.Lines))];
}
