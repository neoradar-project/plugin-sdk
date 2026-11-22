using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.WindProfile;

internal sealed class WindProfileFeature : IFeature
{
    private readonly IRenderingInterface _rendering;
    private readonly WindProfileRenderer _renderer;
    private ScreenHandle _handle;

    public WindProfileFeature(ICoreApi core, HostSurfaces surfaces)
    {
        _rendering = surfaces.Rendering;
        _renderer = new WindProfileRenderer(core);
    }

    public void Initialize()
    {
        _handle = _rendering.RegisterScreen(
            new ScreenDefinition(
                Name: "Vertical Wind Profile",
                Scope: ScreenScope.PluginInsetScreen,
                ZOrder: 0,
                EnabledByDefault: false,
                InsetWidth: 360,
                InsetHeight: 700,
                ShowInList: true),
            _renderer);
    }

    public void Shutdown()
    {
        if (_handle.IsValid) _rendering.UnregisterScreen(_handle);
        _handle = ScreenHandle.Invalid;
    }
}
