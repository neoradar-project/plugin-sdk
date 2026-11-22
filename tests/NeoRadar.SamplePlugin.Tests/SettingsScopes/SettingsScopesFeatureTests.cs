using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.SettingsScopes;
using NeoRadar.SamplePlugin.SettingsScopes.Events;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.SettingsScopes;

public sealed class SettingsScopesFeatureTests
{
    private readonly FakeSettingsApi _settings = new();
    private readonly PluginEventBus _bus = new(new FakeLoggerApi());
    private IReadOnlyList<string> _shown = [];

    public SettingsScopesFeatureTests() => _bus.Subscribe<SettingsProbed>(p => _shown = p.Lines);

    [Fact]
    public void AKeyNeverSet_ReadsNull_NotEmpty()
    {
        new SettingsScopesFeature(_settings, _bus).ReadUnsetKey();

        Assert.Contains($"Get({SettingsScopesFeature.UnsetKey}) returned null (never set)", _shown);
    }

    [Fact]
    public void AWriteBeforeTheSettingsLoad_ShowsWhatTheHostReturned()
    {
        _settings.Loaded = false;
        SettingsScopesFeature feature = new(_settings, _bus);

        feature.Initialize();
        feature.Show();

        Assert.Contains($"Set({SettingsScopesFeature.LoadedAtKey}) in Initialize returned False", _shown);
    }

    [Fact]
    public void AProfileWrite_WithNoProfileActive_IsRefused_AndSaysWhy()
    {
        _settings.ActiveProfile = null;

        new SettingsScopesFeature(_settings, _bus).WriteForProfile();

        Assert.Contains($"SetForProfile({SettingsScopesFeature.ProfileKey}) returned False (no profile is active)", _shown);
    }
}
