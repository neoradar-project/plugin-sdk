using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Showcase;
using NeoRadar.SamplePlugin.WhoIsOnline.Events;

namespace NeoRadar.SamplePlugin.WhoIsOnline;

/// <summary>The network page's demos and the rate limits page's chat burst, and their readouts.</summary>
internal sealed class WhoIsOnlineDemos : IShowcaseDemos
{
    private readonly IWhoIsOnline _stations;
    private readonly IPluginEventBus _bus;
    private readonly ShowcaseWindow _window;

    public WhoIsOnlineDemos(IWhoIsOnline stations, IPluginEventBus bus, ShowcaseWindow window)
    {
        _stations = stations;
        _bus = bus;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("RefreshStations", _stations.Refresh),
        new("PostOnSample", () => _stations.PostOnChannel("Sample")),
        new("PostOnSampleTwo", () => _stations.PostOnChannel("Sample Two")),
        new("PostOnSystem", () => _stations.PostOnChannel("SYSTEM")),
        new("ChatBurst", _stations.PostBurst),
    ];

    public IReadOnlyList<IDisposable> ShowReports() =>
    [
        _bus.Subscribe<StationsListed>(e => _window.ShowReadout(ShowcaseGuide.Network, e.Lines)),
        _bus.Subscribe<ChatBurstPosted>(e => _window.ShowReadout(ShowcaseGuide.RateLimits, e.Lines)),
    ];
}
