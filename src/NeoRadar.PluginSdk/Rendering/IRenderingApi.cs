using System.Numerics;

namespace NeoRadar.PluginSdk.Rendering;

/// <summary>
/// Provides screen registration and the drawing surface for each registered screen.
/// </summary>
public interface IRenderingInterface
{
    // Screen lifecycle

    /// <summary>
    /// Registers a new screen for this plugin.
    /// </summary>
    /// <returns>The screen's handle, or <see cref="ScreenHandle.Invalid"/> when the host has no rendering surface.</returns>
    ScreenHandle RegisterScreen(ScreenDefinition definition, IPluginScreenRenderer renderer);

    /// <summary>
    /// Unregisters a screen and removes everything drawn on it.
    /// </summary>
    /// <returns><c>false</c> if the handle is unknown.</returns>
    bool UnregisterScreen(ScreenHandle handle);

    /// <summary>
    /// Enables or disables a screen.
    /// </summary>
    /// <returns><c>false</c> if the handle is unknown.</returns>
    bool SetScreenEnabled(ScreenHandle handle, bool enabled);

    /// <summary>
    /// Returns a value indicating whether a screen is enabled.
    /// </summary>
    bool IsScreenEnabled(ScreenHandle handle);

    /// <summary>
    /// Returns the drawing surface for a screen.
    /// </summary>
    /// <returns>The surface, or <c>null</c> if the handle is unknown or not yet bound, which usually resolves next frame.</returns>
    IPluginGraphics? Graphics(ScreenHandle handle);

    // Screen z-order

    /// <summary>
    /// Draws a screen above every other screen that shares its <see cref="ScreenDefinition.ZOrder"/>.
    /// </summary>
    /// <remarks>Main-radar screens only; it never lifts a screen above a higher ZOrder.</remarks>
    bool BringScreenToFront(ScreenHandle handle) => false;

    // PluginInsetWorld camera control

    /// <summary>
    /// Recentres a <see cref="ScreenScope.PluginInsetWorld"/> camera on a point.
    /// </summary>
    /// <param name="worldCenter">The new centre in EPSG:3857 metres.</param>
    bool SetViewportCenter(ScreenHandle handle, Vector2 worldCenter) => false;

    /// <summary>
    /// Sets the web-mercator zoom level of a <see cref="ScreenScope.PluginInsetWorld"/> camera.
    /// </summary>
    bool SetViewportZoom(ScreenHandle handle, double zoom) => false;

    /// <summary>
    /// Recentres a <see cref="ScreenScope.PluginInsetWorld"/> camera on a latitude and longitude.
    /// </summary>
    /// <param name="zoom">The new zoom level, or <c>null</c> to keep the current one.</param>
    bool CenterOnLatLon(ScreenHandle handle, double lat, double lon, double? zoom = null) => false;
}

/// <summary>
/// Provides access to the host's rendering interface.
/// </summary>
public interface IRenderingApi
{
    /// <summary>
    /// Gets the host's rendering interface, which is never null and the same instance on every call.
    /// </summary>
    IRenderingInterface GetInterface();
}
