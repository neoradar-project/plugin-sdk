using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Aircraft;

/// <summary>
/// Provides read access to connected aircraft.
/// </summary>
public interface IAircraftApi
{
    /// <summary>
    /// Gets every connected aircraft.
    /// </summary>
    IReadOnlyList<Aircraft> GetAll();

    /// <summary>
    /// Gets an aircraft by callsign, or null when it is not connected.
    /// </summary>
    Aircraft? GetByCallsign(string callsign);

    /// <summary>
    /// Gets the aircraft's distance from its departure airport.
    /// </summary>
    /// <returns>The distance in nautical miles, or null when the aircraft or its origin is unknown.</returns>
    double? GetDistanceFromOrigin(string callsign);

    /// <summary>
    /// Gets the aircraft's distance to its destination airport.
    /// </summary>
    /// <returns>The distance in nautical miles, or null when the aircraft or its destination is unknown.</returns>
    double? GetDistanceToDestination(string callsign);
}
