using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.WindProfile;

internal sealed class WindProfileFeature : IFeature
{
    private PluginContext? _context;
    private WindProfileRenderer? _renderer;
    private ScreenHandle _handle;

    public void Initialize(PluginContext context)
    {
        _context = context;
        _renderer = new WindProfileRenderer(context.Core);
        _handle = context.Rendering.RegisterScreen(
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
        if (_handle.IsValid)
            _context?.Rendering.UnregisterScreen(_handle);
        _handle = ScreenHandle.Invalid;
        _renderer = null;
    }
}
