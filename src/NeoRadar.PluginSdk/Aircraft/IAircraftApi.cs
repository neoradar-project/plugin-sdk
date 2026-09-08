using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Aircraft;

public interface IAircraftApi
{
    IReadOnlyList<Aircraft> GetAll();

    Aircraft? GetByCallsign(string callsign);

    /// <summary>Distance in nautical miles, or null if aircraft / origin missing.</summary>
    double? GetDistanceFromOrigin(string callsign);

    /// <summary>Distance in nautical miles, or null if aircraft / destination missing.</summary>
    double? GetDistanceToDestination(string callsign);
}
