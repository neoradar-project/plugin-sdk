using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoRadar.PluginSdk.Squawk;

/// <summary>
/// Provides squawk codes on behalf of a plugin.
/// </summary>
public interface ISquawkProvider
{
    /// <summary>
    /// Generates a squawk for an aircraft.
    /// </summary>
    /// <remarks>Called off the UI and render threads; decline by returning null, never by throwing.</remarks>
    /// <param name="callsign">The aircraft's callsign.</param>
    /// <param name="aircraft">The aircraft.</param>
    /// <param name="flightplan">The aircraft's flight plan, or null when none is filed.</param>
    /// <param name="cancellationToken">Cancelled when the host's timeout elapses; a later result is discarded.</param>
    /// <returns>Four octal digits (0-7), or null to let the host try the next provider.</returns>
    Task<string?> GenerateSquawkAsync(
        string callsign,
        Aircraft.Aircraft aircraft,
        Flightplan.Flightplan? flightplan,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the provider's name, which must be unique within the host, compared without case.
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Gets the provider's priority; a higher value is preferred when several are registered.
    /// </summary>
    int Priority { get; }
}

/// <summary>
/// Represents a squawk provider assigning a code to an aircraft.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="Squawk">The assigned code.</param>
/// <param name="ProviderName">The provider that generated the code.</param>
public sealed record SquawkAssignedEvent(string Callsign, string Squawk, string ProviderName);

/// <summary>
/// Provides registration and selection of squawk providers.
/// </summary>
public interface ISquawkApi
{
    /// <summary>
    /// Registers a squawk provider.
    /// </summary>
    /// <returns><c>false</c> when a provider of that name is already registered or the provider is invalid.</returns>
    bool RegisterProvider(ISquawkProvider provider);

    /// <summary>
    /// Registers a squawk provider and returns a handle that unregisters it on dispose.
    /// </summary>
    /// <returns>The handle, or null when a provider of that name is already registered or the provider is invalid.</returns>
    IDisposable? RegisterProviderWithToken(ISquawkProvider provider);

    /// <summary>
    /// Sets which registered provider the host asks first.
    /// </summary>
    bool SetActiveProvider(string providerName);

    /// <summary>
    /// Gets the names of every registered provider the loaded system allows.
    /// </summary>
    /// <remarks>A provider the system's squawk policy disables still registers but is left out here, is never asked, and cannot be made active.</remarks>
    IReadOnlyList<string> GetAvailableProviders();
}
