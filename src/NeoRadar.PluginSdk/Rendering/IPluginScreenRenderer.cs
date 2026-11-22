namespace NeoRadar.PluginSdk.Rendering;

/// <summary>
/// Provides the optional callbacks for one registered screen.
/// </summary>
public interface IPluginScreenRenderer
{
    /// <summary>
    /// Called every frame for animation that is not driven by events.
    /// </summary>
    /// <remarks>Runs on the render thread every frame, so it must not allocate.</remarks>
    void OnFrameUpdate(IPluginGraphics g, FrameInfo info) { }

    /// <summary>
    /// Called once per press of the left button inside the screen's hit-test region.
    /// </summary>
    /// <remarks>Right and middle presses belong to the host.</remarks>
    /// <returns><c>true</c> to consume the press and capture the pointer until release, so lower ZOrders miss it.</returns>
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
    /// Called when the controller or the plugin disables the screen, but not on unload.
    /// </summary>
    void OnDisabled() { }

    /// <summary>
    /// Called when a disabled screen is enabled again.
    /// </summary>
    void OnEnabled() { }
}
