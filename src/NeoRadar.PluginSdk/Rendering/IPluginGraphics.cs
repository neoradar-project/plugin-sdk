using System;
using System.Collections.Generic;
using System.Numerics;

namespace NeoRadar.PluginSdk.Rendering;

/// <summary>
/// Retained-mode drawing surface for one screen. Add objects once; mutate or
/// remove them as state changes. The host renders every frame from the scene
/// state; there is no per-frame "draw" call.
/// </summary>
public interface IPluginGraphics
{
    // ── Add (world-space; throws on PluginInsetScreen) ──

    /// <summary>Add a world-space polyline. Coordinates in EPSG:3857 metres.</summary>
    GraphicsObjectRef AddWorldLine(IReadOnlyList<Vector2> points, RgbaColor color, float width = 1.4f);

    /// <summary>Add multiple world-space polylines as a single retained object.</summary>
    GraphicsObjectRef AddWorldPolyline(IReadOnlyList<IReadOnlyList<Vector2>> polylines, RgbaColor color, float width = 1.4f);

    /// <summary>Add a filled world-space polygon (first ring outer, subsequent rings holes).</summary>
    GraphicsObjectRef AddWorldPolygon(IReadOnlyList<IReadOnlyList<Vector2>> rings, RgbaColor color);

    /// <summary>Add a single world-space point with a fixed-screen-size symbol.</summary>
    GraphicsObjectRef AddWorldPoint(Vector2 worldPos, PointSymbol symbol, RgbaColor color);

    /// <summary>
    /// Add a world-space text label. <paramref name="align"/> places <paramref name="worldPos"/>
    /// at the text's left edge, horizontal centre or right edge; a later
    /// <see cref="SetText"/> keeps the alignment.
    /// </summary>
    GraphicsObjectRef AddWorldText(Vector2 worldPos, string text, RgbaColor color, float fontSize = 12f, TextAlign align = TextAlign.Left);

    // ── Add (screen-space; available everywhere) ──

    /// <summary>Add a screen-space polyline. Coordinates in pixels relative to the control.</summary>
    GraphicsObjectRef AddScreenLine(IReadOnlyList<Vector2> points, RgbaColor color, float width = 1.4f);

    /// <summary>Add a filled screen-space rectangle.</summary>
    GraphicsObjectRef AddScreenRect(Rect bounds, RgbaColor fillColor);

    /// <summary>Add a stroked screen-space rectangle outline.</summary>
    GraphicsObjectRef AddScreenStrokeRect(Rect bounds, RgbaColor color, float width = 1f);

    /// <summary>Add a screen-space point with a symbol.</summary>
    GraphicsObjectRef AddScreenPoint(Vector2 screenPos, PointSymbol symbol, RgbaColor color);

    /// <summary>
    /// Add a screen-space text label. <paramref name="anchor"/> is the text's left edge,
    /// horizontal centre or right edge at its baseline, per <paramref name="align"/>;
    /// a later <see cref="SetText"/> keeps the alignment.
    /// </summary>
    GraphicsObjectRef AddScreenText(Vector2 anchor, string text, RgbaColor color, float fontSize = 12f, TextAlign align = TextAlign.Left);

    // ── Measure ──

    /// <summary>
    /// The width and height <paramref name="text"/> occupies at <paramref name="fontSize"/>,
    /// for layouts that must size themselves (button widths, column fitting). Synchronous and
    /// cheap enough for a rebuild, not for every frame; cache the result alongside the text.
    /// Returns <see cref="Vector2.Zero"/> when the host has no font atlas bound yet.
    /// </summary>
    Vector2 MeasureScreenText(string text, float fontSize);

    // ── Mutate (cheap; no rebuild) ──

    bool SetVisible(GraphicsObjectRef r, bool visible);
    bool SetColor(GraphicsObjectRef r, RgbaColor color);
    bool SetTransform(GraphicsObjectRef r, Transform2D transform);
    bool SetMinZoom(GraphicsObjectRef r, float minZoom);
    bool SetHitTestVisible(GraphicsObjectRef r, bool hittable);

    /// <summary>Update the text content of a screen-space text ref. Returns false if the ref isn't a text ref.</summary>
    bool SetText(GraphicsObjectRef r, string text);

    /// <summary>Move the anchor point of a screen-space ref (text/rect/line). Returns false if the ref doesn't support repositioning.</summary>
    bool SetScreenAnchor(GraphicsObjectRef r, Vector2 anchor);

    /// <summary>Replace the points of a screen-space line ref. Use for leader lines whose endpoints both change.</summary>
    bool SetScreenPoints(GraphicsObjectRef r, IReadOnlyList<Vector2> points);

    /// <summary>Resize a screen-space rect ref. Use to grow/shrink hit-test or selection rectangles.</summary>
    bool SetScreenSize(GraphicsObjectRef r, float width, float height);

    // ── Hierarchy ──

    bool BringToFront(GraphicsObjectRef r);
    bool SendToBack(GraphicsObjectRef r);

    // ── Remove ──

    bool Remove(GraphicsObjectRef r);
    void Clear();

    // ── Pointer capture ──

    void CapturePointer();
    void ReleasePointer();
    bool HasCapture { get; }

    // ── Coordinate helpers ──

    Vector2 LatLonToWorld(double latDeg, double lonDeg);
    (double Lat, double Lon) WorldToLatLon(Vector2 world);
    Vector2 ScreenToWorld(Vector2 screen);
    Vector2 WorldToScreen(Vector2 world);

    // ── State ──

    Vector2 ScreenSize { get; }
    ViewportInfo? Viewport { get; }
}
