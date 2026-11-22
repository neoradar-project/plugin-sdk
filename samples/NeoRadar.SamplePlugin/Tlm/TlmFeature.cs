using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;

namespace NeoRadar.SamplePlugin.Tlm;

internal sealed class TlmFeature : IFeature, IAircraftEventHandler
{
    private readonly IRenderingInterface _rendering;
    private readonly TlmRenderer _renderer;
    private ScreenHandle _handle;

    public TlmFeature(ICoreApi core, HostSurfaces surfaces)
    {
        _rendering = surfaces.Rendering;
        _renderer = new TlmRenderer(core);
    }

    public void Initialize()
    {
        _handle = _rendering.RegisterScreen(
            new ScreenDefinition(
                Name: "Traffic Load Monitor (TLM)",
                Scope: ScreenScope.PluginInsetScreen,
                ZOrder: 0,
                EnabledByDefault: false,
                InsetWidth: 520,
                InsetHeight: 320,
                ShowInList: true),
            _renderer);
    }

    public void Shutdown()
    {
        if (_handle.IsValid) _rendering.UnregisterScreen(_handle);
        _handle = ScreenHandle.Invalid;
    }

    // Client threads; the renderer reads the dirty flag on the render thread.
    public void OnAircraftConnected(AircraftConnectedEvent evt) => _renderer.MarkDirty();

    public void OnAircraftDisconnected(AircraftDisconnectedEvent evt) => _renderer.MarkDirty();

    public void OnPositionUpdate(PositionUpdateEvent evt) => _renderer.MarkDirty();
}
