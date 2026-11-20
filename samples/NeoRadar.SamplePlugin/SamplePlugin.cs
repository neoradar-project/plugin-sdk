using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Features.Aman;
using NeoRadar.SamplePlugin.Features.DemoWindow;
using NeoRadar.SamplePlugin.Features.MiniMap;
using NeoRadar.SamplePlugin.Features.NonSeq;
using NeoRadar.SamplePlugin.Features.StatusBadge;
using NeoRadar.SamplePlugin.Features.TestCommands;
using NeoRadar.SamplePlugin.Features.Tlm;
using NeoRadar.SamplePlugin.Features.WarningZone;
using NeoRadar.SamplePlugin.Features.WindProfile;

namespace NeoRadar.SamplePlugin;

public sealed class SamplePlugin : BasePlugin
{
    public static int InitializeCount;
    public static int ShutdownCount;
    public static int AircraftConnectedCount;
    public static int WindowEventCount;
    public static int ScreenPointerCount;

    public override PluginManifest Manifest { get; } = new(
        Name: "Sample",
        Author: "NeoRadar",
        Version: "1.0.0",
        SdkMajorVersion: PluginSdkVersion.Major,
        Description: "aman-dman-style demo plugin");

    private FeatureHost? _features;

    public override void Initialize(PluginManifest metadata, ICoreApi coreApi, ClientInformation clientInfo)
    {
        InitializeCount++;
        coreApi.Logger.Info($"SamplePlugin initialized for {clientInfo.ClientName} {clientInfo.Version}");

        PluginContext context = new(coreApi);
        _features = new FeatureHost(coreApi.Logger, new IFeature[]
        {
            new DemoWindowFeature(),
            new WarningZoneFeature(),
            new StatusBadgeFeature(),
            new AmanFeature(),
            new MiniMapFeature(),
            new TlmFeature(),
            new WindProfileFeature(),
            new NonSeqFeature(),
            new TestCommandsFeature(),
        });
        _features.Initialize(context);
    }

    public override void Shutdown()
    {
        ShutdownCount++;
        _features?.Shutdown();
        _features = null;
    }

    public override void OnAircraftConnected(AircraftConnectedEvent evt)
    {
        AircraftConnectedCount++;
        _features?.Dispatch(f => f.OnAircraftConnected(evt), nameof(OnAircraftConnected));
    }

    public override void OnAircraftDisconnected(AircraftDisconnectedEvent evt)
        => _features?.Dispatch(f => f.OnAircraftDisconnected(evt), nameof(OnAircraftDisconnected));

    public override void OnPositionUpdate(PositionUpdateEvent evt)
        => _features?.Dispatch(f => f.OnPositionUpdate(evt), nameof(OnPositionUpdate));

    public override void OnWindowEvent(WindowEvent evt)
    {
        WindowEventCount++;
        _features?.Dispatch(f => f.OnWindowEvent(evt), nameof(OnWindowEvent));
    }
}
