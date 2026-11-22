namespace NeoRadar.PluginSdk.Aircraft;

/// <summary>
/// Represents a snapshot of a connected aircraft.
/// </summary>
public sealed record Aircraft
{
    /// <summary>
    /// Gets the aircraft's callsign.
    /// </summary>
    public string Callsign { get; init; } = string.Empty;

    /// <summary>
    /// Gets the pilot's network ID, or null when it is not known.
    /// </summary>
    public int? Cid { get; init; }

    /// <summary>
    /// Gets the pilot's real name, or empty until the pilot's details arrive.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the aircraft's latest position.
    /// </summary>
    public Position Position { get; init; } = new();

    /// <summary>
    /// Gets a value indicating whether the pilot's real name has been received.
    /// </summary>
    public bool HasPilotDetails { get; init; }

    /// <summary>
    /// Gets a value indicating whether the aircraft's position reports are overdue.
    /// </summary>
    public bool IsTimingOut { get; init; }

    /// <summary>
    /// Gets a value indicating whether at least one standard position report has been received.
    /// </summary>
    public bool IsReady { get; init; }

    /// <summary>
    /// Gets the squawk the transponder is set to, as four octal digits.
    /// </summary>
    public string Squawk { get; init; } = string.Empty;

    /// <summary>
    /// Gets the transponder mode.
    /// </summary>
    public TransponderMode TransponderMode { get; init; } = TransponderMode.Standby;

    /// <summary>
    /// Gets the difference between pressure altitude and true altitude, in feet.
    /// </summary>
    public int PressureDifference { get; init; }
}
