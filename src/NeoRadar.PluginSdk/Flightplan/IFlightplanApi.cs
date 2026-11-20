using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Flightplan;

/// <summary>
/// Provides read access to filed flight plans.
/// </summary>
public interface IFlightplanApi
{
    /// <summary>
    /// Gets every filed flight plan.
    /// </summary>
    IReadOnlyList<Flightplan> GetAll();

    /// <summary>
    /// Gets a flight plan by callsign, or null when none is filed.
    /// </summary>
    Flightplan? GetByCallsign(string callsign);
}
