namespace NeoRadar.PluginSdk.Airport;

public sealed record AirportAddedEvent(string Icao);

public sealed record AirportRemovedEvent(string Icao);

public sealed record AirportStatusChangedEvent(string Icao, AirportStatus Status);

public sealed record RunwayStatusChangedEvent(string Icao, string Runway, bool Active, bool IsArrival);

public sealed record AirportConfigurationsUpdatedEvent(int ConfigurationCount);
