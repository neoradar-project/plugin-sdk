using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Airport;

/// <summary>
/// Represents the outcome of <see cref="IAirportApi.BatchUpdateRunways"/>.
/// </summary>
/// <param name="AddedSuccess">The activations the host applied.</param>
/// <param name="AddedFailed">The activations the host refused.</param>
/// <param name="RemovedSuccess">The deactivations the host applied.</param>
/// <param name="RemovedFailed">The deactivations the host refused.</param>
public sealed record BatchUpdateRunwaysResult(
    IReadOnlyList<RunwayStatusChange> AddedSuccess,
    IReadOnlyList<RunwayStatusChange> AddedFailed,
    IReadOnlyList<RunwayStatusChange> RemovedSuccess,
    IReadOnlyList<RunwayStatusChange> RemovedFailed);

/// <summary>
/// Provides access to airport and runway configuration.
/// </summary>
public interface IAirportApi
{
    /// <summary>
    /// Gets every airport configuration.
    /// </summary>
    IReadOnlyList<AirportConfig> GetConfigurations();

    /// <summary>
    /// Gets an airport's configuration by ICAO code, or null when it has none.
    /// </summary>
    AirportConfig? GetConfigurationByIcao(string icao);

    /// <summary>
    /// Returns whether a runway is active for departures.
    /// </summary>
    bool IsDepRunwayActive(string icao, string runway);

    /// <summary>
    /// Returns whether a runway is active for arrivals.
    /// </summary>
    bool IsArrRunwayActive(string icao, string runway);

    /// <summary>
    /// Activates a runway for departures or arrivals.
    /// </summary>
    bool SetRunwayStatus(string icao, string runway, RunwayType type);

    /// <summary>
    /// Sets whether an airport's extended runway centrelines are drawn.
    /// </summary>
    bool SetShowRunwayCenterlines(string icao, bool show);

    /// <summary>
    /// Deactivates a runway for departures or arrivals.
    /// </summary>
    bool RemoveRunwayStatus(string icao, string runway, RunwayType type);

    /// <summary>
    /// Activates and deactivates several runways in one call.
    /// </summary>
    BatchUpdateRunwaysResult BatchUpdateRunways(
        IReadOnlyList<RunwayStatusChange> toAdd,
        IReadOnlyList<RunwayStatusChange> toRemove);

    /// <summary>
    /// Removes an airport from the airport configuration.
    /// </summary>
    bool DeleteAirport(string icao);

    /// <summary>
    /// Sets an airport as active or observed.
    /// </summary>
    bool SetAirportStatus(string icao, AirportStatus status);
}
