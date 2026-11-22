namespace NeoRadar.PluginSdk.Flightplan;

/// <summary>
/// Represents any part of a flight plan changing, its parsed route included.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="Origin">The departure airport's ICAO code.</param>
/// <param name="Destination">The destination airport's ICAO code.</param>
/// <param name="Route">The route as it stands, whose parsed form can follow in a later update.</param>
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
/// Represents a flight plan's route or its parsed form changing.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="NewRoute">The route string, which may be the one already held.</param>
public sealed record FlightplanRouteChangedEvent(string Callsign, string NewRoute);
