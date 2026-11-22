using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NeoRadar.PluginSdk;
using NeoRadar.SamplePlugin.Alerts;
using NeoRadar.SamplePlugin.Aman;
using NeoRadar.SamplePlugin.Commands;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.DemoWindow;
using NeoRadar.SamplePlugin.EventLog;
using NeoRadar.SamplePlugin.MiniMap;
using NeoRadar.SamplePlugin.NonSeq;
using NeoRadar.SamplePlugin.QuietHours;
using NeoRadar.SamplePlugin.RouteInspector;
using NeoRadar.SamplePlugin.RunwayBoard;
using NeoRadar.SamplePlugin.SettingsScopes;
using NeoRadar.SamplePlugin.Showcase;
using NeoRadar.SamplePlugin.SquawkDemo;
using NeoRadar.SamplePlugin.StatusBadge;
using NeoRadar.SamplePlugin.TagDemo;
using NeoRadar.SamplePlugin.Tests.Fakes;
using NeoRadar.SamplePlugin.ThemeFollow;
using NeoRadar.SamplePlugin.Tlm;
using NeoRadar.SamplePlugin.TrackWrites;
using NeoRadar.SamplePlugin.WarningZone;
using NeoRadar.SamplePlugin.WhoIsOnline;
using NeoRadar.SamplePlugin.WindProfile;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Common;

public sealed class FeatureRegistrationTests
{
    private static readonly Type[] s_features =
    [
        typeof(ShowcaseFeature),
        typeof(EventLogFeature),
        typeof(ThemeFollowFeature),
        typeof(WarningZoneFeature),
        typeof(StatusBadgeFeature),
        typeof(AmanFeature),
        typeof(MiniMapFeature),
        typeof(TlmFeature),
        typeof(WindProfileFeature),
        typeof(NonSeqFeature),
        typeof(CommandsFeature),
        typeof(DemoWindowFeature),
        typeof(RouteInspectorFeature),
        typeof(TrackWritesFeature),
        typeof(WhoIsOnlineFeature),
        typeof(RunwayBoardFeature),
        typeof(QuietHoursFeature),
        typeof(TagDemoFeature),
        typeof(SettingsScopesFeature),
        typeof(AlertsFeature),
        typeof(SquawkDemoFeature),
    ];

    public static TheoryData<Type> Features => new(s_features);

    [Theory]
    [MemberData(nameof(Features))]
    public void EachFeature_IsRegisteredOnce_AndResolvesAsAFeature(Type feature)
    {
        using ServiceProvider provider = Compose();

        IFeature[] resolved = provider.GetServices<IFeature>().Where(f => f.GetType() == feature).ToArray();

        Assert.True(resolved.Length == 1, $"{feature.Name} resolved {resolved.Length} times; expected once from FeatureRegistration.AddFeatures");
    }

    [Fact]
    public void NoFeatureIsRegistered_ThatTheTestDoesNotName()
    {
        using ServiceProvider provider = Compose();
        HashSet<Type> named = s_features.ToHashSet();

        Type[] unnamed = provider.GetServices<IFeature>().Select(f => f.GetType()).Where(t => !named.Contains(t)).ToArray();

        Assert.Empty(unnamed);
    }

    [Fact]
    public void RegisteredFeatures_NamesEachFeature_InInitializeOrder()
    {
        using ServiceProvider provider = Compose();

        Assert.Equal(s_features.Select(t => t.Name), provider.GetRequiredService<RegisteredFeatures>().Names);
    }

    [Fact]
    public void AddingAFeatureTwice_FailsAtComposition()
    {
        ServiceCollection services = new();
        services.AddFeature<DemoWindowFeature>();

        InvalidOperationException refused = Assert.Throws<InvalidOperationException>(() => services.AddFeature<DemoWindowFeature>());

        Assert.Contains(nameof(DemoWindowFeature), refused.Message);
    }

    [Fact]
    public void AHandlerInterface_ResolvesToTheSameInstanceAsTheFeature()
    {
        using ServiceProvider provider = Compose();

        AmanFeature feature = provider.GetRequiredService<AmanFeature>();

        Assert.Contains(provider.GetServices<IAircraftEventHandler>(), handler => ReferenceEquals(handler, feature));
    }

    internal static ServiceProvider Compose(FakeCoreApi? core = null)
    {
        ServiceCollection services = new();
        services.AddSamplePlugin(core ?? new FakeCoreApi(), new ClientInformation("Test", "0.0.0", @"C:\NeoRadar"));
        return services.BuildServiceProvider();
    }
}
