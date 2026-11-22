using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Logger;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Common.Scheduling;
using NeoRadar.SamplePlugin.Showcase;

namespace NeoRadar.SamplePlugin.Common;

internal static class Composition
{
    /// <summary>Builds the plugin's service graph: the host's APIs, the shared parts, then every feature.</summary>
    public static IServiceCollection AddSamplePlugin(this IServiceCollection services, ICoreApi core, ClientInformation clientInfo)
    {
        services.AddSingleton(core);
        services.AddSingleton(clientInfo);
        services.AddSingleton(core.Package);
        services.AddSingleton(core.Aircraft);
        services.AddSingleton(core.Flightplan);
        services.AddSingleton(core.Controller);
        services.AddSingleton(core.ControllerData);
        services.AddSingleton(core.Airport);
        services.AddSingleton(core.Squawk);
        services.AddSingleton(core.Logger);
        services.AddSingleton(core.Fsd);
        services.AddSingleton(core.Chat);
        services.AddSingleton(core.Sound);
        services.AddSingleton(core.Focus);
        services.AddSingleton(core.Navdata);
        services.AddSingleton(core.Feedback);
        services.AddSingleton(core.Settings);
        services.AddSingleton(core.Menu);
        services.AddSingleton(core.Theme);
        services.AddSingleton(core.Expressions);

        services.AddSingleton(_ => HostSurfaces.FromHost(core));
        services.AddSingleton<IPluginEventBus, PluginEventBus>();
        services.AddSingleton<DemoWritesGate>();
        services.AddSingleton<ThemeColours>();
        services.AddSingleton(sp => new TimedTaskScheduler(sp.GetRequiredService<ILoggerApi>(), TimeProvider.System, TaskScheduler.Default));
        services.AddSingleton<ITimedTaskScheduler>(sp => sp.GetRequiredService<TimedTaskScheduler>());
        services.AddSingleton<FeatureHost>();
        services.AddSingleton(sp => new ShowcaseWindow(sp.GetRequiredService<HostSurfaces>().Window, sp.GetRequiredService<DemoWritesGate>()));
        services.AddSingleton<ShowcaseDemoCatalogue>();
        services.AddSingleton<DescribeHost>();

        return services.AddFeatures();
    }
}
