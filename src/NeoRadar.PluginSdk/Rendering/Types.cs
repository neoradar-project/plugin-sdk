namespace NeoRadar.PluginSdk.Rendering;

/// <summary>
/// Represents the camera of a map screen at the time it was read.
/// </summary>
/// <param name="CenterX">The X coordinate of the view centre in EPSG:3857 metres.</param>
/// <param name="CenterY">The Y coordinate of the view centre in EPSG:3857 metres.</param>
/// <param name="Zoom">The web-mercator zoom level.</param>
/// <param name="WidthPixels">The width of the view in device-independent pixels, the unit of every screen coordinate.</param>
/// <param name="HeightPixels">The height of the view in device-independent pixels.</param>
/// <param name="PixelsPerMeter">The number of device-independent pixels per EPSG:3857 metre at the current zoom.</param>
public sealed record ViewportInfo(
    double CenterX,
    double CenterY,
    double Zoom,
    double WidthPixels,
    double HeightPixels,
    double PixelsPerMeter);
