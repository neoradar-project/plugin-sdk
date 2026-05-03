using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Rendering;

/// <summary>Stable handle returned by every Draw call.</summary>
public readonly record struct ObjectHandle(ulong Value)
{
    public static ObjectHandle Invalid => new(0UL);

    public bool IsValid => Value != 0UL;
}

public enum CoordinateSpace { World = 0, Screen = 1 }

public sealed record Color(byte R = 255, byte G = 255, byte B = 255, byte A = 255);

/// <summary>EPSG:3857 metres (World) or screen pixels (Screen).</summary>
public sealed record Point2D(double X = 0, double Y = 0);

public sealed record ViewportInfo(
    double CenterX,
    double CenterY,
    double Zoom,
    double WidthPixels,
    double HeightPixels,
    double PixelsPerMeter);

public sealed record RenderObjectEvent(
    ObjectHandle Handle,
    double WorldX,
    double WorldY,
    double ScreenX,
    double ScreenY,
    int Button);

public sealed record DrawLineParams(
    IReadOnlyList<Point2D> Points,
    Color Color,
    float Width = 1.4f,
    CoordinateSpace Space = CoordinateSpace.World,
    float MinZoom = 0.0f,
    bool HitTestable = false);

public sealed record DrawFillParams(
    IReadOnlyList<Point2D> Ring,
    Color Color,
    CoordinateSpace Space = CoordinateSpace.World,
    float MinZoom = 0.0f,
    bool HitTestable = false);

public sealed record DrawTextParams(
    Point2D Position,
    string Text,
    Color Color,
    float FontSize = 12.0f,
    CoordinateSpace Space = CoordinateSpace.World,
    float MinZoom = 0.0f);

public sealed record DrawPointParams(
    IReadOnlyList<Point2D> Positions,
    Color Color,
    float Size = 4.0f,
    string Symbol = "circle",
    CoordinateSpace Space = CoordinateSpace.World,
    float MinZoom = 0.0f,
    bool HitTestable = false);
