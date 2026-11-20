using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Flightplan;

/// <summary>
/// Represents a point on the earth.
/// </summary>
/// <param name="Latitude">The latitude in decimal degrees.</param>
/// <param name="Longitude">The longitude in decimal degrees.</param>
public sealed record Position(double Latitude, double Longitude);

/// <summary>
/// Represents a named point a route can pass through.
/// </summary>
public record Waypoint
{
    /// <summary>
    /// Gets the kind of waypoint.
    /// </summary>
    public WaypointType Type { get; init; } = WaypointType.UNKNOWN;

    /// <summary>
    /// Gets the waypoint's identifier, or empty for an unset waypoint.
    /// </summary>
    public string Identifier { get; init; } = string.Empty;

    /// <summary>
    /// Gets the waypoint's position.
    /// </summary>
    public Position PositionValue { get; init; } = new(0, 0);

    /// <summary>
    /// Gets the navaid frequency in hertz, or 0 when there is none.
    /// </summary>
    public int FrequencyHz { get; init; }
}

/// <summary>
/// Represents the level and speed planned at a route waypoint.
/// </summary>
/// <param name="PlannedAltitude">The planned altitude, or null when none is planned.</param>
/// <param name="PlannedSpeed">The planned speed, or null when none is planned.</param>
/// <param name="AltitudeUnit">The unit of <paramref name="PlannedAltitude"/>; a flight level arrives converted to feet.</param>
/// <param name="SpeedUnit">The unit of <paramref name="PlannedSpeed"/>.</param>
public sealed record PlannedAltitudeAndSpeed(
    int? PlannedAltitude,
    int? PlannedSpeed,
    DistanceUnit AltitudeUnit = DistanceUnit.FEET,
    SpeedUnit SpeedUnit = SpeedUnit.KNOTS);

/// <summary>
/// Represents a waypoint as it appears in a parsed route.
/// </summary>
public sealed record RouteWaypoint : Waypoint
{
    /// <summary>
    /// Gets the level and speed planned at the waypoint, or null when none are planned.
    /// </summary>
    public PlannedAltitudeAndSpeed? PlannedPosition { get; init; }

    /// <summary>
    /// Gets the flight rules the route is flown under at the waypoint.
    /// </summary>
    public FlightRule FlightRule { get; init; } = FlightRule.IFR;
}

/// <summary>
/// Represents a problem found while parsing a route.
/// </summary>
/// <param name="Type">The kind of problem.</param>
/// <param name="Message">A readable description of the problem.</param>
/// <param name="TokenIndex">The index of the offending token in the route string.</param>
/// <param name="Token">The offending token.</param>
/// <param name="Level">How serious the problem is.</param>
public sealed record ParsingError(
    ParsingErrorType Type,
    string Message,
    int TokenIndex,
    string Token,
    ParsingErrorLevel Level);

/// <summary>
/// Represents one leg of a parsed route.
/// </summary>
/// <param name="From">The waypoint the leg starts at.</param>
/// <param name="To">The waypoint the leg ends at.</param>
/// <param name="Airway">The airway the leg follows, or "DCT" for a direct leg.</param>
/// <param name="Heading">The leg's bearing in degrees.</param>
/// <param name="MinimumLevel">The leg's minimum level, or -1 when none is known.</param>
public sealed record ParsedRouteSegment(
    RouteWaypoint From,
    RouteWaypoint To,
    string Airway,
    int Heading = 0,
    int MinimumLevel = -1);

/// <summary>
/// Represents a flight plan's route, parsed and with any direct-to applied.
/// </summary>
public sealed record Route
{
    /// <summary>
    /// Gets the route string as filed.
    /// </summary>
    public string RawRoute { get; init; } = string.Empty;

    /// <summary>
    /// Gets the route's waypoints in order.
    /// </summary>
    public IReadOnlyList<RouteWaypoint> Waypoints { get; init; } = System.Array.Empty<RouteWaypoint>();

    /// <summary>
    /// Gets the legs of the parsed route.
    /// </summary>
    public IReadOnlyList<ParsedRouteSegment> Segments { get; init; } = System.Array.Empty<ParsedRouteSegment>();

    /// <summary>
    /// Gets the legs the aircraft is expected to fly, with any direct-to applied.
    /// </summary>
    public IReadOnlyList<ParsedRouteSegment> ExplicitSegments { get; init; } = System.Array.Empty<ParsedRouteSegment>();

    /// <summary>
    /// Gets the problems found while parsing the route.
    /// </summary>
    public IReadOnlyList<ParsingError> Errors { get; init; } = System.Array.Empty<ParsingError>();

    /// <summary>
    /// Gets the expected legs as they stood before the current direct-to was applied.
    /// </summary>
    public IReadOnlyList<ParsedRouteSegment> OriginalSegments { get; init; } = System.Array.Empty<ParsedRouteSegment>();

    /// <summary>
    /// Gets a value indicating whether a direct-to is applied.
    /// </summary>
    public bool HasDirectApplied { get; init; }

