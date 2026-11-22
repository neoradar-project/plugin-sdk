using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Aircraft;

/// <summary>
/// Represents an aircraft connecting to the network.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="Cid">The pilot's network ID, or null when it is not known.</param>
public sealed record AircraftConnectedEvent(string Callsign, int? Cid);

/// <summary>
/// Represents an aircraft disconnecting from the network.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
public sealed record AircraftDisconnectedEvent(string Callsign);

/// <summary>
/// Represents one position batch.
/// </summary>
/// <param name="Aircrafts">A snapshot of every known aircraft.</param>
public sealed record PositionUpdateEvent(IReadOnlyList<Aircraft> Aircrafts);
