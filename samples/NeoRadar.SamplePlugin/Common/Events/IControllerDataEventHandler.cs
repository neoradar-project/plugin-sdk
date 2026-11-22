using NeoRadar.PluginSdk.ControllerData;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Controller-data events (tracking, handoffs, levels, scratchpad and the rest), on client threads.</summary>
internal interface IControllerDataEventHandler
{
    void OnControllerDataUpdated(ControllerDataUpdatedEvent evt) { }

    void OnAircraftBeaconCodeChanged(AircraftBeaconCodeChangedEvent evt) { }

    void OnAircraftHandoffCancelled(AircraftHandoffCancelledEvent evt) { }

    void OnAircraftOwnedByChanged(AircraftOwnedByChangedEvent evt) { }

    void OnAircraftHandoffRejected(AircraftHandoffRejectedEvent evt) { }

    void OnAircraftHandoffInitiated(AircraftHandoffInitiatedEvent evt) { }

    void OnAircraftTerminatedTracking(AircraftTerminatedTrackingEvent evt) { }

    void OnAircraftInitiatedTracking(AircraftInitiatedTrackingEvent evt) { }

    void OnAircraftTemporaryAltitudeChanged(AircraftTemporaryAltitudeChangedEvent evt) { }

    void OnAircraftCDMStatusChanged(AircraftCDMStatusChangedEvent evt) { }

    void OnAircraftScratchpadUpdated(AircraftScratchpadUpdatedEvent evt) { }

    void OnAircraftHeadingChanged(AircraftHeadingChangedEvent evt) { }

    void OnAircraftAssignedSpeedChanged(AircraftAssignedSpeedChangedEvent evt) { }
}
