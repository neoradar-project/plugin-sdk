using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Showcase;

namespace NeoRadar.SamplePlugin.Aman;

/// <summary>The screens page's AMAN inset switch.</summary>
internal sealed class AmanDemos : IShowcaseDemos
{
    private readonly IAmanInset _aman;
    private readonly ShowcaseWindow _window;

    public AmanDemos(IAmanInset aman, ShowcaseWindow window)
    {
        _aman = aman;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("ToggleAman", Toggle),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [];

    private void Toggle() => _window.ShowReadout(ShowcaseGuide.Screens, [$"AMAN inset is now {(_aman.Toggle() ? "on" : "off")}"]);
}
