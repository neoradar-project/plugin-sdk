using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoRadar.PluginSdk.Squawk;

/// <summary>
/// Plugin-implemented squawk provider. Returned squawk must be 4 octal digits (0-7).
/// </summary>
public interface ISquawkProvider
{
    /// <summary>
    /// Generate a squawk for the given aircraft / flightplan. Return four octal digits (0-7), or
    /// <c>null</c> to decline so the host tries the next provider. Called off the UI thread, never
    /// on the render thread. Honour <paramref name="cancellationToken"/>: the host cancels it once
    /// its timeout elapses and discards a result that arrives afterwards. A routine decline is a
    /// <c>null</c> return, never an exception.
    /// </summary>
    Task<string?> GenerateSquawkAsync(
        string callsign,
        Aircraft.Aircraft aircraft,
        Flightplan.Flightplan? flightplan,
        CancellationToken cancellationToken);

    /// <summary>Provider name (must be unique within the host).</summary>
    string ProviderName { get; }

    /// <summary>Higher = preferred when multiple providers register.</summary>
    int Priority { get; }
}

public sealed record SquawkAssignedEvent(string Callsign, string Squawk, string ProviderName);

public interface ISquawkApi
{
    bool RegisterProvider(ISquawkProvider provider);

    /// <summary>
    /// Register and return an <see cref="IDisposable"/> that unregisters on dispose.
    /// </summary>
    IDisposable RegisterProviderWithToken(ISquawkProvider provider);

    bool SetActiveProvider(string providerName);

    IReadOnlyList<string> GetAvailableProviders();
}
