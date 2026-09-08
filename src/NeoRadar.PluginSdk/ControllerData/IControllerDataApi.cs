using System.Collections.Generic;

namespace NeoRadar.PluginSdk.ControllerData;

/// <summary>
/// Read and write the controller-assigned state block. Every write is rate limited per operation,
/// roughly one call per second per plugin, and returns false when it is throttled, when the user
/// is not connected as ATC, or when the callsign is unknown.
/// </summary>
public interface IControllerDataApi
{
    IReadOnlyList<ControllerDataModel> GetAll();

    ControllerDataModel? GetByCallsign(string callsign);

    /// <summary>Set ground status. Rate-limited to roughly one call per second per plugin; excess calls return false.</summary>
    bool SetGroundStatus(string callsign, GroundStatus groundStatus);

    /// <summary>
    /// Set the assigned squawk, four octal digits (0-7). Rate-limited to roughly one call per
    /// second per plugin; excess calls return false, as does an unknown callsign or a user who is
    /// not connected as ATC.
    /// </summary>
    bool SetAssignedSquawk(string callsign, string squawk);

    /// <summary>
    /// Set the cleared flight level in FEET (35000, not 350). Rate-limited to roughly one call per
    /// second per plugin; excess calls return false, as does an unknown callsign or a user who is
    /// not connected as ATC.
    /// </summary>
    bool SetClearedFlightLevel(string callsign, int clearedFlightLevel);

    /// <summary>
    /// Set the assigned heading in degrees, normalised into 1 to 360 by the host. Clears any
    /// assigned direct-to. Rate-limited to roughly one call per second per plugin; excess calls
    /// return false, as does an unknown callsign or a user who is not connected as ATC.
    /// </summary>
    bool SetAssignedHeading(string callsign, int heading);

    /// <summary>
    /// Set the assigned speed: knots (100 to 400, or 999 for high speed) when
    /// <paramref name="isMach"/> is false, hundredths of Mach (50 to 100, so 78 is M0.78) when it
    /// is true. The host ignores a value outside those ranges. Rate-limited to roughly one call
    /// per second per plugin; excess calls return false, as does an unknown callsign or a user who
    /// is not connected as ATC.
    /// </summary>
    bool SetAssignedSpeed(string callsign, int speed, bool isMach, AssignedSpeedVariance variance);

    /// <summary>
    /// Clear the assigned speed and its variance. Rate-limited to roughly one call per second per
    /// plugin; excess calls return false, as does an unknown callsign or a user who is not
    /// connected as ATC.
    /// </summary>
    bool ClearAssignedSpeed(string callsign);

    /// <summary>
    /// Set the scratchpad text. Rate-limited to roughly one call per second per plugin; excess
    /// calls return false, as does an unknown callsign or a user who is not connected as ATC.
    /// </summary>
    bool SetScratchpad(string callsign, string scratchpad);

    /// <summary>
    /// Set the assigned direct-to waypoint by identifier. Rate-limited to roughly one call per
    /// second per plugin; excess calls return false, as does an unknown callsign or a user who is
    /// not connected as ATC.
    /// </summary>
    bool SetAssignedDirect(string callsign, string waypointIdentifier);

    /// <summary>
    /// Clear the assigned direct-to waypoint. Rate-limited to roughly one call per second per
    /// plugin; excess calls return false, as does an unknown callsign or a user who is not
    /// connected as ATC.
    /// </summary>
    bool ClearAssignedDirect(string callsign);

    /// <summary>
    /// Set the next controller the aircraft is coordinated to, by callsign. Rate-limited to
    /// roughly one call per second per plugin; excess calls return false, as does an unknown
    /// callsign or a user who is not connected as ATC.
    /// </summary>
    bool SetNextController(string callsign, string nextControllerCallsign);

    /// <summary>
    /// Set the clearance-issued flag. Rate-limited to roughly one call per second per plugin;
    /// excess calls return false, as does an unknown callsign or a user who is not connected as ATC.
    /// </summary>
    bool SetClearanceIssued(string callsign, bool issued);

    /// <summary>
    /// Start tracking the aircraft as the local controller. Rate-limited to roughly one call per
    /// second per plugin; excess calls return false, as does an unknown callsign or a user who is
    /// not connected as ATC.
    /// </summary>
    bool AssumeAircraft(string callsign);

    /// <summary>
    /// Drop tracking of an aircraft the local controller owns. Rate-limited to roughly one call
    /// per second per plugin; excess calls return false, as does an unknown callsign or a user who
    /// is not connected as ATC.
    /// </summary>
    bool ReleaseAircraft(string callsign);

    /// <summary>
    /// Offer the aircraft to another controller by callsign. Rate-limited to roughly one call per
    /// second per plugin; excess calls return false, as does an unknown callsign or a user who is
    /// not connected as ATC.
    /// </summary>
    bool HandoffAircraft(string callsign, string toController);

    /// <summary>
    /// Accept a handoff offered by <paramref name="fromController"/>. Rate-limited to roughly one
    /// call per second per plugin; excess calls return false, as does an unknown callsign or a
    /// user who is not connected as ATC.
    /// </summary>
    bool AcceptHandoff(string callsign, string fromController);

    /// <summary>
    /// Reject a handoff offered by <paramref name="fromController"/>. Rate-limited to roughly one
    /// call per second per plugin; excess calls return false, as does an unknown callsign or a
    /// user who is not connected as ATC.
    /// </summary>
    bool RejectHandoff(string callsign, string fromController);

    /// <summary>
    /// Withdraw a handoff the local controller offered to <paramref name="targetController"/>.
    /// Rate-limited to roughly one call per second per plugin; excess calls return false, as does
    /// an unknown callsign or a user who is not connected as ATC.
    /// </summary>
    bool CancelHandoff(string callsign, string targetController);
}
