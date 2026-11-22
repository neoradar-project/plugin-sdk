using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.WarningZone;

internal sealed class WarningZoneFeature : IFeature
{
    private readonly IRenderingInterface _rendering;
    private readonly WarningCircleRenderer _renderer;
    private ScreenHandle _handle;

    public WarningZoneFeature(HostSurfaces surfaces, ThemeColours colours)
    {
        _rendering = surfaces.Rendering;
        _renderer = new WarningCircleRenderer(colours);
    }

    public void Initialize()
    {
        _handle = _rendering.RegisterScreen(
            new ScreenDefinition(
                Name: "Warning Zone",
                Scope: ScreenScope.MainWorld,
                ZOrder: 30,
                EnabledByDefault: true,
                ShowInList: false),
            _renderer);
    }

    public void Shutdown()
    {
        if (_handle.IsValid) _rendering.UnregisterScreen(_handle);
        _handle = ScreenHandle.Invalid;
    }
}
