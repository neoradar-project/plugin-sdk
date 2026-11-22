using NeoRadar.PluginSdk.Aircraft;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Aircraft connect, disconnect and position-batch events, on client threads.</summary>
internal interface IAircraftEventHandler
{
    void OnAircraftConnected(AircraftConnectedEvent evt) { }

    void OnAircraftDisconnected(AircraftDisconnectedEvent evt) { }

    void OnPositionUpdate(PositionUpdateEvent evt) { }
}
