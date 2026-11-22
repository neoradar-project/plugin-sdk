using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Showcase;

namespace NeoRadar.SamplePlugin.DemoWindow;

/// <summary>The screens page's button that opens the form window.</summary>
internal sealed class DemoWindowDemos : IShowcaseDemos
{
    private readonly IDemoWindow _form;

    public DemoWindowDemos(IDemoWindow form)
    {
        _form = form;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("OpenForm", _form.Open),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [];
}
