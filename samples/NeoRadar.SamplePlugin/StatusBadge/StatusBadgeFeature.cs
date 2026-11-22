using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;

namespace NeoRadar.SamplePlugin.StatusBadge;

internal sealed class StatusBadgeFeature : IFeature, IAircraftEventHandler, IStatusBadgeScreen
{
    private readonly IRenderingInterface _rendering;
    private readonly StatusBadgeRenderer _renderer;
    private ScreenHandle _handle;

    public StatusBadgeFeature(ICoreApi core, HostSurfaces surfaces, ThemeColours colours)
    {
        _rendering = surfaces.Rendering;
        _renderer = new StatusBadgeRenderer(core, colours);
    }

    public void Initialize()
    {
        _handle = _rendering.RegisterScreen(
            new ScreenDefinition(
                Name: "Status Badge",
                Scope: ScreenScope.MainScreen,
                ZOrder: 80,
                EnabledByDefault: true,
                ShowInList: false),
            _renderer);
        _renderer.Refresh();
    }

    public void Shutdown()
    {
        if (_handle.IsValid) _rendering.UnregisterScreen(_handle);
        _handle = ScreenHandle.Invalid;
    }

    public bool Toggle()
    {
        bool on = !_rendering.IsScreenEnabled(_handle);
        _rendering.SetScreenEnabled(_handle, on);
        return _rendering.IsScreenEnabled(_handle);
    }

    // Client threads.
    public void OnAircraftConnected(AircraftConnectedEvent evt) => _renderer.Refresh();

    public void OnAircraftDisconnected(AircraftDisconnectedEvent evt) => _renderer.Refresh();
}