    /// <summary>
    /// Gets the direct-to waypoint, or an empty waypoint when none is applied.
    /// </summary>
    public Waypoint CurrentDirectWaypoint { get; init; } = new();

    /// <summary>
    /// Gets the assigned SID, or empty when none is assigned.
    /// </summary>
    public string Sid { get; init; } = string.Empty;

    /// <summary>
    /// Gets the assigned STAR, or empty when none is assigned.
    /// </summary>
    public string Star { get; init; } = string.Empty;

    /// <summary>
    /// Gets the suggested SID, or empty when there is none.
    /// </summary>
    public string SuggestedSid { get; init; } = string.Empty;

    /// <summary>
    /// Gets the suggested STAR, or empty when there is none.
    /// </summary>
    public string SuggestedStar { get; init; } = string.Empty;

    /// <summary>
    /// Gets the assigned departure runway, or empty when none is assigned.
    /// </summary>
    public string DepRunway { get; init; } = string.Empty;

    /// <summary>
    /// Gets the assigned arrival runway, or empty when none is assigned.
    /// </summary>
    public string ArrRunway { get; init; } = string.Empty;

    /// <summary>
    /// Gets the suggested departure runway, or empty when there is none.
    /// </summary>
    public string SuggestedDepRunway { get; init; } = string.Empty;

    /// <summary>
    /// Gets the suggested arrival runway, or empty when there is none.
    /// </summary>
    public string SuggestedArrRunway { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the flight plan has been amended since it was filed.
    /// </summary>
    public bool IsAmended { get; init; }
}

/// <summary>
/// Represents a filed flight plan.
/// </summary>
public sealed record Flightplan
{
    /// <summary>
    /// Gets the aircraft's callsign.
    /// </summary>
    public string Callsign { get; init; } = string.Empty;

    /// <summary>
    /// Gets the filed flight rules.
    /// </summary>
    public string FlightRule { get; init; } = string.Empty;

    /// <summary>
    /// Gets the aircraft type field as filed, type, wake category and equipment together.
    /// </summary>
    public string RawType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the aircraft type designator.
    /// </summary>
    public string AcType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the ICAO equipment code.
    /// </summary>
    public string Equipment { get; init; } = string.Empty;

    /// <summary>
    /// Gets the ICAO wake turbulence category.
    /// </summary>
    public string WakeCategory { get; init; } = string.Empty;

    /// <summary>
    /// Gets the ICAO transponder equipment code.
    /// </summary>
    public string TransponderEquipment { get; init; } = string.Empty;

    /// <summary>
    /// Gets the departure airport's ICAO code.
    /// </summary>
    public string Origin { get; init; } = string.Empty;

    /// <summary>
    /// Gets the destination airport's ICAO code.
    /// </summary>
    public string Destination { get; init; } = string.Empty;

    /// <summary>
    /// Gets the alternate airport's ICAO code.
    /// </summary>
    public string Alternate { get; init; } = string.Empty;

    /// <summary>
    /// Gets the departure airport as a waypoint, or null when it is not known.
    /// </summary>
    public Waypoint? OriginWaypoint { get; init; }

    /// <summary>
    /// Gets the destination airport as a waypoint, or null when it is not known.
    /// </summary>
    public Waypoint? DestinationWaypoint { get; init; }

    /// <summary>
    /// Gets the alternate airport as a waypoint, or null when it is not known.
    /// </summary>
    public Waypoint? AlternateWaypoint { get; init; }

    /// <summary>
    /// Gets the parsed route.
    /// </summary>
    public Route Route { get; init; } = new();

    /// <summary>
    /// Gets the planned cruising altitude in feet, or 0 when the filed level could not be read.
    /// </summary>
    public int PlannedAltitude { get; init; }

    /// <summary>
    /// Gets the planned true airspeed in knots.
    /// </summary>
    public int PlannedTas { get; init; }

    /// <summary>
    /// Gets the hours part of the estimated time en route, as filed.
    /// </summary>
    public string FlightTimeHours { get; init; } = string.Empty;

    /// <summary>
    /// Gets the minutes part of the estimated time en route, as filed.
    /// </summary>
    public string FlightTimeMinutes { get; init; } = string.Empty;

    /// <summary>
    /// Gets the hours part of the fuel endurance, as filed.
    /// </summary>
    public string FuelTimeHours { get; init; } = string.Empty;

    /// <summary>
    /// Gets the minutes part of the fuel endurance, as filed.
    /// </summary>
    public string FuelTimeMinutes { get; init; } = string.Empty;

    /// <summary>
    /// Gets the estimated off-block time, as filed.
    /// </summary>
    public string Eobt { get; init; } = string.Empty;

    /// <summary>
    /// Gets the actual off-block time, as filed.
    /// </summary>
    public string Aobt { get; init; } = string.Empty;

    /// <summary>
    /// Gets the filed remarks.
    /// </summary>
    public string Remarks { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the flight plan is valid.
    /// </summary>
    public bool IsValid { get; init; } = true;

    /// <summary>
    /// Gets the pilot's voice capability.
    /// </summary>
    public VoiceType VoiceType { get; init; } = VoiceType.Unknown;
}
