namespace NeoRadar.PluginSdk.Flightplan;

/// <summary>
/// Specifies the voice capability a pilot filed.
/// </summary>
public enum VoiceType
{
    /// <summary>
    /// The pilot can send and receive voice, which is also what a plan with no voice marker reads as.
    /// </summary>
    Full = 0,

    /// <summary>
    /// The pilot can receive voice and replies by text.
    /// </summary>
    Receive = 1,

    /// <summary>
    /// The pilot uses text only.
    /// </summary>
    Text = 2
}

/// <summary>
/// Specifies the flight rules a route leg is flown under.
/// </summary>
/// <remarks>A plan that changes rules (Y or Z) gives each leg its own rule, and the plan's filed rules are <see cref="Flightplan.FlightRule"/>.</remarks>
public enum FlightRule
{
    /// <summary>
    /// Instrument flight rules.
    /// </summary>
    IFR,

    /// <summary>
    /// Visual flight rules.
    /// </summary>
    VFR
}

/// <summary>
/// Specifies the kind of a waypoint.
/// </summary>
public enum WaypointType
{
    /// <summary>
    /// A named fix.
    /// </summary>
    FIX,

    /// <summary>
    /// A VOR.
    /// </summary>
    VOR,

    /// <summary>
    /// A DME.
    /// </summary>
    DME,

    /// <summary>
    /// A co-located VOR and DME.
    /// </summary>
    VORDME,

    /// <summary>
    /// A co-located NDB and DME.
    /// </summary>
    NDBDME,

    /// <summary>
    /// A VORTAC.
    /// </summary>
    VORTAC,

    /// <summary>
    /// A TACAN.
    /// </summary>
    TACAN,

    /// <summary>
    /// An NDB.
    /// </summary>
    NDB,

    /// <summary>
    /// An airport.
    /// </summary>
    AIRPORT,

    /// <summary>
    /// A point given as latitude and longitude.
    /// </summary>
    LATLON,

    /// <summary>
    /// The kind is not known.
    /// </summary>
    UNKNOWN,
}

/// <summary>
/// Specifies how serious a route parsing error is.
/// </summary>
public enum ParsingErrorLevel
{
    /// <summary>
    /// An informational message.
    /// </summary>
    INFO,

    /// <summary>
    /// An error in the route.
    /// </summary>
    PARSE_ERROR
}

/// <summary>
/// Specifies the kind of a route parsing error.
/// </summary>
public enum ParsingErrorType
{
    /// <summary>
    /// The route is empty.
    /// </summary>
    ROUTE_EMPTY,

    /// <summary>
    /// The procedure does not serve the assigned runway.
    /// </summary>
    PROCEDURE_RUNWAY_MISMATCH,

    /// <summary>
    /// The procedure does not belong to the airport.
    /// </summary>
    PROCEDURE_AIRPORT_MISMATCH,

    /// <summary>
    /// The procedure is not known.
    /// </summary>
    UNKNOWN_PROCEDURE,

    /// <summary>
    /// The waypoint is not known.
    /// </summary>
    UNKNOWN_WAYPOINT,

    /// <summary>
    /// The runway is not valid.
    /// </summary>
    INVALID_RUNWAY,

    /// <summary>
    /// The data is not valid, or the server reported the problem as text only.
    /// </summary>
    INVALID_DATA,

    /// <summary>
    /// The airway is not known.
    /// </summary>
    UNKNOWN_AIRWAY,

    /// <summary>
    /// The token is not in a recognised format.
    /// </summary>
    INVALID_TOKEN_FORMAT,

    /// <summary>
    /// The airway is known but no path along it joins the fixes either side of it.
    /// </summary>
    AIRWAY_FIX_NOT_FOUND,
}
