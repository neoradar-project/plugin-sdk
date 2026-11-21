using System;
using System.Collections.Generic;
using System.Numerics;

namespace NeoRadar.PluginSdk.Rendering;

/// <summary>
/// Provides the retained-mode drawing surface for one screen, rendered every frame from its objects.
/// </summary>
/// <remarks>Add an object once and mutate or remove it as state changes; there is no per-frame draw call.</remarks>
public interface IPluginGraphics
{
    // World-space objects; adding one throws InvalidOperationException on a PluginInsetScreen

    /// <summary>
    /// Adds a world-space polyline.
    /// </summary>
    /// <param name="points">The vertices in EPSG:3857 metres.</param>
    GraphicsObjectRef AddWorldLine(IReadOnlyList<Vector2> points, RgbaColor color, float width = 1.4f);

    /// <summary>
    /// Adds several world-space polylines as a single object.
    /// </summary>
    /// <param name="polylines">The polylines, each in EPSG:3857 metres.</param>
    GraphicsObjectRef AddWorldPolyline(IReadOnlyList<IReadOnlyList<Vector2>> polylines, RgbaColor color, float width = 1.4f);

    /// <summary>
    /// Adds a filled world-space polygon.
    /// </summary>
    /// <param name="rings">The outer ring first, then any holes, in EPSG:3857 metres.</param>
    GraphicsObjectRef AddWorldPolygon(IReadOnlyList<IReadOnlyList<Vector2>> rings, RgbaColor color);

    /// <summary>
    /// Adds a world-space point drawn with a symbol of fixed screen size.
    /// </summary>
    /// <param name="worldPos">The position in EPSG:3857 metres.</param>
    GraphicsObjectRef AddWorldPoint(Vector2 worldPos, PointSymbol symbol, RgbaColor color);

    /// <summary>
    /// Adds a world-space text label.
    /// </summary>
    /// <param name="worldPos">The position in EPSG:3857 metres.</param>
    /// <param name="align">Which edge of the text sits on <paramref name="worldPos"/>; <see cref="SetText"/> keeps it.</param>
    GraphicsObjectRef AddWorldText(Vector2 worldPos, string text, RgbaColor color, float fontSize = 12f, TextAlign align = TextAlign.Left);

    // Screen-space objects, available on every screen

    /// <summary>
    /// Adds a screen-space polyline.
    /// </summary>
    /// <param name="points">The vertices in pixels relative to the control; they are not snapped to whole pixels.</param>
    /// <param name="width">The distance from the centre of the line to its edge, so 0.5 draws a one-pixel hairline.</param>
    GraphicsObjectRef AddScreenLine(IReadOnlyList<Vector2> points, RgbaColor color, float width = 1.4f);

    /// <summary>
    /// Adds a filled screen-space rectangle.
    /// </summary>
    GraphicsObjectRef AddScreenRect(Rect bounds, RgbaColor fillColor);

    /// <summary>
    /// Adds a screen-space rectangle outline snapped to whole device pixels.
    /// </summary>
    /// <param name="width">The full width of the outline, so 1 is one crisp pixel at any display scaling.</param>
    GraphicsObjectRef AddScreenStrokeRect(Rect bounds, RgbaColor color, float width = 1f);

    /// <summary>
    /// Adds a screen-space point drawn with a symbol.
    /// </summary>
    GraphicsObjectRef AddScreenPoint(Vector2 screenPos, PointSymbol symbol, RgbaColor color);

    /// <summary>
    /// Adds a screen-space text label.
    /// </summary>
    /// <param name="anchor">The baseline point at the edge or centre chosen by <paramref name="align"/>; <see cref="SetText"/> keeps it.</param>
    GraphicsObjectRef AddScreenText(Vector2 anchor, string text, RgbaColor color, float fontSize = 12f, TextAlign align = TextAlign.Left);

    /// <summary>
    /// Adds a screen-space text label aligned inside a rectangle using the font's own metrics.
    /// </summary>
    /// <param name="bounds">The rectangle the text is aligned in; text is not clipped to it.</param>
    /// <remarks>Prefer this to <see cref="AddScreenText"/> for text in a box; it keeps the rectangle through later edits.</remarks>
    GraphicsObjectRef AddScreenTextInRect(
        Rect bounds,
        string text,
        RgbaColor color,
        float fontSize = 12f,
        TextAlign align = TextAlign.Center,
        TextVerticalAlign verticalAlign = TextVerticalAlign.Middle);

