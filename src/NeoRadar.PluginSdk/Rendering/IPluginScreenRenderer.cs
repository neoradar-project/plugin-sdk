namespace NeoRadar.PluginSdk.Rendering;

/// <summary>
/// Provides the callbacks for one registered screen; every member is optional.
/// </summary>
public interface IPluginScreenRenderer
{
    /// <summary>
    /// Called every frame for animation that is not driven by events.
    /// </summary>
    /// <remarks>Runs on the render thread every frame, so it must not allocate.</remarks>
    void OnFrameUpdate(IPluginGraphics g, FrameInfo info) { }

    /// <summary>
    /// Called when a pointer is pressed inside the screen's hit-test region.
    /// </summary>
    /// <returns><c>true</c> to consume the press and capture the pointer until release; lower ZOrders then miss it.</returns>
    bool OnPointerPressed(PointerEvent e) => false;

    /// <summary>
    /// Called when the pointer moves over the screen or while it is captured.
    /// </summary>
    void OnPointerMoved(PointerEvent e) { }

    /// <summary>
    /// Called when a pointer button is released.
    /// </summary>
    void OnPointerReleased(PointerEvent e) { }

    /// <summary>
    /// Called when the mouse wheel turns over the screen.
    /// </summary>
    /// <returns><c>true</c> to consume the event.</returns>
    bool OnPointerWheel(PointerWheelEvent e) => false;

    /// <summary>
    /// Called when a key is pressed while an inset screen has focus.
    /// </summary>
    /// <returns><c>true</c> to consume the event.</returns>
    bool OnKeyDown(KeyEvent e) => false;

    /// <summary>
    /// Called when a key is released while an inset screen has focus.
    /// </summary>
    /// <returns><c>true</c> to consume the event.</returns>
    bool OnKeyUp(KeyEvent e) => false;

    /// <summary>
    /// Called when the host disables the screen, by a user toggle or an unload.
    /// </summary>
    void OnDisabled() { }

    /// <summary>
    /// Called when a disabled screen is enabled again.
    /// </summary>
    void OnEnabled() { }
}
