using System;
using NeoRadar.PluginSdk.Theme;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Scheduling;

namespace NeoRadar.SamplePlugin.ThemeFollow;

/// <summary>Reads the client's palette once a second, as a theme switch raises no event, and hands renderers the colours.</summary>
internal sealed class ThemeFollowFeature : IFeature
{
    private static readonly TimeSpan ReadPeriod = TimeSpan.FromSeconds(1);

    private readonly IThemeApi _theme;
    private readonly ThemeColours _colours;
    private readonly ITimedTaskScheduler _scheduler;
    private IDisposable? _job;

    public ThemeFollowFeature(IThemeApi theme, ThemeColours colours, ITimedTaskScheduler scheduler)
    {
        _theme = theme;
        _colours = colours;
        _scheduler = scheduler;
    }

    // GetPalette parses on every call, so it runs on the pool once a second rather than in each frame.
    public void Initialize()
    {
        _colours.Update(_theme.GetPalette());
        _job = _scheduler.Every("read-theme-palette", ReadPeriod, () => _colours.Update(_theme.GetPalette()));
    }

    public void Shutdown()
    {
        _job?.Dispose();
        _job = null;
    }
}
