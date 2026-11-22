namespace NeoRadar.PluginSdk.Controller;

/// <summary>
/// Represents a controller's ATIS lines changing.
/// </summary>
/// <param name="Callsign">The controller's callsign.</param>
/// <param name="LineCount">The number of ATIS lines now held.</param>
public sealed record AtisLinesUpdateEvent(string Callsign, int LineCount);

/// <summary>
/// Represents a controller's reported capabilities changing.
/// </summary>
/// <param name="Callsign">The controller's callsign.</param>
public sealed record CapabilitiesUpdateEvent(string Callsign);

/// <summary>
/// Represents a controller disconnecting from the network.
/// </summary>
/// <param name="Callsign">The controller's callsign.</param>
public sealed record ControllerDisconnectedEvent(string Callsign);

/// <summary>
/// Represents a controller connecting to the network.
/// </summary>
/// <param name="Callsign">The controller's callsign.</param>
/// <param name="Name">The controller's real name.</param>
/// <param name="Cid">The controller's network ID.</param>
/// <param name="Rating">The controller's network rating.</param>
/// <param name="Facility">The facility the controller is working.</param>
public sealed record ControllerConnectedEvent(
    string Callsign,
    string Name,
    string Cid,
    Rating Rating,
    FacilityType Facility);

/// <summary>
/// Represents the host resolving whether a controller is a working ATC position.
/// </summary>
/// <param name="Callsign">The controller's callsign.</param>
/// <param name="IsATC">Whether the controller is a working ATC position.</param>
public sealed record IsControllerATCEvent(string Callsign, bool IsATC);
