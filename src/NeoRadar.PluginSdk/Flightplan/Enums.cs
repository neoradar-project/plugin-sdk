namespace NeoRadar.PluginSdk.Flightplan;

public enum VoiceType { Unknown = 0, Full = 1, Receive = 2, Text = 3 }

public enum FlightRule { IFR, VFR, DVFR, SVFR }

public enum WaypointType
{
    FIX,
    VOR,
    DME,
    VORDME,
    NDBDME,
    VORTAC,
    TACAN,
    NDB,
    AIRPORT,
    LATLON,
    UNKNOWN,
}

public enum ParsingErrorLevel { INFO, PARSE_ERROR }

public enum ParsingErrorType
{
    ROUTE_EMPTY,
    PROCEDURE_RUNWAY_MISMATCH,
    PROCEDURE_AIRPORT_MISMATCH,
    UNKNOWN_PROCEDURE,
    UNKNOWN_WAYPOINT,
    NO_PROCEDURE_FOUND,
    INVALID_RUNWAY,
    INVALID_DATA,
    UNKNOWN_AIRPORT,
    UNKNOWN_NAVAID,
    UNKNOWN_AIRWAY,
    INVALID_TOKEN_FORMAT,
    INVALID_AIRWAY_FORMAT,
    INVALID_AIRWAY_DIRECTION,
    AIRWAY_FIX_NOT_FOUND,
    INSUFFICIENT_FLIGHT_LEVEL,
    MULTIPLE_AIRWAYS_FOUND,
}

public enum SpeedUnit { KNOTS, MACH, KMH }

public enum DistanceUnit { NAUTICAL_MILES, KILOMETERS, FEET, METERS }
