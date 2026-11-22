namespace NeoRadar.PluginSdk.ControllerData;

/// <summary>
/// Specifies how an assigned speed is to be read.
/// </summary>
public enum AssignedSpeedVariance
{
    /// <summary>
    /// The speed is exact.
    /// </summary>
    None = 0,

    /// <summary>
    /// The speed is a minimum.
    /// </summary>
    Plus = 1,

    /// <summary>
    /// The speed is a maximum.
    /// </summary>
    Minus = 2,
}

/// <summary>
/// Specifies how an aircraft relates to the local controller.
/// </summary>
public enum AttentionState
{
    /// <summary>
    /// The aircraft is of no concern to the local controller.
    /// </summary>
    Unconcerned = 0,

    /// <summary>
    /// The aircraft is in the local controller's notification area, or their airspace under another controller.
    /// </summary>
    Notified = 1,

    /// <summary>
    /// The aircraft is being handed off to the local controller.
    /// </summary>
    IncomingTransfer = 2,

    /// <summary>
    /// The local controller tracks the aircraft.
    /// </summary>
    Assumed = 3,

    /// <summary>
    /// The local controller is handing the aircraft off.
    /// </summary>
    OutgoingTransfer = 4,

    /// <summary>
    /// The aircraft is in the local controller's airspace and tracked by no one.
    /// </summary>
    Intruder = 5,

    /// <summary>
    /// The aircraft's path will or should enter the local controller's airspace.
    /// </summary>
    Concerned = 6,

    /// <summary>
    /// The local controller handed the aircraft off, the next controller accepted it, and it is still inside the local controller's airspace.
    /// </summary>
    Redundant = 7,
}

/// <summary>
/// Specifies an aircraft's ground status.
/// </summary>
public enum GroundStatus
{
    /// <summary>
    /// No ground status is set.
    /// </summary>
    None = 0,

    /// <summary>
    /// The aircraft is cleared to start up.
    /// </summary>
    Start = 1,

    /// <summary>
    /// The aircraft is cleared to push back.
    /// </summary>
    Push = 2,

    /// <summary>
    /// The aircraft is cleared to taxi.
    /// </summary>
    Taxi = 3,

    /// <summary>
    /// The aircraft is departing.
    /// </summary>
    Dep = 4,
}

/// <summary>
/// Represents the controller-assigned data of one aircraft.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="ClearedFlightLevel">The cleared level in feet, or 0 for none, 1 for ILS, 2 for visual.</param>
/// <param name="AssignedDirect">The assigned direct-to waypoint, or empty when none.</param>
/// <param name="AssignedHeading">The assigned heading in degrees, or 0 when none is assigned.</param>
/// <param name="AssignedSpeed">The assigned speed in knots, or 0 when none or a Mach number is assigned.</param>
/// <param name="AssignedMach">The assigned Mach number in hundredths, or 0 when none or a speed in knots is assigned.</param>
/// <param name="AssignedVerticalRate">The assigned vertical rate, or 0 when none is assigned.</param>
/// <param name="AssignedSquawk">The assigned squawk, or empty when none.</param>
/// <param name="Scratchpad">The scratchpad text.</param>
/// <param name="RawScratchpad">The scratchpad line as typed or received, flags included.</param>
/// <param name="ClearanceIssued">True if the clearance has been issued.</param>
/// <param name="OwnedByMe">True if the local controller tracks the aircraft.</param>
/// <param name="OwnedByCallsign">The tracking controller, or empty when the aircraft is untracked.</param>
/// <param name="FutureOwnerCallsign">The next controller, or empty when none is known.</param>
/// <param name="AssignedSpeedVariance">How the assigned speed is to be read.</param>
/// <param name="AttentionState">How the aircraft relates to the local controller.</param>
/// <param name="GroundStatus">The aircraft's ground status.</param>
public sealed record ControllerDataModel(
    string Callsign,
    int ClearedFlightLevel,
    string AssignedDirect,
    int AssignedHeading,
    int AssignedSpeed,
    int AssignedMach,
    int AssignedVerticalRate,
    string AssignedSquawk,
    string Scratchpad,
    string RawScratchpad,
    bool ClearanceIssued,
    bool OwnedByMe,
    string OwnedByCallsign,
    string FutureOwnerCallsign,
    AssignedSpeedVariance AssignedSpeedVariance,
    AttentionState AttentionState,
    GroundStatus GroundStatus);
