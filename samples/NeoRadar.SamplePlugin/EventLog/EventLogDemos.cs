using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.EventLog.Events;
using NeoRadar.SamplePlugin.Showcase;

namespace NeoRadar.SamplePlugin.EventLog;

/// <summary>The event log page's demo, and its counts table and readout.</summary>
internal sealed class EventLogDemos : IShowcaseDemos
{
    private readonly IEventLog _eventLog;
    private readonly IPluginEventBus _bus;
    private readonly ShowcaseWindow _window;

    public EventLogDemos(IEventLog eventLog, IPluginEventBus bus, ShowcaseWindow window)
    {
        _eventLog = eventLog;
        _bus = bus;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("LogEachLevel", _eventLog.WriteEachLogLevel),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [_bus.Subscribe<EventsCounted>(_window.ShowCounts)];
}
