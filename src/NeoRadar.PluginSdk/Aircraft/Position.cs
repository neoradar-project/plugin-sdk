using System;

namespace NeoRadar.PluginSdk.Aircraft;

public sealed record Position
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    public int Altitude { get; init; }
    public double Agl { get; init; }
    public double TrueAltitude { get; init; }
    public double PressureAltitude { get; init; }

    public int GroundSpeed { get; init; }
    public int ReportedHeading { get; init; }
    public int TrackHeading { get; init; }
    public int VerticalSpeed { get; init; }
    public int VerticalTrend { get; init; }

    public double Pitch { get; init; }
    public double Bank { get; init; }

    public double VelX { get; init; }
    public double VelY { get; init; }
    public double VelZ { get; init; }
    public double VelH { get; init; }

    public bool OnGround { get; init; }
    public bool Stopped { get; init; } = true;
    public TransponderMode TransponderMode { get; init; } = TransponderMode.Standby;

    /// <summary>When the position was reported, in UTC; the default value when the host has none.</summary>
    public DateTimeOffset Timestamp { get; init; }
}
