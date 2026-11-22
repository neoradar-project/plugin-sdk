using NeoRadar.PluginSdk.Airport;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Airport and runway configuration events, on client threads.</summary>
internal interface IAirportEventHandler
{
    void OnAirportAdded(AirportAddedEvent evt) { }

    void OnAirportRemoved(AirportRemovedEvent evt) { }

    void OnAirportStatusChanged(AirportStatusChangedEvent evt) { }

    void OnRunwayStatusChanged(RunwayStatusChangedEvent evt) { }

    void OnAirportConfigurationsUpdated(AirportConfigurationsUpdatedEvent evt) { }
}
