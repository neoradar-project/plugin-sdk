using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.QuietHours.Events;
using NeoRadar.SamplePlugin.Showcase;

namespace NeoRadar.SamplePlugin.QuietHours;

/// <summary>The expressions page's demos, and its readout of the values pushed.</summary>
internal sealed class QuietHoursDemos : IShowcaseDemos
{
    private readonly IQuietHours _expressions;
    private readonly IPluginEventBus _bus;
    private readonly ShowcaseWindow _window;

    public QuietHoursDemos(IQuietHours expressions, IPluginEventBus bus, ShowcaseWindow window)
    {
        _expressions = expressions;
        _bus = bus;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("QuietHoursOn", () => _expressions.SetQuiet(true)),
        new("QuietHoursOff", () => _expressions.SetQuiet(false)),
        new("FlagSelected", _expressions.ToggleFlagOnSelected),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [_bus.Subscribe<ExpressionsPushed>(e => _window.ShowReadout(ShowcaseGuide.Expressions, e.Lines))];
}
