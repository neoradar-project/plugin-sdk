using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Flightplan;

public sealed record Position(double Latitude, double Longitude);

public record Waypoint
{
    public WaypointType Type { get; init; } = WaypointType.UNKNOWN;
    public string Identifier { get; init; } = string.Empty;
    public Position PositionValue { get; init; } = new(0, 0);
    public int FrequencyHz { get; init; }
}

public sealed record PlannedAltitudeAndSpeed(
    int? PlannedAltitude,
    int? PlannedSpeed,
    DistanceUnit AltitudeUnit = DistanceUnit.FEET,
    SpeedUnit SpeedUnit = SpeedUnit.KNOTS);

public sealed record RouteWaypoint : Waypoint
{
    public PlannedAltitudeAndSpeed? PlannedPosition { get; init; }
    public FlightRule FlightRule { get; init; } = FlightRule.IFR;
}

public sealed record ParsingError(
    ParsingErrorType Type,
    string Message,
    int TokenIndex,
    string Token,
    ParsingErrorLevel Level);

public sealed record ParsedRouteSegment(
    RouteWaypoint From,
    RouteWaypoint To,
    string Airway,
    int Heading = 0,
    int MinimumLevel = -1);

public sealed record Route
{
    public string RawRoute { get; init; } = string.Empty;
    public IReadOnlyList<RouteWaypoint> Waypoints { get; init; } = System.Array.Empty<RouteWaypoint>();
    public IReadOnlyList<ParsedRouteSegment> Segments { get; init; } = System.Array.Empty<ParsedRouteSegment>();
    public IReadOnlyList<ParsedRouteSegment> ExplicitSegments { get; init; } = System.Array.Empty<ParsedRouteSegment>();
    public IReadOnlyList<ParsingError> Errors { get; init; } = System.Array.Empty<ParsingError>();
    public IReadOnlyList<ParsedRouteSegment> OriginalSegments { get; init; } = System.Array.Empty<ParsedRouteSegment>();

    public bool HasDirectApplied { get; init; }
    public Waypoint CurrentDirectWaypoint { get; init; } = new();

    public string Sid { get; init; } = string.Empty;
    public string Star { get; init; } = string.Empty;
    public string SuggestedSid { get; init; } = string.Empty;
    public string SuggestedStar { get; init; } = string.Empty;
    public string DepRunway { get; init; } = string.Empty;
    public string ArrRunway { get; init; } = string.Empty;
    public string SuggestedDepRunway { get; init; } = string.Empty;
    public string SuggestedArrRunway { get; init; } = string.Empty;

    public bool IsAmended { get; init; }
}

public sealed record Flightplan
{
    public string Callsign { get; init; } = string.Empty;
    public string FlightRule { get; init; } = string.Empty;
    public string RawType { get; init; } = string.Empty;
    public string AcType { get; init; } = string.Empty;
    public string Equipment { get; init; } = string.Empty;
    public string WakeCategory { get; init; } = string.Empty;
    public string TransponderEquipment { get; init; } = string.Empty;
    public string Origin { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public string Alternate { get; init; } = string.Empty;
    public Waypoint? OriginWaypoint { get; init; }
    public Waypoint? DestinationWaypoint { get; init; }
    public Waypoint? AlternateWaypoint { get; init; }
    public Route Route { get; init; } = new();
    public int PlannedAltitude { get; init; }
    public int PlannedTas { get; init; }
    public string FlightTimeHours { get; init; } = string.Empty;
    public string FlightTimeMinutes { get; init; } = string.Empty;
    public string FuelTimeHours { get; init; } = string.Empty;
    public string FuelTimeMinutes { get; init; } = string.Empty;
    public string Eobt { get; init; } = string.Empty;
    public string Aobt { get; init; } = string.Empty;
    public string Remarks { get; init; } = string.Empty;
    public bool IsValid { get; init; } = true;
    public VoiceType VoiceType { get; init; } = VoiceType.Unknown;
}
