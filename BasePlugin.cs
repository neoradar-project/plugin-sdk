namespace NeoRadar.PluginSdk;

/// <summary>
/// Base class plugins inherit from. Provides Initialize / Shutdown / Manifest plus a full
/// set of virtual event handlers the host calls when the corresponding Core event fires.
/// Override only the handlers you care about; empty virtual defaults keep unused ones cost-free.
/// </summary>
public abstract class BasePlugin
{
    public abstract PluginManifest Manifest { get; }

    /// <summary>
    /// Called once after the plugin assembly is loaded. The plugin should
    /// store <paramref name="coreApi"/> and use it for the rest of its lifetime.
    /// </summary>
    public abstract void Initialize(PluginManifest metadata, ICoreApi coreApi, ClientInformation clientInfo);

    public virtual void Shutdown() { }

    // Aircraft events
    public virtual void OnAircraftConnected(Aircraft.AircraftConnectedEvent evt) { }
    public virtual void OnAircraftDisconnected(Aircraft.AircraftDisconnectedEvent evt) { }
    public virtual void OnPositionUpdate(Aircraft.PositionUpdateEvent evt) { }

    // Airport events
    public virtual void OnAirportAdded(Airport.AirportAddedEvent evt) { }
    public virtual void OnAirportRemoved(Airport.AirportRemovedEvent evt) { }
    public virtual void OnAirportStatusChanged(Airport.AirportStatusChangedEvent evt) { }
    public virtual void OnRunwayStatusChanged(Airport.RunwayStatusChangedEvent evt) { }
    public virtual void OnAirportConfigurationsUpdated(Airport.AirportConfigurationsUpdatedEvent evt) { }

    // Controller events
    public virtual void OnAtcPositionUpdate(Controller.AtcPositionUpdateEvent evt) { }
    public virtual void OnAtisLinesUpdate(Controller.AtisLinesUpdateEvent evt) { }
    public virtual void OnCapabilitiesUpdate(Controller.CapabilitiesUpdateEvent evt) { }
    public virtual void OnControllerDisconnected(Controller.ControllerDisconnectedEvent evt) { }
    public virtual void OnControllerConnected(Controller.ControllerConnectedEvent evt) { }
    public virtual void OnIsControllerATC(Controller.IsControllerATCEvent evt) { }

    // ControllerData events
    public virtual void OnControllerDataUpdated(ControllerData.ControllerDataUpdatedEvent evt) { }
    public virtual void OnAircraftBeaconCodeChanged(ControllerData.AircraftBeaconCodeChangedEvent evt) { }
    public virtual void OnAircraftHandoffCancelled(ControllerData.AircraftHandoffCancelledEvent evt) { }
    public virtual void OnAircraftOwnedByChanged(ControllerData.AircraftOwnedByChangedEvent evt) { }
    public virtual void OnAircraftHandoffRejected(ControllerData.AircraftHandoffRejectedEvent evt) { }
    public virtual void OnAircraftTerminatedTracking(ControllerData.AircraftTerminatedTrackingEvent evt) { }
    public virtual void OnAircraftInitiatedTracking(ControllerData.AircraftInitiatedTrackingEvent evt) { }
    public virtual void OnAircraftTemporaryAltitudeChanged(ControllerData.AircraftTemporaryAltitudeChangedEvent evt) { }
    public virtual void OnAircraftCDMStatusChanged(ControllerData.AircraftCDMStatusChangedEvent evt) { }
    public virtual void OnAircraftScratchpadUpdated(ControllerData.AircraftScratchpadUpdatedEvent evt) { }
    public virtual void OnAircraftHeadingChanged(ControllerData.AircraftHeadingChangedEvent evt) { }
    public virtual void OnAircraftAssignedSpeedChanged(ControllerData.AircraftAssignedSpeedChangedEvent evt) { }

    // Flightplan events
    public virtual void OnFlightplanUpdated(Flightplan.FlightplanUpdatedEvent evt) { }
    public virtual void OnFlightplanRemoved(Flightplan.FlightplanRemovedEvent evt) { }
    public virtual void OnFlightplanVoiceTypeChanged(Flightplan.FlightplanVoiceTypeChangedEvent evt) { }
    public virtual void OnFlightplanRouteChanged(Flightplan.FlightplanRouteChangedEvent evt) { }

    // FSD events
    public virtual void OnFsdError(Fsd.FsdErrorEvent evt) { }
    public virtual void OnFsdConnectionStateChange(Fsd.FsdConnectionStateChangeEvent evt) { }
    public virtual void OnFsdConnected(Fsd.FsdConnectedEvent evt) { }
    public virtual void OnFsdDisconnected(Fsd.FsdDisconnectedEvent evt) { }
    public virtual void OnFsdConnectionModelUpdated(Fsd.FsdConnectionModelUpdatedEvent evt) { }

    // Chat / text-message events
    public virtual void OnFlightplanMessageReceived(Chat.FlightplanMessageReceivedEvent evt) { }
    public virtual void OnATISInfoMessageReceived(Chat.ATISInfoMessageReceivedEvent evt) { }

    // Squawk events
    public virtual void OnSquawkAssigned(Squawk.SquawkAssignedEvent evt) { }

    // Tag events
    public virtual void OnTagAction(Tag.TagActionEvent evt) { }
    public virtual void OnTagDropdownAction(Tag.DropdownActionEvent evt) { }
    /// <summary>Return false to suppress the dropdown for this action / callsign.</summary>
    public virtual bool OnTagShowDropdown(string actionId, string callsign) => true;

    // Rendering events
    public virtual void OnRenderObjectClicked(Rendering.RenderObjectEvent evt) { }
    public virtual void OnRenderObjectHovered(Rendering.RenderObjectEvent evt) { }
    public virtual void OnViewportChanged(Rendering.ViewportInfo info) { }

    // Window events
    public virtual void OnWindowEvent(Window.WindowEvent evt) { }
}
