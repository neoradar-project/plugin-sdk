using System;
using System.Numerics;

namespace NeoRadar.PluginSdk.Rendering;

/// <summary>Where a plugin screen renders. Picked at registration time.</summary>
public enum ScreenScope
{
    /// <summary>Overlay on the main radar in EPSG:3857 metres. Pans and zooms with the camera.</summary>
    MainWorld,

    /// <summary>Overlay on the main radar in pixel coordinates. Independent of the camera.</summary>
    MainScreen,

    /// <summary>Dedicated inset window with its own viewport (camera, pan, zoom).</summary>
    PluginInsetWorld,

    /// <summary>Dedicated inset window with no viewport: pure screen space.</summary>
    PluginInsetScreen,
}

/// <summary>How often a plugin inset should re-render in response to position batches.</summary>
public enum BatchInvalidationFrequency
{
    /// <summary>Wake every position batch (~1.5 Hz). Default.</summary>
    EveryBatch = 1,

    /// <summary>Wake every other batch (~0.75 Hz). For displays that update at half rate.</summary>
    EveryOtherBatch = 2,

    /// <summary>Wake every fourth batch (~0.4 Hz). For slow-changing displays (wind, weather).</summary>
    EveryFourthBatch = 4,

    /// <summary>Never auto-wake on batches. Plugin drives invalidation through its own mutations.</summary>
    OnDemand = 0,
}

/// <summary>
/// Declarative screen registration. <see cref="ZOrder"/> slots between host modules:
/// SectorDisplay=-20, ExtendedCenterlines=-10, GeoJson=0, MouseDistanceMeasurer=5,
/// RangeRings=10, SpeedVector=45, AircraftDistanceMeasurer=46, RouteDraw=47, Rdf=48,
/// Targets=50, HeadingTool=55, Tags=60, FeedbackOverlay=90. Default 100 paints above
/// all host chrome.
/// </summary>
public sealed record ScreenDefinition(
    string Name,
    ScreenScope Scope,
    int ZOrder = 100,
    bool EnabledByDefault = true,
    string? Icon = null,
    int InsetWidth = 480,
    int InsetHeight = 320,
    bool ShowInList = true)
{
    /// <summary>How often this screen re-renders on position batches. Inset scopes only;
    /// ignored for main-radar overlays.</summary>
    public BatchInvalidationFrequency BatchFrequency { get; init; } = BatchInvalidationFrequency.EveryBatch;
}

/// <summary>Stable, opaque handle returned by <c>RegisterScreen</c>.</summary>
public readonly record struct ScreenHandle(ulong Value)
{
    public static ScreenHandle Invalid => new(0UL);

    public bool IsValid => Value != 0UL;
}

/// <summary>
/// Opaque handle for a retained graphics object. Plugins keep these in their own
/// dictionaries to associate scene objects with metadata. Scoped to a single
/// screen: refs from one screen never address objects in another.
/// </summary>
public readonly record struct GraphicsObjectRef(ulong ScreenId, ulong ObjectId)
{
    public static GraphicsObjectRef Invalid => new(0UL, 0UL);

    public bool IsValid => ScreenId != 0UL && ObjectId != 0UL;
}

/// <summary>
/// SDK-side colour mirror. Floats in [0,1]. Adapter converts to host
/// <c>NeoRadar.GeoJson.RgbaColor</c> at the boundary.
/// </summary>
public readonly record struct RgbaColor(float R, float G, float B, float A = 1f)
{
    public static RgbaColor FromBytes(byte r, byte g, byte b, byte a = 255)
        => new(r / 255f, g / 255f, b / 255f, a / 255f);
}

/// <summary>
/// 2D affine transform applied to a retained object. Translate/rotate/scale
/// around <see cref="Pivot"/>.
/// </summary>
public readonly record struct Transform2D(
    Vector2 Position,
    float RotationDegrees,
    Vector2 Scale,
    Vector2 Pivot)
{
    public static Transform2D Identity => new(Vector2.Zero, 0f, Vector2.One, Vector2.Zero);

    public static Transform2D Translate(Vector2 position)
        => new(position, 0f, Vector2.One, Vector2.Zero);

    public static Transform2D TranslateRotate(Vector2 position, float rotationDegrees)
        => new(position, rotationDegrees, Vector2.One, Vector2.Zero);
}

/// <summary>
/// How a text object sits against its anchor: the anchor is the text's left edge,
/// horizontal centre or right edge, at its baseline.
/// </summary>
public enum TextAlign : byte
{
    /// <summary>The anchor is the left edge of the text.</summary>
    Left = 0,

    /// <summary>The anchor is the horizontal centre of the text.</summary>
    Center = 1,

