using System;

namespace NeoRadar.PluginSdk.Aircraft;

/// <summary>
/// Represents an aircraft's reported position and motion.
/// </summary>
public sealed record Position
{
    /// <summary>
    /// Gets the latitude in decimal degrees.
    /// </summary>
    public double Latitude { get; init; }

    /// <summary>
    /// Gets the longitude in decimal degrees.
    /// </summary>
    public double Longitude { get; init; }

    /// <summary>
    /// Gets the radar altitude in feet, the pressure altitude rounded to the nearest 25.
    /// </summary>
    public int Altitude { get; init; }

    /// <summary>
    /// Gets the height above ground level in feet.
    /// </summary>
    public double Agl { get; init; }

    /// <summary>
    /// Gets the true altitude in feet.
    /// </summary>
    public double TrueAltitude { get; init; }

    /// <summary>
    /// Gets the pressure altitude in feet.
    /// </summary>
    public double PressureAltitude { get; init; }

    /// <summary>
    /// Gets the ground speed in knots.
    /// </summary>
    public int GroundSpeed { get; init; }

    /// <summary>
    /// Gets the heading the pilot client reports, in degrees.
    /// </summary>
    public int ReportedHeading { get; init; }

    /// <summary>
    /// Gets the track over the ground in degrees.
    /// </summary>
    public int TrackHeading { get; init; }

    /// <summary>
    /// Gets the vertical speed in feet per minute.
    /// </summary>
    public int VerticalSpeed { get; init; }

    /// <summary>
    /// Gets the vertical trend: 1 climbing, -1 descending, 0 level.
    /// </summary>
    public int VerticalTrend { get; init; }

    /// <summary>
    /// Gets the pitch in degrees.
    /// </summary>
    public double Pitch { get; init; }

    /// <summary>
    /// Gets the bank in degrees.
    /// </summary>
    public double Bank { get; init; }

    /// <summary>
    /// Gets the eastward velocity in metres per second.
    /// </summary>
    public double VelX { get; init; }

    /// <summary>
    /// Gets the upward velocity in metres per second.
    /// </summary>
    public double VelY { get; init; }

    /// <summary>
    /// Gets the northward velocity in metres per second.
    /// </summary>
    public double VelZ { get; init; }

    /// <summary>
    /// Gets the heading rate in radians per second, clockwise positive, or 0 when none was reported.
    /// </summary>
    public double VelH { get; init; }

    /// <summary>
    /// Gets a value indicating whether the aircraft is on the ground.
    /// </summary>
    public bool OnGround { get; init; }

    /// <summary>
    /// Gets a value indicating whether the aircraft is stationary.
    /// </summary>
    public bool Stopped { get; init; } = true;

    /// <summary>
    /// Gets the transponder mode at the time of the report.
    /// </summary>
    public TransponderMode TransponderMode { get; init; } = TransponderMode.Standby;

    /// <summary>
    /// Gets the time the position was reported, in UTC, or the default value when the host has none.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }
}
