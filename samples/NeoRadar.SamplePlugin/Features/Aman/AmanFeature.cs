using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.Aman;

internal sealed class AmanFeature : IFeature
{
    private PluginContext? _context;
    private AmanRenderer? _renderer;
    private ScreenHandle _handle;

    public void Initialize(PluginContext context)
    {
        _context = context;
        _renderer = new AmanRenderer(context.Core);
        _handle = context.Rendering.RegisterScreen(
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
        if (_handle.IsValid)
            _context?.Rendering.UnregisterScreen(_handle);
        _handle = ScreenHandle.Invalid;
        _renderer = null;
    }

    public void OnAircraftConnected(AircraftConnectedEvent evt) => _renderer?.MarkDirty();
    public void OnAircraftDisconnected(AircraftDisconnectedEvent evt) => _renderer?.MarkDirty();
    public void OnPositionUpdate(PositionUpdateEvent evt) => _renderer?.MarkDirty();
}
