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
    /// <remarks>
    /// A route point takes its kind from the airport or navaid of its identifier nearest its position, within 2 NM, in the
    /// client's navdata. A coordinate is <see cref="WaypointType.LATLON"/> and anything else <see cref="WaypointType.FIX"/>.
    /// </remarks>
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
    /// Gets the navaid frequency in hertz, from the client's navdata, or 0 when the point is not a navaid.
    /// </summary>
    public int FrequencyHz { get; init; }
}

/// <summary>
/// Represents a waypoint as it appears in a parsed route.
/// </summary>
public sealed record RouteWaypoint : Waypoint
{
    /// <summary>
    /// Gets the altitude in feet planned at the waypoint, or null when none is planned.
    /// </summary>
    /// <remarks>A filed flight level arrives converted to feet.</remarks>
    public int? PlannedAltitude { get; init; }

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
/// <param name="TokenIndex">The index of the offending token in the route string, or -1 when the problem is tied to no token.</param>
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
/// <remarks>An airway run in <see cref="Route.Segments"/> carries the highest minimum level of its hops.</remarks>
/// <param name="From">The waypoint the leg starts at.</param>
/// <param name="To">The waypoint the leg ends at.</param>
/// <param name="Airway">The airway the leg follows, or "DCT" for a direct leg.</param>
/// <param name="Heading">The leg's true initial bearing in whole degrees, 0 to 359.</param>
/// <param name="MinimumLevel">The leg's minimum level in feet, or -1 when none is known.</param>
public sealed record ParsedRouteSegment(
    RouteWaypoint From,
    RouteWaypoint To,
    string Airway,
    int Heading = 0,
    int MinimumLevel = -1);

/// <summary>
/// Represents a flight plan's route, parsed and with any direct-to applied.
/// </summary>
/// <remarks>
/// The NeoRadar server parses the route, so without one every parsed field (the waypoints, legs, errors, procedures and
/// runways) stays empty. With one, a new route string arrives first with the old or no parsed form, and the parsed form
/// follows in a later update.
/// </remarks>
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
    /// Gets the legs of the parsed route as filed, one per airway or direct, without procedure legs.
    /// </summary>
    /// <remarks>While a direct-to is applied, only the legs from the direct point on are kept.</remarks>
    public IReadOnlyList<ParsedRouteSegment> Segments { get; init; } = System.Array.Empty<ParsedRouteSegment>();

    /// <summary>
    /// Gets the legs the aircraft is expected to fly, with any direct-to applied.
    /// </summary>
    /// <remarks>While a direct-to is applied, only the legs from the direct point on are kept.</remarks>
    public IReadOnlyList<ParsedRouteSegment> ExplicitSegments { get; init; } = System.Array.Empty<ParsedRouteSegment>();

    /// <summary>
    /// Gets the problems found while parsing the route.
    /// </summary>
    public IReadOnlyList<ParsingError> Errors { get; init; } = System.Array.Empty<ParsingError>();

    /// <summary>
    /// Gets a value indicating whether a direct-to is applied.
    /// </summary>
    public bool HasDirectApplied { get; init; }

    /// <summary>
    /// Gets the direct-to waypoint, or an empty waypoint when none is applied.
    /// </summary>
    /// <remarks>Its position and kind are those of the first leg that starts at it.</remarks>
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
    /// Gets the ICAO equipment code, or empty when the type field files none.
    /// </summary>
    public string Equipment { get; init; } = string.Empty;

    /// <summary>
    /// Gets the ICAO wake turbulence category, or empty when the type field files none.
    /// </summary>
    public string WakeCategory { get; init; } = string.Empty;

    /// <summary>
    /// Gets the ICAO transponder equipment code, or empty when the type field files none.
    /// </summary>
    /// <remarks>In the FAA form, for example H/B744/L, this is the FAA equipment suffix.</remarks>
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
    /// Gets a value indicating whether the flight plan exists and is filed.
    /// </summary>
    /// <remarks>It says nothing about the route, whose problems are in <see cref="Route.Errors"/>.</remarks>
    public bool IsValid { get; init; } = true;

    /// <summary>
    /// Gets the pilot's voice capability.
    /// </summary>
    /// <remarks>A plan whose remarks carry no /V/, /R/ or /T/ reads <see cref="VoiceType.Full"/>, as VATSIM treats it.</remarks>
    public VoiceType VoiceType { get; init; } = VoiceType.Full;
}
