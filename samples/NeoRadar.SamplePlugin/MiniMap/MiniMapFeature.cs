using System;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Alerts.Events;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;

namespace NeoRadar.SamplePlugin.MiniMap;

internal sealed class MiniMapFeature : IFeature, IAircraftEventHandler
{
    private const double InitialLat = 51.5;
    private const double InitialLon = -0.1;
    private const double InitialZoom = 8.0;

    private readonly IRenderingInterface _rendering;
    private readonly MiniMapRenderer _renderer;
    private readonly IPluginEventBus _bus;
    private ScreenHandle _handle;
    private IDisposable? _followed;

    public MiniMapFeature(ICoreApi core, HostSurfaces surfaces, ThemeColours colours, IPluginEventBus bus)
    {
        _rendering = surfaces.Rendering;
        _bus = bus;
        _renderer = new MiniMapRenderer(core, colours);
    }

    public void Initialize()
    {
        _handle = _rendering.RegisterScreen(
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
        _rendering.CenterOnLatLon(_handle, InitialLat, InitialLon, zoom: InitialZoom);

        // Delivered on the publisher's thread; the host moves the camera on the UI thread itself.
        _followed = _bus.Subscribe<SelectedAircraftFollowed>(evt => _rendering.CenterOnLatLon(_handle, evt.Latitude, evt.Longitude));
    }

    public void Shutdown()
    {
        _followed?.Dispose();
        _followed = null;
        if (_handle.IsValid) _rendering.UnregisterScreen(_handle);
        _handle = ScreenHandle.Invalid;
    }

    // Client thread (the position batch); the renderer reads the dirty flag on the render thread.
    public void OnPositionUpdate(PositionUpdateEvent evt) => _renderer.MarkDirty();
}
