using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Showcase;
using NeoRadar.SamplePlugin.TrackWrites.Events;

namespace NeoRadar.SamplePlugin.TrackWrites;

/// <summary>The controller data page's demos, and its readout of what was sent and refused.</summary>
internal sealed class TrackWritesDemos : IShowcaseDemos
{
    private readonly ITrackWrites _writes;
    private readonly IPluginEventBus _bus;
    private readonly ShowcaseWindow _window;

    public TrackWritesDemos(ITrackWrites writes, IPluginEventBus bus, ShowcaseWindow window)
    {
        _writes = writes;
        _bus = bus;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("SetScratchpad", _writes.SetScratchpad),
        new("SetClearedLevel", _writes.SetClearedLevel),
        new("BurstScratchpad", _writes.BurstScratchpad),
        new("Assume", _writes.Assume),
        new("Release", _writes.Release),
        new("AcceptHandoff", _writes.AcceptHandoffWithNoOffer),
        new("RejectHandoff", _writes.RejectHandoffWithNoOffer),
        new("CancelHandoff", _writes.CancelHandoff),
        new("SetNextController", _writes.SetNextController),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [_bus.Subscribe<TrackWritesTallied>(e => _window.ShowReadout(ShowcaseGuide.ControllerData, e.Lines))];
}
