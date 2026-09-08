using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Navdata;

/// <summary>
/// Read-only queries against the client's navdata database. Every call is a cache-backed lookup
/// and returns an empty list, or null, when the database is closed or nothing matches.
/// </summary>
public interface INavdataApi
{
    /// <summary>Runways of an airport by ICAO code. Empty when the airport is unknown.</summary>
    IReadOnlyList<RunwayInfo> GetRunways(string icao);

    /// <summary>
    /// Navaids matching an identifier. An identifier is not unique worldwide, so this can return
    /// several; empty when nothing matches.
    /// </summary>
    IReadOnlyList<NavaidInfo> GetNavaids(string identifier);

    /// <summary>The first airport matching an ICAO code, or null when nothing matches.</summary>
    AirportInfo? GetAirport(string icao);
}
