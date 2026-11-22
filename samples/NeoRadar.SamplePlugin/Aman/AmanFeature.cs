using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;

namespace NeoRadar.SamplePlugin.Aman;

internal sealed class AmanFeature : IFeature, IAircraftEventHandler, IAmanInset
{
    private readonly IRenderingInterface _rendering;
    private readonly AmanRenderer _renderer;
    private ScreenHandle _handle;

    public AmanFeature(ICoreApi core, HostSurfaces surfaces)
    {
        _rendering = surfaces.Rendering;
        _renderer = new AmanRenderer(core);
    }

    public void Initialize()
    {
        _handle = _rendering.RegisterScreen(
            new ScreenDefinition(
                Name: "AMAN",
                Scope: ScreenScope.PluginInsetScreen,
                ZOrder: 0,
                EnabledByDefault: true,
                InsetWidth: 480,
                InsetHeight: 600,
                ShowInList: true),
            _renderer);
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

    // Client threads; the renderer reads the dirty flag on the render thread.
    public void OnAircraftConnected(AircraftConnectedEvent evt) => _renderer.MarkDirty();

    public void OnAircraftDisconnected(AircraftDisconnectedEvent evt) => _renderer.MarkDirty();

    public void OnPositionUpdate(PositionUpdateEvent evt) => _renderer.MarkDirty();
}
