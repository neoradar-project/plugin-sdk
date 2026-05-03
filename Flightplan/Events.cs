namespace NeoRadar.PluginSdk.Flightplan;

public sealed record FlightplanUpdatedEvent(
    string Callsign,
    string Origin,
    string Destination,
    Route Route,
    string AcType,
    string Altitude,
    string Rules);

public sealed record FlightplanRemovedEvent(string Callsign);

public sealed record FlightplanVoiceTypeChangedEvent(
    string Callsign,
    VoiceType OldVoiceType,
    VoiceType NewVoiceType);

public sealed record FlightplanRouteChangedEvent(string Callsign, string NewRoute);
