using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.WarningZone;

internal sealed class WarningZoneFeature : IFeature
{
    private PluginContext? _context;
    private ScreenHandle _handle;

    public void Initialize(PluginContext context)
    {
        _context = context;
        WarningCircleRenderer renderer = new(context.Core);
        _handle = context.Rendering.RegisterScreen(
            new ScreenDefinition(
                Name: "Warning Zone",
                Scope: ScreenScope.MainWorld,
                ZOrder: 30,
                EnabledByDefault: true,
                ShowInList: false),
            renderer);
    }

    public void Shutdown()
    {
        if (_handle.IsValid)
            _context?.Rendering.UnregisterScreen(_handle);
        _handle = ScreenHandle.Invalid;
    }
}
