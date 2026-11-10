namespace NeoRadar.PluginSdk.Rendering;

/// <summary>Stable handle for a hit-tested render object.</summary>
public readonly record struct ObjectHandle(ulong Value)
{
    public static ObjectHandle Invalid => new(0UL);

    public bool IsValid => Value != 0UL;
}

public sealed record ViewportInfo(
    double CenterX,
    double CenterY,
    double Zoom,
    double WidthPixels,
    double HeightPixels,
    double PixelsPerMeter);
