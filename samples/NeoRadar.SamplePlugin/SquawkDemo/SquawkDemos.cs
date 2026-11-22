using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Showcase;
using NeoRadar.SamplePlugin.SquawkDemo.Events;

namespace NeoRadar.SamplePlugin.SquawkDemo;

/// <summary>The squawk page's demos, and its readout of the provider's activity.</summary>
internal sealed class SquawkDemos : IShowcaseDemos
{
    private readonly ISquawkDemo _squawk;
    private readonly IPluginEventBus _bus;
    private readonly ShowcaseWindow _window;

    public SquawkDemos(ISquawkDemo squawk, IPluginEventBus bus, ShowcaseWindow window)
    {
        _squawk = squawk;
        _bus = bus;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("RegisterSquawk", _squawk.Register),
        new("UnregisterSquawk", _squawk.Unregister),
        new("ListSquawk", _squawk.ListProviders),
        new("ActivateSquawk", _squawk.MakeActive),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [_bus.Subscribe<SquawkActivityRecorded>(e => _window.ShowReadout(ShowcaseGuide.Squawk, e.Lines))];
}
