using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Settings;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.SettingsScopes.Events;

namespace NeoRadar.SamplePlugin.SettingsScopes;

/// <summary>A value that follows the controller everywhere, one kept per profile, and a key never set.</summary>
internal sealed class SettingsScopesFeature : IFeature, ISettingsScopes, IProfileEventHandler
{
    public const string LoadedAtKey = "sample.loaded-at";
    public const string GlobalKey = "sample.global";
    public const string ProfileKey = "sample.profile";
    public const string UnsetKey = "sample.never-set";

    private readonly ISettingsApi _settings;
    private readonly IPluginEventBus _bus;
    private readonly RecentLines _results = new(6);

    public SettingsScopesFeature(ISettingsApi settings, IPluginEventBus bus)
    {
        _settings = settings;
        _bus = bus;
    }

    // A write this early may wait for the saved settings to load, and is refused only if they could not be read.
    public void Initialize()
        => _results.Add($"Set({LoadedAtKey}) in Initialize returned {_settings.Set(LoadedAtKey, Now())}");

    public void Shutdown()
    {
    }

    public void WriteGlobal() => Report($"Set({GlobalKey}) returned {_settings.Set(GlobalKey, Now())}");

    public void WriteForProfile()
        => Report($"SetForProfile({ProfileKey}) returned {_settings.SetForProfile(ProfileKey, Now())}{(_settings.ActiveProfile is null ? " (no profile is active)" : string.Empty)}");

    public void ReadUnsetKey() => Report($"Get({UnsetKey}) returned {Shown(_settings.Get(UnsetKey))}");

    public void Show() => Report(null);

    // Client thread.
    public void OnActiveProfileChanged(ActiveProfileChangedEvent evt)
        => Report($"Profile changed to {evt.ProfileName ?? "none"}; {ProfileKey} reads {Shown(_settings.GetForProfile(ProfileKey))}");

    private void Report(string? result)
    {
        if (result is not null) _results.Add(result);
        List<string> lines =
        [
            $"Active profile: {_settings.ActiveProfile ?? "none"}",
            $"Plugin scope: {GlobalKey} = {Shown(_settings.Get(GlobalKey))}, keys {string.Join(", ", _settings.Keys())}",
            $"Profile scope: {ProfileKey} = {Shown(_settings.GetForProfile(ProfileKey))}",
        ];
        lines.AddRange(_results.Snapshot());
        _bus.Publish(new SettingsProbed(lines));
    }

    private static string Shown(string? value) => value is null ? "null (never set)" : $"'{value}'";

    private static string Now() => DateTimeOffset.UtcNow.ToString("u");
}
