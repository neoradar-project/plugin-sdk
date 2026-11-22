using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Navdata;

/// <summary>
/// Provides read-only lookups in the client's navdata database.
/// </summary>
/// <remarks>Every call returns empty or null when the database is closed or nothing matches.</remarks>
public interface INavdataApi
{
    /// <summary>
    /// Gets an airport's runways by ICAO code, each with a position at both ends.
    /// </summary>
    /// <remarks>A runway the navdata gives no position for at one end or both is left out.</remarks>
    IReadOnlyList<RunwayInfo> GetRunways(string icao);

    /// <summary>
    /// Gets every navaid with the given identifier, since identifiers are not unique worldwide.
    /// </summary>
    IReadOnlyList<NavaidInfo> GetNavaids(string identifier);

    /// <summary>
    /// Gets the first airport matching an ICAO code.
    /// </summary>
    AirportInfo? GetAirport(string icao);
}
