using System.Numerics;

namespace NeoRadar.PluginSdk.Rendering;

/// <summary>
/// Plugin-facing rendering surface. Plugins register one or more
/// <see cref="ScreenDefinition"/>s; each registration returns a
/// <see cref="ScreenHandle"/> the plugin uses to look up its
/// <see cref="IPluginGraphics"/> drawing surface.
/// </summary>
public interface IRenderingInterface
{
    // ── Screen lifecycle ──

    /// <summary>
    /// Register a new screen for this plugin. Returns <see cref="ScreenHandle.Invalid"/>
    /// if the host's rendering surface isn't available (e.g. running headless).
    /// </summary>
    ScreenHandle RegisterScreen(ScreenDefinition definition, IPluginScreenRenderer renderer);

    bool UnregisterScreen(ScreenHandle handle);

    bool SetScreenEnabled(ScreenHandle handle, bool enabled);
    bool IsScreenEnabled(ScreenHandle handle);

    /// <summary>
    /// Returns the drawing surface for this screen, or <c>null</c> if the
    /// handle is unknown or the screen hasn't been bound to a render surface
    /// yet (typically resolves on the next frame after registration).
    /// </summary>
    IPluginGraphics? Graphics(ScreenHandle handle);

    // ── PluginInsetWorld camera control ──

    /// <summary>Recentre a <see cref="ScreenScope.PluginInsetWorld"/> camera on a mercator point.</summary>
    bool SetViewportCenter(ScreenHandle handle, Vector2 worldCenter) => false;

    /// <summary>Set a <see cref="ScreenScope.PluginInsetWorld"/> camera's web-mercator zoom level.</summary>
    bool SetViewportZoom(ScreenHandle handle, double zoom) => false;

    /// <summary>
    /// Recentre a <see cref="ScreenScope.PluginInsetWorld"/> camera on a lat/lon
    /// pair, with an optional zoom override. Convenience over the mercator form.
    /// </summary>
    bool CenterOnLatLon(ScreenHandle handle, double lat, double lon, double? zoom = null) => false;

    // ── Legacy (queue-based) drawing API ──
    // Returns null in Phase 1 — kept for ABI compatibility with plugins that
    // were authored against the previous IRenderingInterface shape. New
    // plugins should use the screen-based API above.

    /// <summary>Legacy: queue-based draw. Use <see cref="RegisterScreen"/> instead.</summary>
    ObjectHandle DrawLine(DrawLineParams parameters) => ObjectHandle.Invalid;

    /// <summary>Legacy: queue-based draw. Use <see cref="RegisterScreen"/> instead.</summary>
    ObjectHandle DrawFill(DrawFillParams parameters) => ObjectHandle.Invalid;

    /// <summary>Legacy: queue-based draw. Use <see cref="RegisterScreen"/> instead.</summary>
    ObjectHandle DrawText(DrawTextParams parameters) => ObjectHandle.Invalid;

    /// <summary>Legacy: queue-based draw. Use <see cref="RegisterScreen"/> instead.</summary>
    ObjectHandle DrawPoints(DrawPointParams parameters) => ObjectHandle.Invalid;

    /// <summary>Legacy: queue-based mutator.</summary>
    bool SetVisible(ObjectHandle handle, bool visible) => false;

    /// <summary>Legacy: queue-based mutator.</summary>
    bool SetColor(ObjectHandle handle, Color color) => false;

    /// <summary>Legacy: queue-based mutator.</summary>
    bool Remove(ObjectHandle handle) => false;

    /// <summary>Legacy: queue-based mutator.</summary>
    void RemoveAll() { }

    ViewportInfo GetViewport();

    Point2D WorldToScreen(double worldX, double worldY);

    Point2D ScreenToWorld(double screenX, double screenY);
}

public interface IRenderingApi
{
    IRenderingInterface? GetInterface();
}
