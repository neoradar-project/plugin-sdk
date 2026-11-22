using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.SettingsScopes.Events;
using NeoRadar.SamplePlugin.Showcase;

namespace NeoRadar.SamplePlugin.SettingsScopes;

/// <summary>The settings page's demos, and its readout of both scopes.</summary>
internal sealed class SettingsScopesDemos : IShowcaseDemos
{
    private readonly ISettingsScopes _settings;
    private readonly IPluginEventBus _bus;
    private readonly ShowcaseWindow _window;

    public SettingsScopesDemos(ISettingsScopes settings, IPluginEventBus bus, ShowcaseWindow window)
    {
        _settings = settings;
        _bus = bus;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("WriteGlobalSetting", _settings.WriteGlobal),
        new("WriteProfileSetting", _settings.WriteForProfile),
        new("ReadUnsetSetting", _settings.ReadUnsetKey),
        new("ShowSettings", _settings.Show),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [_bus.Subscribe<SettingsProbed>(e => _window.ShowReadout(ShowcaseGuide.Settings, e.Lines))];
}
