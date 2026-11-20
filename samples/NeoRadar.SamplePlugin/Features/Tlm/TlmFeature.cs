using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.Tlm;

internal sealed class TlmFeature : IFeature
{
    private PluginContext? _context;
    private TlmRenderer? _renderer;
    private ScreenHandle _handle;

    public void Initialize(PluginContext context)
    {
        _context = context;
        _renderer = new TlmRenderer(context.Core);
        _handle = context.Rendering.RegisterScreen(
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
        if (_handle.IsValid)
            _context?.Rendering.UnregisterScreen(_handle);
        _handle = ScreenHandle.Invalid;
        _renderer = null;
    }

    public void OnAircraftConnected(AircraftConnectedEvent evt) => _renderer?.MarkDirty();
    public void OnAircraftDisconnected(AircraftDisconnectedEvent evt) => _renderer?.MarkDirty();
    public void OnPositionUpdate(PositionUpdateEvent evt) => _renderer?.MarkDirty();
}
