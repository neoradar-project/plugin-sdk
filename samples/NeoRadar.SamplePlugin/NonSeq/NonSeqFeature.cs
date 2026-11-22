using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;

namespace NeoRadar.SamplePlugin.NonSeq;

internal sealed class NonSeqFeature : IFeature, IAircraftEventHandler
{
    private readonly IRenderingInterface _rendering;
    private readonly NonSeqRenderer _renderer;
    private ScreenHandle _handle;

    public NonSeqFeature(ICoreApi core, HostSurfaces surfaces)
    {
        _rendering = surfaces.Rendering;
        _renderer = new NonSeqRenderer(core);
    }

    public void Initialize()
    {
        _handle = _rendering.RegisterScreen(
            new ScreenDefinition(
                Name: "Non-Sequenced Flights",
                Scope: ScreenScope.PluginInsetScreen,
                ZOrder: 0,
                EnabledByDefault: false,
                InsetWidth: 480,
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
