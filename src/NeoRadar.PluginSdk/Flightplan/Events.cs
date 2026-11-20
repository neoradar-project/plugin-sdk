namespace NeoRadar.PluginSdk.Flightplan;

/// <summary>
/// Represents a flight plan being filed or amended.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="Origin">The departure airport's ICAO code.</param>
/// <param name="Destination">The destination airport's ICAO code.</param>
/// <param name="Route">The parsed route.</param>
/// <param name="AcType">The aircraft type designator.</param>
/// <param name="Altitude">The planned altitude in feet as text, "0" when the filed level could not be read.</param>
/// <param name="Rules">The filed flight rules.</param>
public sealed record FlightplanUpdatedEvent(
    string Callsign,
    string Origin,
    string Destination,
    Route Route,
    string AcType,
    string Altitude,
    string Rules);

/// <summary>
/// Represents a flight plan being removed.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
public sealed record FlightplanRemovedEvent(string Callsign);

/// <summary>
/// Represents a flight plan's voice capability changing.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="OldVoiceType">The previous voice capability.</param>
/// <param name="NewVoiceType">The new voice capability.</param>
public sealed record FlightplanVoiceTypeChangedEvent(
    string Callsign,
    VoiceType OldVoiceType,
    VoiceType NewVoiceType);

/// <summary>
/// Represents a flight plan's route string changing.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="NewRoute">The new route string.</param>
public sealed record FlightplanRouteChangedEvent(string Callsign, string NewRoute);
