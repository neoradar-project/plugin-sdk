using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Window;

namespace NeoRadar.SamplePlugin.Common;

internal interface IFeature
{
    void Initialize(PluginContext context);
    void Shutdown();

    void OnAircraftConnected(AircraftConnectedEvent evt) { }
    void OnAircraftDisconnected(AircraftDisconnectedEvent evt) { }
    void OnPositionUpdate(PositionUpdateEvent evt) { }
    void OnWindowEvent(WindowEvent evt) { }
}