    // Measurement

    /// <summary>
    /// Measures the width and height of a string at a font size.
    /// </summary>
    /// <returns>The size in pixels, or <see cref="Vector2.Zero"/> when the host has no font atlas bound yet.</returns>
    /// <remarks>Cheap enough for a rebuild but not for every frame, so cache the result with the text.</remarks>
    Vector2 MeasureScreenText(string text, float fontSize);

    // Mutation, applied without a rebuild

    /// <summary>
    /// Shows or hides an object.
    /// </summary>
    bool SetVisible(GraphicsObjectRef r, bool visible);

    /// <summary>
    /// Sets the colour of an object.
    /// </summary>
    bool SetColor(GraphicsObjectRef r, RgbaColor color);

    /// <summary>
    /// Sets the transform applied to an object.
    /// </summary>
    bool SetTransform(GraphicsObjectRef r, Transform2D transform);

    /// <summary>
    /// Sets the lowest zoom level at which an object is drawn.
    /// </summary>
    bool SetMinZoom(GraphicsObjectRef r, float minZoom);

    /// <summary>
    /// Sets whether an object takes part in hit testing.
    /// </summary>
    bool SetHitTestVisible(GraphicsObjectRef r, bool hittable);

    /// <summary>
    /// Replaces the text of a text object.
    /// </summary>
    /// <returns><c>false</c> if the object is not text.</returns>
    bool SetText(GraphicsObjectRef r, string text);

    /// <summary>
    /// Moves the anchor point of a screen-space text, rectangle, line or point.
    /// </summary>
    /// <returns><c>false</c> if the object cannot be repositioned.</returns>
    bool SetScreenAnchor(GraphicsObjectRef r, Vector2 anchor);

    /// <summary>
    /// Replaces the points of a screen-space line.
    /// </summary>
    bool SetScreenPoints(GraphicsObjectRef r, IReadOnlyList<Vector2> points);

    /// <summary>
    /// Resizes a screen-space rectangle.
    /// </summary>
    bool SetScreenSize(GraphicsObjectRef r, float width, float height);

    // Ordering

    /// <summary>
    /// Draws an object above every other object on the same screen.
    /// </summary>
    bool BringToFront(GraphicsObjectRef r);

    /// <summary>
    /// Draws an object below every other object on the same screen.
    /// </summary>
    bool SendToBack(GraphicsObjectRef r);

    // Removal

    /// <summary>
    /// Removes an object from the screen.
    /// </summary>
    bool Remove(GraphicsObjectRef r);

    /// <summary>
    /// Removes every object from the screen.
    /// </summary>
    void Clear();

    // Pointer capture

    /// <summary>
    /// Routes every pointer event to this screen until <see cref="ReleasePointer"/> is called.
    /// </summary>
    void CapturePointer();

    /// <summary>
    /// Releases a pointer capture taken by this screen.
    /// </summary>
    void ReleasePointer();

    /// <summary>
    /// Gets a value indicating whether this screen holds the pointer capture.
    /// </summary>
    bool HasCapture { get; }

    // Redraw

    /// <summary>
    /// Asks the host to draw this screen again soon, which runs its renderer's OnFrameUpdate, and is safe to call from any thread.
    /// </summary>
    void RequestRedraw();

    // Coordinate conversion

    /// <summary>
    /// Converts a latitude and longitude in degrees to EPSG:3857 metres.
    /// </summary>
    Vector2 LatLonToWorld(double latDeg, double lonDeg);

    /// <summary>
    /// Converts EPSG:3857 metres to a latitude and longitude in degrees.
    /// </summary>
    (double Lat, double Lon) WorldToLatLon(Vector2 world);

    /// <summary>
    /// Converts a screen position in pixels to EPSG:3857 metres.
    /// </summary>
    Vector2 ScreenToWorld(Vector2 screen);

    /// <summary>
    /// Converts EPSG:3857 metres to a screen position in pixels.
    /// </summary>
    Vector2 WorldToScreen(Vector2 world);

    // State

    /// <summary>
    /// Gets the size of the screen in pixels.
    /// </summary>
    Vector2 ScreenSize { get; }

    /// <summary>
    /// Gets the camera of a world-space screen, or <c>null</c> on a screen-space scope.
    /// </summary>
    ViewportInfo? Viewport { get; }
}
