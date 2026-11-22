namespace NeoRadar.PluginSdk.Focus;

/// <summary>
/// Provides access to the client's selected aircraft, the equivalent of the EuroScope ASEL aircraft.
/// </summary>
public interface IFocusApi
{
    /// <summary>
    /// Gets the selected aircraft's callsign, or null when nothing is selected or the selection is not an aircraft.
    /// </summary>
    /// <remarks>
    /// Null as soon as the selected aircraft leaves the client's aircraft list. The selection itself clears afterwards on
    /// the UI thread, followed by <see cref="BasePlugin.OnSelectedAircraftChanged"/>.
    /// </remarks>
    string? GetSelectedAircraft();

    /// <summary>
    /// Gets the callsign of the flight plan the selected track stands for, its correlated plan or else its own.
    /// </summary>
    /// <remarks>Read or amend a plan by this callsign rather than by the track's.</remarks>
    /// <returns>The plan's callsign, or empty when nothing is selected, the selected aircraft has left the client's aircraft list, or another track holds the plan by hand.</returns>
    string GetSelectedPlanCallsign();

    /// <summary>
    /// Selects an aircraft by callsign, which moves the user's visible selection.
    /// </summary>
    /// <remarks>Rate limited to a burst of five, then one call per second, and one call per callsign per second.</remarks>
    /// <returns><c>false</c> when the callsign is unknown or the call was throttled.</returns>
    bool SelectAircraft(string callsign);
}
