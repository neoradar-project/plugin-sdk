using System.Collections.Generic;
using System.Numerics;
using NeoRadar.PluginSdk.Rendering;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

// Keeps every live object so a test can count what a renderer left on its screen.
internal sealed class RecordingGraphics : IPluginGraphics
{
    private ulong _next;

    public Dictionary<GraphicsObjectRef, RgbaColor> Live { get; } = new();

    public int Mutations { get; private set; }

    public HashSet<GraphicsObjectRef> NotHitTestable { get; } = new();

    public Vector2 ScreenSize { get; set; } = new(800, 600);

    public ViewportInfo? Viewport => null;

    public bool HasCapture { get; private set; }

    public GraphicsObjectRef AddWorldLine(IReadOnlyList<Vector2> points, RgbaColor color, float width = 1.4f) => Add(color);

    public GraphicsObjectRef AddWorldPolyline(IReadOnlyList<IReadOnlyList<Vector2>> polylines, RgbaColor color, float width = 1.4f) => Add(color);

    public GraphicsObjectRef AddWorldPolygon(IReadOnlyList<IReadOnlyList<Vector2>> rings, RgbaColor color) => Add(color);

    public GraphicsObjectRef AddWorldPoint(Vector2 worldPos, PointSymbol symbol, RgbaColor color) => Add(color);

    public GraphicsObjectRef AddWorldText(Vector2 worldPos, string text, RgbaColor color, float fontSize = 12f, TextAlign align = TextAlign.Left) => Add(color);

    public GraphicsObjectRef AddScreenLine(IReadOnlyList<Vector2> points, RgbaColor color, float width = 1.4f) => Add(color);

    public GraphicsObjectRef AddScreenRect(Rect bounds, RgbaColor fillColor) => Add(fillColor);

    public GraphicsObjectRef AddScreenStrokeRect(Rect bounds, RgbaColor color, float width = 1f) => Add(color);

    public GraphicsObjectRef AddScreenPoint(Vector2 screenPos, PointSymbol symbol, RgbaColor color) => Add(color);

    public GraphicsObjectRef AddScreenText(Vector2 anchor, string text, RgbaColor color, float fontSize = 12f, TextAlign align = TextAlign.Left) => Add(color);

    public GraphicsObjectRef AddScreenTextInRect(Rect bounds, string text, RgbaColor color, float fontSize = 12f, TextAlign align = TextAlign.Center, TextVerticalAlign verticalAlign = TextVerticalAlign.Middle) => Add(color);

    public Vector2 MeasureScreenText(string text, float fontSize) => new(text.Length * fontSize * 0.6f, fontSize);

    public bool SetVisible(GraphicsObjectRef r, bool visible) => Mutate(r);

    public bool SetColor(GraphicsObjectRef r, RgbaColor color)
    {
        if (!Live.ContainsKey(r)) return false;
        Live[r] = color;
        return Mutate(r);
    }

    public bool SetTransform(GraphicsObjectRef r, Transform2D transform) => Mutate(r);

    public bool SetMinZoom(GraphicsObjectRef r, float minZoom) => Mutate(r);

    public bool SetHitTestVisible(GraphicsObjectRef r, bool hittable)
    {
        if (hittable) NotHitTestable.Remove(r);
        else NotHitTestable.Add(r);
        return Mutate(r);
    }

    public bool SetText(GraphicsObjectRef r, string text) => Mutate(r);

    public bool SetScreenAnchor(GraphicsObjectRef r, Vector2 anchor) => Mutate(r);

    public bool SetScreenPoints(GraphicsObjectRef r, IReadOnlyList<Vector2> points) => Mutate(r);

    public bool SetScreenSize(GraphicsObjectRef r, float width, float height) => Mutate(r);

    public bool BringToFront(GraphicsObjectRef r) => Mutate(r);

    public bool SendToBack(GraphicsObjectRef r) => Mutate(r);

    public bool Remove(GraphicsObjectRef r)
    {
        Mutations++;
        return Live.Remove(r);
    }

    public void Clear() => Live.Clear();

    public void CapturePointer() => HasCapture = true;

    public void ReleasePointer() => HasCapture = false;

    public void RequestRedraw()
    {
    }

    public Vector2 LatLonToWorld(double latDeg, double lonDeg) => new((float)lonDeg * 111_000f, (float)latDeg * 111_000f);

    public (double Lat, double Lon) WorldToLatLon(Vector2 world) => (world.Y / 111_000.0, world.X / 111_000.0);

    public Vector2 ScreenToWorld(Vector2 screen) => screen;

    public Vector2 WorldToScreen(Vector2 world) => world;

    public void ResetMutations() => Mutations = 0;

    private GraphicsObjectRef Add(RgbaColor color)
    {
        Mutations++;
        GraphicsObjectRef r = new(1UL, ++_next);
        Live[r] = color;
        return r;
    }

    private bool Mutate(GraphicsObjectRef r)
    {
        Mutations++;
        return Live.ContainsKey(r);
    }
}
