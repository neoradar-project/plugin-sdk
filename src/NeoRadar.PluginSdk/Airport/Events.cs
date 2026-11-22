namespace NeoRadar.PluginSdk.Airport;

/// <summary>
/// Represents an airport being added to the airport configuration.
/// </summary>
/// <param name="Icao">The airport's ICAO code, or its navdata ident when it has no ICAO code.</param>
public sealed record AirportAddedEvent(string Icao);

/// <summary>
/// Represents an airport being removed from the airport configuration.
/// </summary>
/// <param name="Icao">The airport's ICAO code, or its navdata ident when it has no ICAO code.</param>
public sealed record AirportRemovedEvent(string Icao);

/// <summary>
/// Represents an airport changing between active and observed.
/// </summary>
/// <remarks>Setting the status an airport already holds raises no event.</remarks>
/// <param name="Icao">The airport's ICAO code, or its navdata ident when it has no ICAO code.</param>
/// <param name="Status">The airport's new status.</param>
public sealed record AirportStatusChangedEvent(string Icao, AirportStatus Status);

/// <summary>
/// Represents a runway being activated or deactivated.
/// </summary>
/// <param name="Icao">The airport's ICAO code, or its navdata ident when it has no ICAO code.</param>
/// <param name="Runway">The runway designator.</param>
/// <param name="Active"><c>true</c> when the runway was activated, <c>false</c> when it was deactivated.</param>
/// <param name="IsArrival"><c>true</c> for the arrival role, <c>false</c> for the departure role.</param>
public sealed record RunwayStatusChangedEvent(string Icao, string Runway, bool Active, bool IsArrival);

/// <summary>
/// Represents the set of airport configurations being replaced.
/// </summary>
/// <param name="ConfigurationCount">The number of airport configurations now held.</param>
public sealed record AirportConfigurationsUpdatedEvent(int ConfigurationCount);
