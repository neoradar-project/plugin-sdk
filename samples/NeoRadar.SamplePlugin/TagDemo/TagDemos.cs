using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Showcase;
using NeoRadar.SamplePlugin.TagDemo.Events;

namespace NeoRadar.SamplePlugin.TagDemo;

/// <summary>The tags page's demos, and its readout of tag pushes and actions.</summary>
internal sealed class TagDemos : IShowcaseDemos
{
    private readonly ITagDemo _tags;
    private readonly IPluginEventBus _bus;
    private readonly ShowcaseWindow _window;

    public TagDemos(ITagDemo tags, IPluginEventBus bus, ShowcaseWindow window)
    {
        _tags = tags;
        _bus = bus;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("PushStatus", _tags.PushStatus),
        new("PushEmptyStatus", _tags.PushEmptyStatus),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [_bus.Subscribe<TagActivityRecorded>(e => _window.ShowReadout(ShowcaseGuide.Tags, e.Lines))];
}