    /// <summary>The anchor is the right edge of the text.</summary>
    Right = 2,
}

/// <summary>
/// How a text object sits vertically against the rectangle it is laid out in, for
/// <see cref="IPluginGraphics.AddScreenTextInRect"/>. The host measures the font's line box,
/// so <see cref="Middle"/> centres the glyphs rather than the baseline.
/// </summary>
public enum TextVerticalAlign : byte
{
    /// <summary>The text's line box sits against the top of the rectangle.</summary>
    Top = 0,

    /// <summary>The text's line box is centred in the rectangle.</summary>
    Middle = 1,

    /// <summary>The text's line box sits against the bottom of the rectangle.</summary>
    Bottom = 2,
}

/// <summary>Common point symbols for <see cref="IPluginGraphics.AddWorldPoint"/>.</summary>
public enum PointSymbol
{
    Circle,
    Square,
    Triangle,
    Diamond,
    Cross,
    Star,
}

/// <summary>Screen-space rectangle in pixel coordinates.</summary>
public readonly record struct Rect(float X, float Y, float Width, float Height)
{
    public float Right => X + Width;
    public float Bottom => Y + Height;
    public Vector2 TopLeft => new(X, Y);
    public Vector2 Size => new(Width, Height);
}

/// <summary>Pointer button identity for <see cref="PointerEvent"/>.</summary>
public enum PointerButton
{
    None,
    Left,
    Middle,
    Right,
    XButton1,
    XButton2,
}

/// <summary>Modifier keys held during a pointer or key event.</summary>
[Flags]
public enum KeyModifiers
{
    None = 0,
    Shift = 1,
    Control = 2,
    Alt = 4,
    Meta = 8,
}

/// <summary>Per-frame info passed to <c>OnFrameUpdate</c>.</summary>
public sealed record FrameInfo(
    double DeltaTime,
    long FrameNumber,
    ViewportInfo? Viewport,
    Vector2 ScreenSize);

/// <summary>
/// Pointer event delivered to <see cref="IPluginScreenRenderer"/>. <see cref="World"/>
/// is null on screen-only scopes. <see cref="Hit"/> is the topmost retained object
/// in the screen's container that contained the cursor (null on miss).
/// </summary>
public sealed record PointerEvent(
    Vector2 Screen,
    Vector2? World,
    PointerButton Button,
    KeyModifiers Modifiers,
    int ClickCount,
    GraphicsObjectRef? Hit);

/// <summary>Pointer wheel scroll event.</summary>
public sealed record PointerWheelEvent(
    Vector2 Screen,
    Vector2? World,
    Vector2 Delta,
    KeyModifiers Modifiers,
    GraphicsObjectRef? Hit);

/// <summary>
/// Subset of common keys plugins can match against in <see cref="KeyEvent"/>.
/// Values mirror <c>Avalonia.Input.Key</c> where they overlap. Unknown keys
/// arrive as <see cref="None"/>.
/// </summary>
public enum Key
{
    None = 0,
    Cancel = 1,
    Back = 2,
    Tab = 3,
    LineFeed = 4,
    Clear = 5,
    Enter = 6,
    Pause = 7,
    CapsLock = 8,
    Escape = 13,
    Space = 18,
    PageUp = 19,
    PageDown = 20,
    End = 21,
    Home = 22,
    Left = 23,
    Up = 24,
    Right = 25,
    Down = 26,
    Insert = 30,
    Delete = 32,
    D0 = 34,
    D1 = 35,
    D2 = 36,
    D3 = 37,
    D4 = 38,
    D5 = 39,
    D6 = 40,
    D7 = 41,
    D8 = 42,
    D9 = 43,
    A = 44,
    B = 45,
    C = 46,
    D = 47,
    E = 48,
    F = 49,
    G = 50,
    H = 51,
    I = 52,
    J = 53,
    K = 54,
    L = 55,
    M = 56,
    N = 57,
    O = 58,
    P = 59,
    Q = 60,
    R = 61,
    S = 62,
    T = 63,
    U = 64,
    V = 65,
    W = 66,
    X = 67,
    Y = 68,
    Z = 69,
    F1 = 90,
    F2 = 91,
    F3 = 92,
    F4 = 93,
    F5 = 94,
    F6 = 95,
    F7 = 96,
    F8 = 97,
    F9 = 98,
    F10 = 99,
    F11 = 100,
    F12 = 101,
}

/// <summary>Keyboard event delivered to <see cref="IPluginScreenRenderer"/>.</summary>
public sealed record KeyEvent(Key Key, KeyModifiers Modifiers, string? Text);
