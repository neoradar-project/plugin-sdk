using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NeoRadar.SamplePlugin.Alerts;
using NeoRadar.SamplePlugin.Aman;
using NeoRadar.SamplePlugin.Commands;
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
using NeoRadar.SamplePlugin.ThemeFollow;
using NeoRadar.SamplePlugin.Tlm;
using NeoRadar.SamplePlugin.TrackWrites;
using NeoRadar.SamplePlugin.WarningZone;
using NeoRadar.SamplePlugin.WhoIsOnline;
using NeoRadar.SamplePlugin.WindProfile;

namespace NeoRadar.SamplePlugin.Common;

internal static class FeatureRegistration
{
    /// <summary>Every per-area handler interface a feature may opt into.</summary>
    public static readonly IReadOnlyList<Type> HandlerInterfaces =
    [
        typeof(IAircraftEventHandler),
        typeof(IAirportEventHandler),
        typeof(IControllerEventHandler),
        typeof(IControllerDataEventHandler),
        typeof(IFlightplanEventHandler),
        typeof(IFsdEventHandler),
        typeof(IChatEventHandler),
        typeof(ISquawkEventHandler),
        typeof(ITagEventHandler),
        typeof(IWindowEventHandler),
        typeof(IFocusEventHandler),
        typeof(IProfileEventHandler),
        typeof(ITimerHandler),
        typeof(IMenuEventHandler),
    ];

    /// <summary>One line per feature, with its Showcase demos, in Initialize order; Shutdown runs the list in reverse.</summary>
    /// <remarks>The Showcase comes first, so it is listening before the others report.</remarks>
    public static IServiceCollection AddFeatures(this IServiceCollection services) => services
        .AddFeature<ShowcaseFeature>().AddShowcaseDemos<HostDemos>()
        .AddFeature<EventLogFeature>().AddShowcaseDemos<EventLogDemos>()
        .AddFeature<ThemeFollowFeature>()
        .AddFeature<WarningZoneFeature>()
        .AddFeature<StatusBadgeFeature>().AddShowcaseDemos<StatusBadgeDemos>()
        .AddFeature<AmanFeature>().AddShowcaseDemos<AmanDemos>()
        .AddFeature<MiniMapFeature>()
        .AddFeature<TlmFeature>()
        .AddFeature<WindProfileFeature>()
        .AddFeature<NonSeqFeature>()
        .AddFeature<CommandsFeature>().AddShowcaseDemos<CommandsDemos>()
        .AddFeature<DemoWindowFeature>().AddShowcaseDemos<DemoWindowDemos>()
        .AddFeature<RouteInspectorFeature>().AddShowcaseDemos<RouteInspectorDemos>()
        .AddFeature<TrackWritesFeature>().AddShowcaseDemos<TrackWritesDemos>()
        .AddFeature<WhoIsOnlineFeature>().AddShowcaseDemos<WhoIsOnlineDemos>()
        .AddFeature<RunwayBoardFeature>().AddShowcaseDemos<RunwayBoardDemos>()
        .AddFeature<QuietHoursFeature>().AddShowcaseDemos<QuietHoursDemos>()
        .AddFeature<TagDemoFeature>().AddShowcaseDemos<TagDemos>()
        .AddFeature<SettingsScopesFeature>().AddShowcaseDemos<SettingsScopesDemos>()
        .AddFeature<AlertsFeature>().AddShowcaseDemos<AlertsDemos>()
        .AddFeature<SquawkDemoFeature>().AddShowcaseDemos<SquawkDemos>();

    /// <summary>Registers a feature once, forwarding its handler interfaces and its own folder's interfaces to it.</summary>
    /// <exception cref="InvalidOperationException">The feature is already registered.</exception>
    public static IServiceCollection AddFeature<TFeature>(this IServiceCollection services)
        where TFeature : class, IFeature
    {
        if (services.Any(descriptor => descriptor.ServiceType == typeof(TFeature)))
            throw new InvalidOperationException($"{typeof(TFeature).Name} is registered twice");

        services.AddSingleton<TFeature>();
        services.AddSingleton<IFeature>(sp => sp.GetRequiredService<TFeature>());
        RegisteredFeaturesOf(services).Add(typeof(TFeature).Name);

        foreach (Type implemented in typeof(TFeature).GetInterfaces())
        {
            if (!HandlerInterfaces.Contains(implemented) && implemented.Namespace != typeof(TFeature).Namespace) continue;
            services.AddSingleton(implemented, sp => sp.GetRequiredService<TFeature>());
        }

        return services;
    }

    private static RegisteredFeatures RegisteredFeaturesOf(IServiceCollection services)
    {
        ServiceDescriptor? existing = services.FirstOrDefault(d => d.ServiceType == typeof(RegisteredFeatures));
        if (existing?.ImplementationInstance is RegisteredFeatures registered) return registered;

        RegisteredFeatures created = new();
        services.AddSingleton(created);
        return created;
    }
}
