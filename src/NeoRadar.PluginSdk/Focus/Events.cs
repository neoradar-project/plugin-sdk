namespace NeoRadar.PluginSdk.Focus;

/// <summary>
/// Represents the selected aircraft changing.
/// </summary>
/// <param name="Callsign">The selected aircraft's callsign, or null when the selection was cleared or is not an aircraft.</param>
public sealed record SelectedAircraftChangedEvent(string? Callsign);
