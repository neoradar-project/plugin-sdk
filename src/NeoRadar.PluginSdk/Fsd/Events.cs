namespace NeoRadar.PluginSdk.Fsd;

public sealed record FsdErrorEvent(string Message);

public sealed record FsdConnectionStateChangeEvent(ConnectionState State);

public sealed record FsdConnectedEvent(string Callsign, string Server, int FacilityType);

public sealed record FsdDisconnectedEvent(string Reason, bool UserInitiated);

public sealed record FsdConnectionModelUpdatedEvent(string Callsign);
