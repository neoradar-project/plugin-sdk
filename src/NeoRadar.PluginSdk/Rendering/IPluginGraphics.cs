using System;
using System.Collections.Generic;
using System.Numerics;

namespace NeoRadar.PluginSdk.Rendering;

/// <summary>
/// Provides the retained-mode drawing surface for one screen, rendered every frame from its objects.
/// </summary>
/// <remarks>Add an object once and mutate or remove it as state changes, as there is no per-frame draw call.</remarks>
public interface IPluginGraphics
{
    /// <summary>
    /// Adds a world-space polyline.
    /// </summary>
    /// <remarks>Every AddWorld call returns <see cref="GraphicsObjectRef.Invalid"/> on a <see cref="ScreenScope.MainScreen"/> or <see cref="ScreenScope.PluginInsetScreen"/>.</remarks>
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
    /// <remarks>
    /// <see cref="SetText"/> and <see cref="SetColor"/> change the label in place, keeping its position, size and alignment.
    /// The colour's alpha is drawn.
    /// </remarks>
    /// <param name="worldPos">The position in EPSG:3857 metres.</param>
    /// <param name="align">The edge of the text that sits on <paramref name="worldPos"/>, kept by <see cref="SetText"/>.</param>
    GraphicsObjectRef AddWorldText(Vector2 worldPos, string text, RgbaColor color, float fontSize = 12f, TextAlign align = TextAlign.Left);

    // Screen-space objects, available on every screen

    /// <summary>
    /// Adds a screen-space polyline.
    /// </summary>
    /// <param name="points">The vertices in device-independent pixels relative to the control, not snapped to whole pixels.</param>
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
    /// <param name="anchor">The baseline point at the edge or centre chosen by <paramref name="align"/>, kept by <see cref="SetText"/>.</param>
    GraphicsObjectRef AddScreenText(Vector2 anchor, string text, RgbaColor color, float fontSize = 12f, TextAlign align = TextAlign.Left);

    /// <summary>
    /// Adds a screen-space text label aligned inside a rectangle using the font's own metrics.
    /// </summary>
    /// <param name="bounds">The rectangle the text is aligned in, which does not clip it.</param>
    /// <remarks>Prefer this to <see cref="AddScreenText"/> for text in a box, because it keeps the rectangle through later edits.</remarks>
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
    /// <remarks>An object shows only while its screen is also enabled, and disabling the screen keeps each object's own setting.</remarks>
    bool SetVisible(GraphicsObjectRef r, bool visible);

    /// <summary>
    /// Sets the colour of an object.
    /// </summary>
    bool SetColor(GraphicsObjectRef r, RgbaColor color);

    /// <summary>
    /// Sets the transform applied to a world object.
    /// </summary>
    /// <returns><c>false</c> for a screen-space object, which takes no transform.</returns>
    bool SetTransform(GraphicsObjectRef r, Transform2D transform);

    /// <summary>
    /// Sets the lowest zoom level at which a world object is drawn.
    /// </summary>
    /// <returns><c>false</c> for a screen-space object, which draws at every zoom.</returns>
    bool SetMinZoom(GraphicsObjectRef r, float minZoom);

    /// <summary>
    /// Sets whether an object takes part in hit testing.
    /// </summary>
    /// <remarks>
    /// An object that does not lets a press through to the targets, the map and the client's tools. A hit-testable object on
    /// a screen whose <see cref="ScreenDefinition.ZOrder"/> is above an active client tool, such as the heading tool at 55,
    /// takes the press and cancels the tool.
    /// </remarks>
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
    /// Draws an object above every other object of its kind on the same screen.
    /// </summary>
    /// <remarks>
    /// World objects order only within their kind (fills, then lines, then points), and every screen-space object draws above
    /// every world object. On a <see cref="ScreenScope.PluginInsetWorld"/> a filled screen rectangle that is not hit-testable
    /// is a background drawn below the world.
    /// </remarks>
    bool BringToFront(GraphicsObjectRef r);

    /// <summary>
    /// Draws an object below every other object of its kind on the same screen.
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
    /// Routes every pointer move and release to this screen until <see cref="ReleasePointer"/> is called or a button is released.
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
    /// Asks the host to draw this screen again soon, running its renderer's OnFrameUpdate while the screen is enabled.
    /// </summary>
    /// <remarks>Safe to call from any thread. Calling it from OnFrameUpdate on every frame keeps the view drawing continuously.</remarks>
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
    /// Converts a screen position in device-independent pixels to EPSG:3857 metres.
    /// </summary>
    Vector2 ScreenToWorld(Vector2 screen);

    /// <summary>
    /// Converts EPSG:3857 metres to a screen position in device-independent pixels.
    /// </summary>
    Vector2 WorldToScreen(Vector2 world);

    // State

    /// <summary>
    /// Gets the size of the screen in device-independent pixels, as <see cref="FrameInfo.ScreenSize"/>.
    /// </summary>
    Vector2 ScreenSize { get; }

    /// <summary>
    /// Gets the camera of a world-space screen, or <c>null</c> on a screen-space scope.
    /// </summary>
    ViewportInfo? Viewport { get; }
}
