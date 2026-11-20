namespace NeoRadar.PluginSdk.Rendering;

/// <summary>
/// Represents a stable handle for a hit-tested render object.
/// </summary>
/// <param name="Value">The host's identifier for the object; 0 is invalid.</param>
public readonly record struct ObjectHandle(ulong Value)
{
    /// <summary>
    /// Gets the handle that refers to no object.
    /// </summary>
    public static ObjectHandle Invalid => new(0UL);

    /// <summary>
    /// Gets a value indicating whether the handle refers to an object.
    /// </summary>
    public bool IsValid => Value != 0UL;
}

/// <summary>
/// Represents the camera of a map screen at the time it was read.
/// </summary>
/// <param name="CenterX">The X coordinate of the view centre in EPSG:3857 metres.</param>
/// <param name="CenterY">The Y coordinate of the view centre in EPSG:3857 metres.</param>
/// <param name="Zoom">The web-mercator zoom level.</param>
/// <param name="WidthPixels">The width of the view in pixels.</param>
/// <param name="HeightPixels">The height of the view in pixels.</param>
/// <param name="PixelsPerMeter">The number of pixels per EPSG:3857 metre at the current zoom.</param>
public sealed record ViewportInfo(
    double CenterX,
    double CenterY,
    double Zoom,
    double WidthPixels,
    double HeightPixels,
    double PixelsPerMeter);
