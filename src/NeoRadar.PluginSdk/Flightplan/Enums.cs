namespace NeoRadar.PluginSdk.Flightplan;

/// <summary>
/// Specifies the voice capability a pilot filed.
/// </summary>
public enum VoiceType
{
    /// <summary>
    /// The voice capability is not known.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The pilot can send and receive voice.
    /// </summary>
    Full = 1,

    /// <summary>
    /// The pilot can receive voice and replies by text.
    /// </summary>
    Receive = 2,

    /// <summary>
    /// The pilot uses text only.
    /// </summary>
    Text = 3
}

/// <summary>
/// Specifies the flight rules a route leg is flown under.
/// </summary>
public enum FlightRule
{
    /// <summary>
    /// Instrument flight rules.
    /// </summary>
    IFR,

    /// <summary>
    /// Visual flight rules.
    /// </summary>
    VFR,

    /// <summary>
    /// Instrument flight rules first, then a change of flight rules, as ICAO Doc 4444 Item 8 letter Y files it.
    /// </summary>
    Yankee,

    /// <summary>
    /// Visual flight rules first, then a change of flight rules, as ICAO Doc 4444 Item 8 letter Z files it.
    /// </summary>
    Zulu
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
    /// No procedure was found for the route.
    /// </summary>
    NO_PROCEDURE_FOUND,

    /// <summary>
    /// The runway is not valid.
    /// </summary>
    INVALID_RUNWAY,

    /// <summary>
    /// The data is not valid.
    /// </summary>
    INVALID_DATA,

    /// <summary>
    /// The airport is not known.
    /// </summary>
    UNKNOWN_AIRPORT,

    /// <summary>
    /// The navaid is not known.
    /// </summary>
    UNKNOWN_NAVAID,

    /// <summary>
    /// The airway is not known.
    /// </summary>
    UNKNOWN_AIRWAY,

    /// <summary>
    /// The token is not in a recognised format.
    /// </summary>
    INVALID_TOKEN_FORMAT,

    /// <summary>
    /// The airway token is not in a recognised format.
    /// </summary>
    INVALID_AIRWAY_FORMAT,

    /// <summary>
    /// The airway is flown against its permitted direction.
    /// </summary>
    INVALID_AIRWAY_DIRECTION,

    /// <summary>
    /// The fix is not on the airway.
    /// </summary>
    AIRWAY_FIX_NOT_FOUND,

    /// <summary>
    /// The planned level is below the airway's minimum.
    /// </summary>
    INSUFFICIENT_FLIGHT_LEVEL,

    /// <summary>
    /// More than one airway matches the token.
    /// </summary>
    MULTIPLE_AIRWAYS_FOUND,
}

/// <summary>
/// Specifies the unit of a planned speed.
/// </summary>
public enum SpeedUnit
{
    /// <summary>
    /// Knots.
    /// </summary>
    KNOTS,

    /// <summary>
    /// Mach number.
    /// </summary>
    MACH,

    /// <summary>
    /// Kilometres per hour.
    /// </summary>
    KMH
}

/// <summary>
/// Specifies the unit of a distance or planned altitude.
/// </summary>
public enum DistanceUnit
{
    /// <summary>
    /// Nautical miles.
    /// </summary>
    NAUTICAL_MILES,

    /// <summary>
    /// Kilometres.
    /// </summary>
    KILOMETERS,

    /// <summary>
    /// Feet.
    /// </summary>
    FEET,

    /// <summary>
    /// Metres.
    /// </summary>
    METERS
}
