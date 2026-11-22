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
/// <remarks>
/// Host layers run from -20 (sectors) to 90 (feedback), so the default ZOrder of 100 sits above them. A hit-testable object
/// above an active client tool, such as the heading tool at 55, takes the press and cancels the tool, and one below it
/// never does. World objects ignore ZOrder and draw by kind, every world fill, then every world line, then points, so a
/// world polygon never paints over a host map line. The controller's choice to turn a screen on or off is kept across
/// launches, but a plugin's own <see cref="IRenderingInterface.SetScreenEnabled"/> is not.
/// </remarks>
/// <param name="Name">The name shown in the screen list.</param>
/// <param name="Scope">Where the screen renders.</param>
/// <param name="ZOrder">The order among screens and host layers for input and screen-space objects.</param>
/// <param name="EnabledByDefault">Whether the screen starts enabled until the controller turns it on or off.</param>
/// <param name="Icon">A Lucide icon name shown before the entry in the Insets &amp; Plugin Windows menu and at the left of an inset's title bar, where an unknown name shows none in the menu and the inset's usual icon in the title bar.</param>
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
    /// Gets how often an inset screen redraws on position batches.
    /// </summary>
    /// <remarks>Main-radar screens ignore it.</remarks>
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
    /// A triangle, drawn as an outline.
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
    /// A five-pointed star, drawn as an outline.
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
/// <remarks>A press reaches a plugin for the left button only, and the extended buttons never reach one.</remarks>
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
/// <param name="Screen">The pointer position in device-independent pixels.</param>
/// <param name="World">The pointer position in EPSG:3857 metres, or <c>null</c> on a screen-space scope.</param>
/// <param name="Button">The button that changed, which is <see cref="PointerButton.Left"/> on a press, the released button on a release and <see cref="PointerButton.None"/> on a move.</param>
/// <param name="Modifiers">The modifier keys held.</param>
/// <param name="ClickCount">The number of clicks in the current sequence on a press, such as 2 for a double-click, or 0 on a move or a release.</param>
/// <param name="Hit">The topmost hit-testable object under the pointer, or <c>null</c> when none is hit.</param>
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
/// <remarks>A macOS trackpad's vertical step may be a fraction of one notch.</remarks>
/// <param name="Screen">The pointer position in device-independent pixels.</param>
/// <param name="World">The pointer position in EPSG:3857 metres, or <c>null</c> on a screen-space scope.</param>
/// <param name="Delta">One notch per event on each axis, as -1, 0 or +1.</param>
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
/// <remarks>Values match <c>Avalonia.Input.Key</c> where they overlap, and any other key arrives as <see cref="None"/>.</remarks>
public enum Key
{
    /// <summary>
    /// No key, or a key outside this enum.
    /// </summary>
    None = 0,

    /// <summary>
    /// The Cancel key.
    /// </summary>
    Cancel = 1,

    /// <summary>
    /// The Backspace key.
    /// </summary>
    Back = 2,

    /// <summary>
    /// The Tab key.
    /// </summary>
    Tab = 3,

    /// <summary>
    /// The Linefeed key.
    /// </summary>
    LineFeed = 4,

    /// <summary>
    /// The Clear key.
    /// </summary>
    Clear = 5,

    /// <summary>
    /// The Enter key.
    /// </summary>
    Enter = 6,

    /// <summary>
    /// The Pause key.
    /// </summary>
    Pause = 7,

    /// <summary>
    /// The Caps Lock key.
    /// </summary>
    CapsLock = 8,

    /// <summary>
    /// The Esc key.
    /// </summary>
    Escape = 13,

    /// <summary>
    /// The Spacebar key.
    /// </summary>
    Space = 18,

    /// <summary>
    /// The Page Up key.
    /// </summary>
    PageUp = 19,

    /// <summary>
    /// The Page Down key.
    /// </summary>
    PageDown = 20,

    /// <summary>
    /// The End key.
    /// </summary>
    End = 21,

    /// <summary>
    /// The Home key.
    /// </summary>
    Home = 22,

    /// <summary>
    /// The Left Arrow key.
    /// </summary>
    Left = 23,

    /// <summary>
    /// The Up Arrow key.
    /// </summary>
    Up = 24,

    /// <summary>
    /// The Right Arrow key.
    /// </summary>
    Right = 25,

    /// <summary>
    /// The Down Arrow key.
    /// </summary>
    Down = 26,

    /// <summary>
    /// The Insert key.
    /// </summary>
    Insert = 31,

    /// <summary>
    /// The Delete key.
    /// </summary>
    Delete = 32,

