using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Showcase;

namespace NeoRadar.SamplePlugin.StatusBadge;

/// <summary>The screens page's status badge switch.</summary>
internal sealed class StatusBadgeDemos : IShowcaseDemos
{
    private readonly IStatusBadgeScreen _badge;
    private readonly ShowcaseWindow _window;

    public StatusBadgeDemos(IStatusBadgeScreen badge, ShowcaseWindow window)
    {
        _badge = badge;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("ToggleStatusBadge", Toggle),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [];

    private void Toggle() => _window.ShowReadout(ShowcaseGuide.Screens, [$"Status badge is now {(_badge.Toggle() ? "on" : "off")}"]);
}
