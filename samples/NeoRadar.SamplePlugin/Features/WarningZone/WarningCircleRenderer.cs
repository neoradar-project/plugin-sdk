using System;
using System.Numerics;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Rendering;

namespace NeoRadar.SamplePlugin.Features.WarningZone;

internal sealed class WarningCircleRenderer : IPluginScreenRenderer
{
    private const int CircleSegments = 64;
    private const float CircleRadiusMetres = 40_000f;
    private static readonly Vector2 CircleCenter = new(-13_557f, 6_710_614f);
    private static readonly RgbaColor FillColor = new(1f, 0.65f, 0.05f, 0.35f);

    private readonly ICoreApi _core;
    private GraphicsObjectRef _ring;
    private bool _seeded;

    public WarningCircleRenderer(ICoreApi core) { _core = core; }

    /// <summary>
    /// Host destroys the GraphicsContainer + adapter on disable; clear the
    /// cached ring ref so re-enable re-seeds the world geometry.
    /// </summary>
    public void OnDisabled()
    {
        _ring = GraphicsObjectRef.Invalid;
        _seeded = false;
    }

    public void OnFrameUpdate(IPluginGraphics g, FrameInfo info)
    {
        if (_seeded) return;
        _seeded = true;
        _ring = g.AddWorldPolygon(new[] { BuildCircle(CircleCenter, CircleRadiusMetres) }, FillColor);
    }

    public bool OnPointerPressed(PointerEvent e)
    {
        if (e.Hit is null || !e.Hit.Value.Equals(_ring)) return false;
        SamplePlugin.ScreenPointerCount++;
        _core.Logger.Info($"Warning zone clicked at world ({e.World?.X:F0}, {e.World?.Y:F0})");
        return true;
    }

    private static Vector2[] BuildCircle(Vector2 center, float radius)
    {
        Vector2[] points = new Vector2[CircleSegments + 1];
        for (int i = 0; i < CircleSegments; i++)
        {
            float angle = MathF.PI * 2f * i / CircleSegments;
            points[i] = new Vector2(
                center.X + MathF.Cos(angle) * radius,
                center.Y + MathF.Sin(angle) * radius);
        }
        points[CircleSegments] = points[0];
        return points;
    }
}
