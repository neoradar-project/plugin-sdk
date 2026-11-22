using System.Collections.Generic;
using NeoRadar.PluginSdk.Settings;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeSettingsApi : ISettingsApi
{
    public Dictionary<string, string> Global { get; } = new();

    public Dictionary<(string Profile, string Key), string> PerProfile { get; } = new();

    public bool Loaded { get; set; } = true;

    public string? ActiveProfile { get; set; } = "Default";

    public string? Get(string key) => Global.GetValueOrDefault(key);

    public bool Set(string key, string value)
    {
        if (!Loaded) return false;
        Global[key] = value;
        return true;
    }

    public bool Remove(string key) => Global.Remove(key);

    public IReadOnlyList<string> Keys() => new List<string>(Global.Keys);

    public string? GetForProfile(string key) => ActiveProfile is null ? null : PerProfile.GetValueOrDefault((ActiveProfile, key));

    public bool SetForProfile(string key, string value)
    {
        if (!Loaded || ActiveProfile is null) return false;
        PerProfile[(ActiveProfile, key)] = value;
        return true;
    }

    public bool RemoveForProfile(string key) => ActiveProfile is not null && PerProfile.Remove((ActiveProfile, key));
}
