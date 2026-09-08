namespace NeoRadar.PluginSdk.Controller;

public sealed record AtcPositionUpdateEvent(string Callsign, double Latitude, double Longitude);

public sealed record AtisLinesUpdateEvent(string Callsign, int LineCount);

public sealed record CapabilitiesUpdateEvent(string Callsign);

public sealed record ControllerDisconnectedEvent(string Callsign);

public sealed record ControllerConnectedEvent(
    string Callsign,
    string Name,
    string Cid,
    Rating Rating,
    FacilityType Facility);

public sealed record IsControllerATCEvent(string Callsign, bool IsATC);
