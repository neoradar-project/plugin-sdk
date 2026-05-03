using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Aircraft;

public sealed record AircraftConnectedEvent(string Callsign, int? Cid);

public sealed record AircraftDisconnectedEvent(string Callsign);

public sealed record PositionUpdateEvent(IReadOnlyList<Aircraft> Aircrafts);