    /// <summary>
    /// The 0 key on the main keyboard.
    /// </summary>
    D0 = 34,

    /// <summary>
    /// The 1 key on the main keyboard.
    /// </summary>
    D1 = 35,

    /// <summary>
    /// The 2 key on the main keyboard.
    /// </summary>
    D2 = 36,

    /// <summary>
    /// The 3 key on the main keyboard.
    /// </summary>
    D3 = 37,

    /// <summary>
    /// The 4 key on the main keyboard.
    /// </summary>
    D4 = 38,

    /// <summary>
    /// The 5 key on the main keyboard.
    /// </summary>
    D5 = 39,

    /// <summary>
    /// The 6 key on the main keyboard.
    /// </summary>
    D6 = 40,

    /// <summary>
    /// The 7 key on the main keyboard.
    /// </summary>
    D7 = 41,

    /// <summary>
    /// The 8 key on the main keyboard.
    /// </summary>
    D8 = 42,

    /// <summary>
    /// The 9 key on the main keyboard.
    /// </summary>
    D9 = 43,

    /// <summary>
    /// The A key.
    /// </summary>
    A = 44,

    /// <summary>
    /// The B key.
    /// </summary>
    B = 45,

    /// <summary>
    /// The C key.
    /// </summary>
    C = 46,

    /// <summary>
    /// The D key.
    /// </summary>
    D = 47,

    /// <summary>
    /// The E key.
    /// </summary>
    E = 48,

    /// <summary>
    /// The F key.
    /// </summary>
    F = 49,

    /// <summary>
    /// The G key.
    /// </summary>
    G = 50,

    /// <summary>
    /// The H key.
    /// </summary>
    H = 51,

    /// <summary>
    /// The I key.
    /// </summary>
    I = 52,

    /// <summary>
    /// The J key.
    /// </summary>
    J = 53,

    /// <summary>
    /// The K key.
    /// </summary>
    K = 54,

    /// <summary>
    /// The L key.
    /// </summary>
    L = 55,

    /// <summary>
    /// The M key.
    /// </summary>
    M = 56,

    /// <summary>
    /// The N key.
    /// </summary>
    N = 57,

    /// <summary>
    /// The O key.
    /// </summary>
    O = 58,

    /// <summary>
    /// The P key.
    /// </summary>
    P = 59,

    /// <summary>
    /// The Q key.
    /// </summary>
    Q = 60,

    /// <summary>
    /// The R key.
    /// </summary>
    R = 61,

    /// <summary>
    /// The S key.
    /// </summary>
    S = 62,

    /// <summary>
    /// The T key.
    /// </summary>
    T = 63,

    /// <summary>
    /// The U key.
    /// </summary>
    U = 64,

    /// <summary>
    /// The V key.
    /// </summary>
    V = 65,

    /// <summary>
    /// The W key.
    /// </summary>
    W = 66,

    /// <summary>
    /// The X key.
    /// </summary>
    X = 67,

    /// <summary>
    /// The Y key.
    /// </summary>
    Y = 68,

    /// <summary>
    /// The Z key.
    /// </summary>
    Z = 69,

    /// <summary>
    /// The F1 key.
    /// </summary>
    F1 = 90,

    /// <summary>
    /// The F2 key.
    /// </summary>
    F2 = 91,

    /// <summary>
    /// The F3 key.
    /// </summary>
    F3 = 92,

    /// <summary>
    /// The F4 key.
    /// </summary>
    F4 = 93,

    /// <summary>
    /// The F5 key.
    /// </summary>
    F5 = 94,

    /// <summary>
    /// The F6 key.
    /// </summary>
    F6 = 95,

    /// <summary>
    /// The F7 key.
    /// </summary>
    F7 = 96,

    /// <summary>
    /// The F8 key.
    /// </summary>
    F8 = 97,

    /// <summary>
    /// The F9 key.
    /// </summary>
    F9 = 98,

    /// <summary>
    /// The F10 key.
    /// </summary>
    F10 = 99,

    /// <summary>
    /// The F11 key.
    /// </summary>
    F11 = 100,

    /// <summary>
    /// The F12 key.
    /// </summary>
    F12 = 101,
}

/// <summary>
/// Represents a keyboard event delivered to a screen renderer.
/// </summary>
/// <param name="Key">The key, or <see cref="Key.None"/> for one outside the enum.</param>
/// <param name="Modifiers">The modifier keys held.</param>
public sealed record KeyEvent(Key Key, KeyModifiers Modifiers);
