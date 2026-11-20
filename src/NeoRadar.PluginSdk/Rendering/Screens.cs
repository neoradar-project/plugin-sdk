using System;
using System.Numerics;

namespace NeoRadar.PluginSdk.Rendering;

/// <summary>
/// Specifies where a plugin screen renders.
/// </summary>
public enum ScreenScope
{
    /// <summary>
    /// An overlay on the main radar in EPSG:3857 metres that pans and zooms with the camera.
    /// </summary>
    MainWorld,

    /// <summary>
    /// An overlay on the main radar in pixels, independent of the camera.
    /// </summary>
    MainScreen,

    /// <summary>
    /// A separate inset window with its own camera.
    /// </summary>
    PluginInsetWorld,

    /// <summary>
    /// A separate inset window in screen space only, with no camera.
    /// </summary>
    PluginInsetScreen,
}

/// <summary>
/// Specifies how often a plugin inset redraws in response to position batches.
/// </summary>
public enum BatchInvalidationFrequency
{
    /// <summary>
    /// Redraws on every position batch, the default.
    /// </summary>
    EveryBatch = 1,

    /// <summary>
    /// Redraws on every other position batch.
    /// </summary>
    EveryOtherBatch = 2,

    /// <summary>
    /// Redraws on every fourth position batch, for slow-changing displays such as wind.
    /// </summary>
    EveryFourthBatch = 4,

    /// <summary>
    /// Never redraws on a batch, so the plugin's own changes drive every redraw.
    /// </summary>
    OnDemand = 0,
}

/// <summary>
/// Represents the declaration of a plugin screen passed to registration.
/// </summary>
/// <param name="Name">The name shown in the screen list.</param>
/// <param name="Scope">Where the screen renders.</param>
/// <param name="ZOrder">The paint order; host layers run from -20 (sectors) to 90 (feedback), so 100 paints above them.</param>
/// <param name="EnabledByDefault">Whether the screen starts enabled.</param>
/// <param name="Icon">The icon shown in the screen list, or <c>null</c> for none.</param>
/// <param name="InsetWidth">The initial inset window width in pixels.</param>
/// <param name="InsetHeight">The initial inset window height in pixels.</param>
/// <param name="ShowInList">Whether the screen appears in the screen list.</param>
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
    /// <summary>
    /// Gets how often an inset screen redraws on position batches; main-radar screens ignore it.
    /// </summary>
    public BatchInvalidationFrequency BatchFrequency { get; init; } = BatchInvalidationFrequency.EveryBatch;
}

/// <summary>
/// Represents the opaque handle returned when a screen is registered.
/// </summary>
public readonly record struct ScreenHandle(ulong Value)
{
    /// <summary>
    /// Gets the handle that refers to no screen.
    /// </summary>
    public static ScreenHandle Invalid => new(0UL);

    /// <summary>
    /// Gets a value indicating whether the handle refers to a screen.
    /// </summary>
    public bool IsValid => Value != 0UL;
}

/// <summary>
/// Represents an opaque handle to a retained graphics object on one screen.
/// </summary>
/// <remarks>A reference from one screen never addresses an object on another.</remarks>
public readonly record struct GraphicsObjectRef(ulong ScreenId, ulong ObjectId)
{
    /// <summary>
    /// Gets the reference that addresses no object.
    /// </summary>
    public static GraphicsObjectRef Invalid => new(0UL, 0UL);

    /// <summary>
    /// Gets a value indicating whether the reference addresses an object.
    /// </summary>
    public bool IsValid => ScreenId != 0UL && ObjectId != 0UL;
}

/// <summary>
/// Represents a colour with each channel from 0 to 1.
/// </summary>
public readonly record struct RgbaColor(float R, float G, float B, float A = 1f)
{
    /// <summary>
    /// Creates a colour from channels from 0 to 255.
    /// </summary>
    public static RgbaColor FromBytes(byte r, byte g, byte b, byte a = 255)
        => new(r / 255f, g / 255f, b / 255f, a / 255f);
}

/// <summary>
/// Represents a 2D transform that translates, rotates and scales an object around its pivot.
/// </summary>
public readonly record struct Transform2D(
    Vector2 Position,
    float RotationDegrees,
    Vector2 Scale,
    Vector2 Pivot)
{
    /// <summary>
    /// Gets the transform that leaves an object unchanged.
    /// </summary>
    public static Transform2D Identity => new(Vector2.Zero, 0f, Vector2.One, Vector2.Zero);

    /// <summary>
    /// Creates a transform that only translates.
    /// </summary>
    public static Transform2D Translate(Vector2 position)
        => new(position, 0f, Vector2.One, Vector2.Zero);

    /// <summary>
    /// Creates a transform that translates and rotates.
    /// </summary>
    public static Transform2D TranslateRotate(Vector2 position, float rotationDegrees)
        => new(position, rotationDegrees, Vector2.One, Vector2.Zero);
}

/// <summary>
/// Specifies which horizontal point of a text object sits on its anchor, at the baseline.
/// </summary>
public enum TextAlign : byte
{
    /// <summary>
    /// The anchor is the left edge of the text.
    /// </summary>
    Left = 0,

    /// <summary>
    /// The anchor is the horizontal centre of the text.
    /// </summary>
    Center = 1,

    /// <summary>
    /// The anchor is the right edge of the text.
    /// </summary>
    Right = 2,
}

/// <summary>
/// Specifies where text sits vertically in the rectangle of <see cref="IPluginGraphics.AddScreenTextInRect"/>.
/// </summary>
/// <remarks>The host measures the font's line box, so <see cref="Middle"/> centres the glyphs, not the baseline.</remarks>
public enum TextVerticalAlign : byte
{
    /// <summary>
    /// The line box sits against the top of the rectangle.
    /// </summary>
    Top = 0,

