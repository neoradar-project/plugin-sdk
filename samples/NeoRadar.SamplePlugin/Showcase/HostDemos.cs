using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Scheduling;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>The Showcase's own demos about the host: what it told the plugin, what one call costs, threads and the features.</summary>
internal sealed class HostDemos : IShowcaseDemos
{
    private readonly DescribeHost _host;
    private readonly ShowcaseWindow _window;
    private readonly ITimedTaskScheduler _scheduler;

    public HostDemos(DescribeHost host, ShowcaseWindow window, ITimedTaskScheduler scheduler)
    {
        _host = host;
        _window = window;
        _scheduler = scheduler;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("ShowPluginInfo", () => _window.ShowReadout(ShowcaseGuide.Start, _host.PluginInfo())),
        new("TimeOneHostCall", () => _window.ShowReadout(ShowcaseGuide.Boundary, _host.TimeOneHostCall())),
        new("ShowThreads", ShowThreads),
        new("ListFeatures", () => _window.ShowReadout(ShowcaseGuide.FeatureHost, _host.Features())),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [];

    // UI thread, from the click.
    private void ShowThreads()
    {
        string click = DescribeHost.Thread("This click (UI thread)");
        _window.ShowReadout(ShowcaseGuide.Threads, [click, "A pool job is due on the next timer tick..."]);
        _scheduler.RunOnce("show-threads", () => _window.ShowReadout(ShowcaseGuide.Threads, [click, DescribeHost.Thread("A scheduled job (thread pool)")]));
    }
}
