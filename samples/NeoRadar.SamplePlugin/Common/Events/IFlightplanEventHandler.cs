using NeoRadar.PluginSdk.Flightplan;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Flight plan events, on client threads.</summary>
internal interface IFlightplanEventHandler
{
    void OnFlightplanUpdated(FlightplanUpdatedEvent evt) { }

    void OnFlightplanRemoved(FlightplanRemovedEvent evt) { }

    void OnFlightplanVoiceTypeChanged(FlightplanVoiceTypeChangedEvent evt) { }

    void OnFlightplanRouteChanged(FlightplanRouteChangedEvent evt) { }
}
