using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Commands.Events;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Showcase;

namespace NeoRadar.SamplePlugin.Commands;

/// <summary>The commands page has no button, as its commands are typed in chat; its readout lists them.</summary>
internal sealed class CommandsDemos : IShowcaseDemos
{
    private readonly IPluginEventBus _bus;
    private readonly ShowcaseWindow _window;

    public CommandsDemos(IPluginEventBus bus, ShowcaseWindow window)
    {
        _bus = bus;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos => [];

    public IReadOnlyList<IDisposable> ShowReports() => [_bus.Subscribe<CommandsListed>(e => _window.ShowReadout(ShowcaseGuide.Commands, e.Lines))];
}
