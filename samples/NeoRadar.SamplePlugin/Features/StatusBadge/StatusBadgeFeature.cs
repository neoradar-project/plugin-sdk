using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.StatusBadge;

internal sealed class StatusBadgeFeature : IFeature
{
    private PluginContext? _context;
    private StatusBadgeRenderer? _renderer;
    private ScreenHandle _handle;

    public void Initialize(PluginContext context)
    {
        _context = context;
        _renderer = new StatusBadgeRenderer(context.Core);
        _handle = context.Rendering.RegisterScreen(
            new ScreenDefinition(
                Name: "Status Badge",
                Scope: ScreenScope.MainScreen,
                ZOrder: 80,
                EnabledByDefault: true,
                ShowInList: false),
            _renderer);
    }

    public void Shutdown()
    {
        if (_handle.IsValid)
            _context?.Rendering.UnregisterScreen(_handle);
        _handle = ScreenHandle.Invalid;
        _renderer = null;
    }

    public void OnAircraftConnected(AircraftConnectedEvent evt) => _renderer?.Refresh();
    public void OnAircraftDisconnected(AircraftDisconnectedEvent evt) => _renderer?.Refresh();
}
