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

    // ── Screen z-order ──

    /// <summary>
    /// Draws this screen above every other screen sharing its
    /// <see cref="ScreenDefinition.ZOrder"/>, until another screen at that ZOrder is raised.
    /// Use it when the controller interacts with one of several overlapping screens.
    /// </summary>
    /// <remarks>
    /// This orders whole screens against each other, which <see cref="IPluginGraphics.BringToFront"/>
    /// cannot do: that reorders objects within one screen and refuses a handle from another.
    /// ZOrder still wins, so a screen cannot be raised above one declared at a higher ZOrder,
    /// and screens with distinct ZOrders are unaffected by this call.
    /// Main-radar scopes only; an inset screen is a real window and its own z-order.
    /// </remarks>
    bool BringScreenToFront(ScreenHandle handle) => false;

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
}

public interface IRenderingApi
{
    /// <summary>The host's interface; never null, and the same instance on every call.</summary>
    IRenderingInterface GetInterface();
}
