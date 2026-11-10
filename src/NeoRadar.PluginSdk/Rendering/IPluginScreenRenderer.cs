namespace NeoRadar.PluginSdk.Rendering;

/// <summary>
/// Plugin-side callback contract for one registered screen. All members are
/// optional; override only what your screen needs.
/// </summary>
public interface IPluginScreenRenderer
{
    /// <summary>
    /// Optional per-frame hook for animation that isn't event-driven.
    /// Don't allocate here; idiomatic plugins drive scene mutations off
    /// FSD events instead.
    /// </summary>
    void OnFrameUpdate(IPluginGraphics g, FrameInfo info) { }

    /// <summary>
    /// Pointer pressed inside this screen's hit-test region. Return <c>true</c>
    /// to consume the event; modules below in ZOrder will not see it. Returning
    /// <c>true</c> also implicitly captures pointer until the matching release.
    /// </summary>
    bool OnPointerPressed(PointerEvent e) => false;

    void OnPointerMoved(PointerEvent e) { }

    void OnPointerReleased(PointerEvent e) { }

    bool OnPointerWheel(PointerWheelEvent e) => false;

    /// <summary>PluginInset* scopes only. Return <c>true</c> to consume.</summary>
    bool OnKeyDown(KeyEvent e) => false;
    bool OnKeyUp(KeyEvent e) => false;

    /// <summary>Fires when the host disables this screen (user toggle or unload).</summary>
    void OnDisabled() { }

    /// <summary>Fires when a previously-disabled screen is re-enabled.</summary>
    void OnEnabled() { }
}
