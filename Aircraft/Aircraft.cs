using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Aircraft;

public sealed record Aircraft
{
    public string Callsign { get; init; } = string.Empty;
    public int? Cid { get; init; }
    public string Name { get; init; } = string.Empty;

    public Position Position { get; init; } = new();
    public IReadOnlyList<Position> PreviousPositions { get; init; } = System.Array.Empty<Position>();

    public bool HasPilotDetails { get; init; }
    public bool IsTimingOut { get; init; }
    public bool IsReady { get; init; }

    public string LastPositionUpdateTime { get; init; } = string.Empty;

    public string Squawk { get; init; } = string.Empty;
    public TransponderMode TransponderMode { get; init; } = TransponderMode.Standby;

    public int PressureDifference { get; init; }
}
