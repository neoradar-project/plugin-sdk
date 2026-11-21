namespace NeoRadar.PluginSdk.Focus;

/// <summary>
/// Provides access to the client's selected aircraft, the equivalent of the EuroScope ASEL aircraft.
/// </summary>
public interface IFocusApi
{
    /// <summary>
    /// Gets the selected aircraft's callsign, or null when nothing is selected or the selection is not an aircraft.
    /// </summary>
    /// <remarks>The selection clears when its aircraft disconnects or is lost, and the plugin hears it through <see cref="BasePlugin.OnSelectedAircraftChanged"/>.</remarks>
    string? GetSelectedAircraft();

    /// <summary>
    /// Gets the callsign of the flight plan the selected track stands for, its correlated plan or else its own.
    /// </summary>
    /// <remarks>Read or amend a plan by this callsign rather than by the track's.</remarks>
    /// <returns>The plan's callsign, or empty when nothing is selected or another track holds the plan by hand.</returns>
    string GetSelectedPlanCallsign();

    /// <summary>
    /// Selects an aircraft by callsign, which moves the user's visible selection.
    /// </summary>
    /// <returns><c>false</c> when the callsign is unknown or the call was throttled: a burst of five, then one per second, and one per callsign per second.</returns>
    bool SelectAircraft(string callsign);
}
