using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace NeoRadar.SamplePlugin.Showcase;

internal static class ShowcaseDemoRegistration
{
    /// <summary>Registers a feature's Showcase demos once; the catalogue refuses a demo id used twice.</summary>
    /// <exception cref="InvalidOperationException">The demos are already registered.</exception>
    public static IServiceCollection AddShowcaseDemos<TDemos>(this IServiceCollection services)
        where TDemos : class, IShowcaseDemos
    {
        if (services.Any(d => d.ServiceType == typeof(IShowcaseDemos) && d.ImplementationType == typeof(TDemos)))
            throw new InvalidOperationException($"{typeof(TDemos).Name} is registered twice");

        return services.AddSingleton<IShowcaseDemos, TDemos>();
    }
}
