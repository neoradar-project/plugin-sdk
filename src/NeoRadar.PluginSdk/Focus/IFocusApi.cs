namespace NeoRadar.PluginSdk.Focus;

/// <summary>
/// The client's current selection, the equivalent of the EuroScope ASEL aircraft. Reads are cheap
/// cache lookups; a write moves the user's selection, so treat it as a user-visible action.
/// </summary>
public interface IFocusApi
{
    /// <summary>
    /// Callsign of the selected aircraft, or null when nothing is selected or the selection is not
    /// an aircraft.
    /// </summary>
    string? GetSelectedAircraft();

    /// <summary>
    /// Select an aircraft by callsign. Returns false when the callsign is unknown or the call was
    /// throttled: selection is rate limited to roughly one call per second per plugin.
    /// </summary>
    bool SelectAircraft(string callsign);
}
