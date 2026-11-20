using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.MiniMap;

internal sealed class MiniMapFeature : IFeature
{
    private const double InitialLat = 51.5;
    private const double InitialLon = -0.1;
    private const double InitialZoom = 8.0;

    private PluginContext? _context;
    private MiniMapRenderer? _renderer;
    private ScreenHandle _handle;

    public void Initialize(PluginContext context)
    {
        _context = context;
        _renderer = new MiniMapRenderer(context.Core);
        _handle = context.Rendering.RegisterScreen(
            new ScreenDefinition(
                Name: "Mini Map",
                Scope: ScreenScope.PluginInsetWorld,
                ZOrder: 0,
                EnabledByDefault: true,
                InsetWidth: 420,
                InsetHeight: 420,
                ShowInList: true),
            _renderer);

        _renderer.SetScreenHandle(_handle);
        context.Rendering.CenterOnLatLon(_handle, InitialLat, InitialLon, zoom: InitialZoom);
    }

    public void Shutdown()
    {
        if (_handle.IsValid)
            _context?.Rendering.UnregisterScreen(_handle);
        _handle = ScreenHandle.Invalid;
        _renderer = null;
    }

    public void OnPositionUpdate(PositionUpdateEvent evt) => _renderer?.MarkDirty();
}