    /// <summary>
    /// The line box is centred in the rectangle.
    /// </summary>
    Middle = 1,

    /// <summary>
    /// The line box sits against the bottom of the rectangle.
    /// </summary>
    Bottom = 2,
}

/// <summary>
/// Specifies the symbol drawn for a point object.
/// </summary>
public enum PointSymbol
{
    /// <summary>
    /// A circle.
    /// </summary>
    Circle,

    /// <summary>
    /// A square.
    /// </summary>
    Square,

    /// <summary>
    /// A triangle.
    /// </summary>
    Triangle,

    /// <summary>
    /// A diamond.
    /// </summary>
    Diamond,

    /// <summary>
    /// A cross.
    /// </summary>
    Cross,

    /// <summary>
    /// A star.
    /// </summary>
    Star,
}

/// <summary>
/// Represents a screen-space rectangle in pixels.
/// </summary>
public readonly record struct Rect(float X, float Y, float Width, float Height)
{
    /// <summary>
    /// Gets the X coordinate of the right edge.
    /// </summary>
    public float Right => X + Width;

    /// <summary>
    /// Gets the Y coordinate of the bottom edge.
    /// </summary>
    public float Bottom => Y + Height;

    /// <summary>
    /// Gets the top-left corner.
    /// </summary>
    public Vector2 TopLeft => new(X, Y);

    /// <summary>
    /// Gets the width and height.
    /// </summary>
    public Vector2 Size => new(Width, Height);
}

/// <summary>
/// Specifies the pointer button of a <see cref="PointerEvent"/>.
/// </summary>
public enum PointerButton
{
    /// <summary>
    /// No button.
    /// </summary>
    None,

    /// <summary>
    /// The left button.
    /// </summary>
    Left,

    /// <summary>
    /// The middle button.
    /// </summary>
    Middle,

    /// <summary>
    /// The right button.
    /// </summary>
    Right,

    /// <summary>
    /// The first extended button, usually back.
    /// </summary>
    XButton1,

    /// <summary>
    /// The second extended button, usually forward.
    /// </summary>
    XButton2,
}

/// <summary>
/// Specifies the modifier keys held during a pointer or key event.
/// </summary>
[Flags]
public enum KeyModifiers
{
    /// <summary>
    /// No modifier.
    /// </summary>
    None = 0,

    /// <summary>
    /// The Shift key.
    /// </summary>
    Shift = 1,

    /// <summary>
    /// The Control key.
    /// </summary>
    Control = 2,

    /// <summary>
    /// The Alt key.
    /// </summary>
    Alt = 4,

    /// <summary>
    /// The Windows or Command key.
    /// </summary>
    Meta = 8,
}

/// <summary>
/// Represents the frame data passed to <see cref="IPluginScreenRenderer.OnFrameUpdate"/>.
/// </summary>
/// <param name="DeltaTime">The time since the previous frame in seconds.</param>
/// <param name="FrameNumber">The host's frame counter.</param>
/// <param name="Viewport">The camera, or <c>null</c> on a screen-space scope.</param>
/// <param name="ScreenSize">The size of the screen in device-independent pixels.</param>
public sealed record FrameInfo(
    double DeltaTime,
    long FrameNumber,
    ViewportInfo? Viewport,
    Vector2 ScreenSize);

/// <summary>
/// Represents a pointer event delivered to a screen renderer.
/// </summary>
/// <param name="Screen">The pointer position in pixels.</param>
/// <param name="World">The pointer position in EPSG:3857 metres, or <c>null</c> on a screen-space scope.</param>
/// <param name="Button">The button that changed.</param>
/// <param name="Modifiers">The modifier keys held.</param>
/// <param name="ClickCount">The number of clicks in the current sequence.</param>
/// <param name="Hit">The topmost object under the pointer, or <c>null</c> when none is hit.</param>
public sealed record PointerEvent(
    Vector2 Screen,
    Vector2? World,
    PointerButton Button,
    KeyModifiers Modifiers,
    int ClickCount,
    GraphicsObjectRef? Hit);

/// <summary>
/// Represents a mouse wheel event delivered to a screen renderer.
/// </summary>
/// <param name="Screen">The pointer position in pixels.</param>
/// <param name="World">The pointer position in EPSG:3857 metres, or <c>null</c> on a screen-space scope.</param>
/// <param name="Delta">The scroll distance on each axis.</param>
/// <param name="Modifiers">The modifier keys held.</param>
/// <param name="Hit">The topmost object under the pointer, or <c>null</c> when none is hit.</param>
public sealed record PointerWheelEvent(
    Vector2 Screen,
    Vector2? World,
    Vector2 Delta,
    KeyModifiers Modifiers,
    GraphicsObjectRef? Hit);

/// <summary>
/// Specifies the keys a plugin can match in a <see cref="KeyEvent"/>.
/// </summary>
/// <remarks>Values match <c>Avalonia.Input.Key</c> where they overlap; any other key arrives as <see cref="None"/>.</remarks>
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

/// <summary>
/// Represents a keyboard event delivered to a screen renderer.
/// </summary>
/// <param name="Key">The key, or <see cref="Key.None"/> for one outside the enum.</param>
/// <param name="Modifiers">The modifier keys held.</param>
/// <param name="Text">The text the key produced, or <c>null</c> when it produced none.</param>
public sealed record KeyEvent(Key Key, KeyModifiers Modifiers, string? Text);
