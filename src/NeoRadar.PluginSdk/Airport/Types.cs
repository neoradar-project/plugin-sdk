using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Airport;

/// <summary>
/// Specifies how the user is working an airport.
/// </summary>
public enum AirportStatus
{
    /// <summary>
    /// The user controls the airport.
    /// </summary>
    Active = 0,

    /// <summary>
    /// The user only observes the airport.
    /// </summary>
    Observed = 1,
}

/// <summary>
/// Specifies the role a runway is active for.
/// </summary>
public enum RunwayType
{
    /// <summary>
    /// The runway is used for departures.
    /// </summary>
    Departure = 0,

    /// <summary>
    /// The runway is used for arrivals.
    /// </summary>
    Arrival = 1,
}

/// <summary>
/// Represents one runway activation or deactivation.
/// </summary>
/// <param name="Icao">The airport's ICAO code.</param>
/// <param name="Runway">The runway designator.</param>
/// <param name="Type">The role the runway is changed for.</param>
public sealed record RunwayStatusChange(string Icao, string Runway, RunwayType Type);

/// <summary>
/// Represents an airport's runway configuration.
/// </summary>
/// <param name="Icao">The airport's ICAO code.</param>
/// <param name="ArrRunways">The runways active for arrivals.</param>
/// <param name="DepRunways">The runways active for departures.</param>
/// <param name="Status">Whether the airport is active or observed.</param>
/// <param name="ShowCenterlines">Whether extended runway centrelines are drawn.</param>
public sealed record AirportConfig(
    string Icao,
    IReadOnlyList<string> ArrRunways,
    IReadOnlyList<string> DepRunways,
    AirportStatus Status,
    bool ShowCenterlines = true);
