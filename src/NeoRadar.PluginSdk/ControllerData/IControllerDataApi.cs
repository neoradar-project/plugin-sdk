using System.Collections.Generic;

namespace NeoRadar.PluginSdk.ControllerData;

/// <summary>
/// Provides read and write access to the controller-assigned data of each aircraft.
/// </summary>
/// <remarks>Each write verb allows a burst of five calls, then one a second, and at most one call a second for the same aircraft.</remarks>
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
    /// <returns>False if the call was throttled, the user is not connected as ATC, the callsign is unknown, or the status is not a defined value.</returns>
    bool SetGroundStatus(string callsign, GroundStatus groundStatus);

    /// <summary>
    /// Sets the assigned squawk.
    /// </summary>
    /// <param name="callsign">The aircraft's callsign.</param>
    /// <param name="squawk">Four octal digits, 0 to 7.</param>
    /// <returns>False if the call was throttled, the user is not connected as ATC, the callsign is unknown, or the code is not four octal digits.</returns>
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
    /// <param name="heading">The heading in degrees, which the host normalises into 1 to 360, so 0 is north.</param>
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
    /// <returns>False if the call was throttled, the user is not connected as ATC, the callsign is unknown, or the variance is not a defined value.</returns>
    bool SetAssignedSpeed(string callsign, int speed, bool isMach, AssignedSpeedVariance variance);

    /// <summary>
    /// Clears the assigned speed and its variance.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool ClearAssignedSpeed(string callsign);

    /// <summary>
    /// Sets the scratchpad text, read as a scratchpad command line exactly as the controller would type it.
    /// </summary>
    /// <remarks>
    /// A command word does what typing it does, so "H270" assigns a heading and a point's name a direct-to. A clearance or
    /// ground-status word is told to the other controllers as the menu tells it.
    /// </remarks>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool SetScratchpad(string callsign, string scratchpad);

    /// <summary>
    /// Sets the assigned direct-to waypoint by identifier, as the direct-to box does.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, the callsign is unknown, or the identifier names no point on the route or in the navigation data.</returns>
    bool SetAssignedDirect(string callsign, string waypointIdentifier);

    /// <summary>
    /// Clears the assigned direct-to waypoint.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, the callsign is unknown, or another controller tracks the aircraft.</returns>
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
    /// <returns>False if the call was throttled, the user is not connected as ATC, the callsign is unknown, or <paramref name="fromController"/> has no offer to the user pending.</returns>
    bool AcceptHandoff(string callsign, string fromController);

    /// <summary>
    /// Rejects a handoff offered by <paramref name="fromController"/>.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, the callsign is unknown, or <paramref name="fromController"/> has no offer to the user pending, in which case nothing is sent.</returns>
    bool RejectHandoff(string callsign, string fromController);

    /// <summary>
    /// Withdraws a handoff the local controller offered to <paramref name="targetController"/>.
    /// </summary>
    /// <returns>False if the call was throttled, the user is not connected as ATC, or the callsign is unknown.</returns>
    bool CancelHandoff(string callsign, string targetController);

    /// <summary>
    /// Gets the nine flight strip annotations of an aircraft, as EuroScope's flight strip boxes.
    /// </summary>
    /// <remarks>A box no controller has written reads empty.</remarks>
    /// <returns>Nine strings, index 0 to 8, or null when the callsign has no controller data.</returns>
    IReadOnlyList<string>? GetFlightStripAnnotations(string callsign);

    /// <summary>
    /// Sets one flight strip annotation and sends the strip to the user's linked EuroScope or NeoRadar instances, never the network.
    /// </summary>
    /// <remarks>Each box holds at most 50 characters, and a ':' or control character is removed before it is stored. The once-a-second limit is per box, not per aircraft.</remarks>
    /// <param name="callsign">The aircraft's callsign.</param>
    /// <param name="index">The box, 0 to 8.</param>
    /// <param name="text">The annotation; empty clears the box.</param>
    /// <returns>False if the call was throttled, the index is outside 0 to 8, or the callsign has no controller data.</returns>
    bool SetFlightStripAnnotation(string callsign, int index, string text);

    /// <summary>
    /// Sends the aircraft's flight strip to another controller, as EuroScope's strip push does.
    /// </summary>
    /// <remarks>The host also pushes the strip by itself after every handoff the user sends.</remarks>
    /// <returns>False if the call was throttled, the user is not connected as ATC, the callsign is unknown, another controller tracks the aircraft, <paramref name="toController"/> is not a known controller, or the strip holds no box yet.</returns>
    bool PushFlightStrip(string callsign, string toController);
}
