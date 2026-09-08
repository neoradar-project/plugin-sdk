namespace NeoRadar.PluginSdk.Focus;

/// <summary>
/// The selected aircraft changed. <c>Callsign</c> is null when the selection was cleared or moved
/// to something that is not an aircraft.
/// </summary>
public sealed record SelectedAircraftChangedEvent(string? Callsign);
