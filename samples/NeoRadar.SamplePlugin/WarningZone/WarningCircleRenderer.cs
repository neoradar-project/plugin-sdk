using System;
using System.Numerics;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.WarningZone;

// A decorative zone: not hit-testable, so a press inside it reaches the targets, the map and the client's tools.
internal sealed class WarningCircleRenderer : IPluginScreenRenderer
{
    private const int CircleSegments = 64;
    private const float CircleRadiusMetres = 40_000f;
    private static readonly Vector2 CircleCenter = new(-13_557f, 6_710_614f);
    private const float FillOpacity = 0.35f;

    private readonly ThemeColours _colours;
    private GraphicsObjectRef _ring;
    private int _themeVersion = -1;

    public WarningCircleRenderer(ThemeColours colours)
    {
        _colours = colours;
    }

    public void OnFrameUpdate(IPluginGraphics g, FrameInfo info)
    {
        ThemeSnapshot theme = _colours.Current;
        if (theme.Version == _themeVersion) return;
        _themeVersion = theme.Version;

        RgbaColor fill = theme.Warning with { A = FillOpacity };
        if (_ring.IsValid)
        {
            g.SetColor(_ring, fill);
            return;
        }

        _ring = g.AddWorldPolygon(new[] { BuildCircle(CircleCenter, CircleRadiusMetres) }, fill);
        g.SetHitTestVisible(_ring, false);
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
