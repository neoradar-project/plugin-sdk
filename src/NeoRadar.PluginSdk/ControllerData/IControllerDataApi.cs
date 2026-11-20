using System.Collections.Generic;

namespace NeoRadar.PluginSdk.ControllerData;

/// <summary>
/// Provides read and write access to the controller-assigned data of each aircraft.
/// </summary>
/// <remarks>Each write operation is rate limited to roughly one call per second per plugin.</remarks>
public interface IControllerDataApi
{
    /// <summary>
    /// Gets the controller data of every aircraft that has any.
    /// </summary>
    IReadOnlyList<ControllerDataModel> GetAll();

    /// <summary>
    /// Gets the controller data of one aircraft.
    /// </summary>
    /// <returns>The data, or null when the callsign has none.</returns>
    ControllerDataModel? GetByCallsign(string callsign);

    /// <summary>
    /// Sets the ground status.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool SetGroundStatus(string callsign, GroundStatus groundStatus);

    /// <summary>
    /// Sets the assigned squawk.
    /// </summary>
    /// <param name="callsign">The aircraft's callsign.</param>
    /// <param name="squawk">Four octal digits, 0 to 7.</param>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool SetAssignedSquawk(string callsign, string squawk);

    /// <summary>
    /// Sets the cleared flight level.
    /// </summary>
    /// <param name="callsign">The aircraft's callsign.</param>
    /// <param name="clearedFlightLevel">The level in feet, such as 35000 rather than 350.</param>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool SetClearedFlightLevel(string callsign, int clearedFlightLevel);

    /// <summary>
    /// Sets the assigned heading and clears any assigned direct-to.
    /// </summary>
    /// <param name="callsign">The aircraft's callsign.</param>
    /// <param name="heading">The heading in degrees, which the host normalises into 1 to 360.</param>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool SetAssignedHeading(string callsign, int heading);

    /// <summary>
    /// Sets the assigned speed and its variance.
    /// </summary>
    /// <remarks>The host ignores a speed outside the ranges given for <paramref name="speed"/>.</remarks>
    /// <param name="callsign">The aircraft's callsign.</param>
    /// <param name="speed">Knots from 100 to 400 or 999 for high speed, or hundredths of Mach from 50 to 100.</param>
    /// <param name="isMach">True when <paramref name="speed"/> is in hundredths of Mach, so 78 is M0.78.</param>
    /// <param name="variance">Whether the speed is exact, a minimum or a maximum.</param>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool SetAssignedSpeed(string callsign, int speed, bool isMach, AssignedSpeedVariance variance);

    /// <summary>
    /// Clears the assigned speed and its variance.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool ClearAssignedSpeed(string callsign);

    /// <summary>
    /// Sets the scratchpad text.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool SetScratchpad(string callsign, string scratchpad);

    /// <summary>
    /// Sets the assigned direct-to waypoint by identifier.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool SetAssignedDirect(string callsign, string waypointIdentifier);

    /// <summary>
    /// Clears the assigned direct-to waypoint.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool ClearAssignedDirect(string callsign);

    /// <summary>
    /// Sets the next controller the aircraft is coordinated to.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool SetNextController(string callsign, string nextControllerCallsign);

    /// <summary>
    /// Sets whether the clearance has been issued.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool SetClearanceIssued(string callsign, bool issued);

    /// <summary>
    /// Starts tracking the aircraft as the local controller.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool AssumeAircraft(string callsign);

    /// <summary>
    /// Stops tracking an aircraft the local controller owns.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool ReleaseAircraft(string callsign);

    /// <summary>
    /// Offers the aircraft to another controller.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool HandoffAircraft(string callsign, string toController);

    /// <summary>
    /// Accepts a handoff offered by <paramref name="fromController"/>.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool AcceptHandoff(string callsign, string fromController);

    /// <summary>
    /// Rejects a handoff offered by <paramref name="fromController"/>.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool RejectHandoff(string callsign, string fromController);

    /// <summary>
    /// Withdraws a handoff the local controller offered to <paramref name="targetController"/>.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool CancelHandoff(string callsign, string targetController);
}
