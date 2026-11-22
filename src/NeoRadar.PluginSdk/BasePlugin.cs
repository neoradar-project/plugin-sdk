namespace NeoRadar.PluginSdk;

/// <summary>
/// Represents the base class every plugin inherits from.
/// </summary>
/// <remarks>Override only the handlers the plugin needs.</remarks>
public abstract class BasePlugin
{
    /// <summary>
    /// Gets the manifest that identifies the plugin to the host.
    /// </summary>
    public abstract PluginManifest Manifest { get; }

    /// <summary>
    /// Called once after the plugin assembly is loaded.
    /// </summary>
    /// <param name="metadata">The manifest the host loaded the plugin with.</param>
    /// <param name="coreApi">The host API, which the plugin keeps for its lifetime.</param>
    /// <param name="clientInfo">The host's name, version and data folder.</param>
    public abstract void Initialize(PluginManifest metadata, ICoreApi coreApi, ClientInformation clientInfo);

    /// <summary>
    /// Called once before the plugin is unloaded, including when NeoRadar exits.
    /// </summary>
    /// <remarks>The host still answers the plugin's calls until this returns.</remarks>
    public virtual void Shutdown() { }

    // Aircraft events
    /// <summary>
    /// Called when an aircraft connects to the network, before its first position.
    /// </summary>
    /// <remarks>Wait for <see cref="Aircraft.Aircraft.IsReady"/> before reading where the aircraft is.</remarks>
    public virtual void OnAircraftConnected(Aircraft.AircraftConnectedEvent evt) { }

    /// <summary>
    /// Called when an aircraft disconnects from the network, and for every aircraft still connected when the user's own FSD connection ends.
    /// </summary>
    public virtual void OnAircraftDisconnected(Aircraft.AircraftDisconnectedEvent evt) { }

    /// <summary>
    /// Called after each position batch with a snapshot of every known aircraft.
    /// </summary>
    public virtual void OnPositionUpdate(Aircraft.PositionUpdateEvent evt) { }

    // Airport events
    /// <summary>
    /// Called when an airport is added to the airport configuration, before the runway changes that added it.
    /// </summary>
    public virtual void OnAirportAdded(Airport.AirportAddedEvent evt) { }

    /// <summary>
    /// Called when an airport is removed from the airport configuration, and for each airport when the FSD session ends.
    /// </summary>
    public virtual void OnAirportRemoved(Airport.AirportRemovedEvent evt) { }

    /// <summary>
    /// Called when an airport changes between active and observed.
    /// </summary>
    public virtual void OnAirportStatusChanged(Airport.AirportStatusChangedEvent evt) { }

    /// <summary>
    /// Called when a runway is activated or deactivated for departures or arrivals.
    /// </summary>
    public virtual void OnRunwayStatusChanged(Airport.RunwayStatusChangedEvent evt) { }

    /// <summary>
    /// Called when the set of airport configurations is replaced, and once with a count of 0 when the FSD session ends.
    /// </summary>
    public virtual void OnAirportConfigurationsUpdated(Airport.AirportConfigurationsUpdatedEvent evt) { }

    // Controller events
    /// <summary>
    /// Called when a controller's ATIS lines change.
    /// </summary>
    public virtual void OnAtisLinesUpdate(Controller.AtisLinesUpdateEvent evt) { }

    /// <summary>
    /// Called when a controller's reported capabilities change.
    /// </summary>
    public virtual void OnCapabilitiesUpdate(Controller.CapabilitiesUpdateEvent evt) { }

    /// <summary>
    /// Called when a controller disconnects from the network, and for each known controller when the user's own session ends.
    /// </summary>
    public virtual void OnControllerDisconnected(Controller.ControllerDisconnectedEvent evt) { }

    /// <summary>
    /// Called when a controller connects to the network.
    /// </summary>
    public virtual void OnControllerConnected(Controller.ControllerConnectedEvent evt) { }

    /// <summary>
    /// Called when the host resolves whether a connected controller is a working ATC position.
    /// </summary>
    public virtual void OnIsControllerATC(Controller.IsControllerATCEvent evt) { }

    // ControllerData events
    /// <summary>
    /// Called when any part of an aircraft's controller data changes.
    /// </summary>
    public virtual void OnControllerDataUpdated(ControllerData.ControllerDataUpdatedEvent evt) { }

    /// <summary>
    /// Called when an aircraft's assigned squawk changes.
    /// </summary>
    public virtual void OnAircraftBeaconCodeChanged(ControllerData.AircraftBeaconCodeChangedEvent evt) { }

    /// <summary>
    /// Called when a pending handoff is cancelled.
    /// </summary>
    public virtual void OnAircraftHandoffCancelled(ControllerData.AircraftHandoffCancelledEvent evt) { }

    /// <summary>
    /// Called when the controller tracking an aircraft changes.
    /// </summary>
    public virtual void OnAircraftOwnedByChanged(ControllerData.AircraftOwnedByChangedEvent evt) { }

    /// <summary>
    /// Called when a handoff is rejected.
    /// </summary>
    public virtual void OnAircraftHandoffRejected(ControllerData.AircraftHandoffRejectedEvent evt) { }

    /// <summary>
    /// Called when a handoff starts, whether the user offers it or receives it.
    /// </summary>
    public virtual void OnAircraftHandoffInitiated(ControllerData.AircraftHandoffInitiatedEvent evt) { }

    /// <summary>
    /// Called when a tracked aircraft is dropped.
    /// </summary>
    public virtual void OnAircraftTerminatedTracking(ControllerData.AircraftTerminatedTrackingEvent evt) { }

    /// <summary>
    /// Called when the local user starts tracking an aircraft.
    /// </summary>
    public virtual void OnAircraftInitiatedTracking(ControllerData.AircraftInitiatedTrackingEvent evt) { }

    /// <summary>
    /// Called when an aircraft's cleared level changes.
    /// </summary>
    public virtual void OnAircraftTemporaryAltitudeChanged(ControllerData.AircraftTemporaryAltitudeChangedEvent evt) { }

    /// <summary>
    /// Called when an aircraft's CDM status changes.
    /// </summary>
    public virtual void OnAircraftCDMStatusChanged(ControllerData.AircraftCDMStatusChangedEvent evt) { }

    /// <summary>
    /// Called when an aircraft's scratchpad changes.
    /// </summary>
    public virtual void OnAircraftScratchpadUpdated(ControllerData.AircraftScratchpadUpdatedEvent evt) { }

    /// <summary>
    /// Called when an aircraft's assigned heading changes.
    /// </summary>
    public virtual void OnAircraftHeadingChanged(ControllerData.AircraftHeadingChangedEvent evt) { }

    /// <summary>
    /// Called when an aircraft's assigned speed changes.
    /// </summary>
    public virtual void OnAircraftAssignedSpeedChanged(ControllerData.AircraftAssignedSpeedChangedEvent evt) { }

    /// <summary>
    /// Called when any of an aircraft's flight strip annotations changes.
    /// </summary>
    public virtual void OnFlightStripAnnotationsChanged(ControllerData.FlightStripAnnotationsChangedEvent evt) { }

    /// <summary>
    /// Called when another controller pushes an aircraft's flight strip to the user.
    /// </summary>
    public virtual void OnFlightStripPushed(ControllerData.FlightStripPushedEvent evt) { }

    // Flightplan events
    /// <summary>
    /// Called when any part of a flight plan changes, including its voice capability and its parsed route.
    /// </summary>
    public virtual void OnFlightplanUpdated(Flightplan.FlightplanUpdatedEvent evt) { }

    /// <summary>
    /// Called when a flight plan is removed, and for each plan announced to plugins when the FSD session ends.
    /// </summary>
    /// <remarks>A plugin loaded during the session can be told of the removal of a plan it never received.</remarks>
    public virtual void OnFlightplanRemoved(Flightplan.FlightplanRemovedEvent evt) { }

    /// <summary>
    /// Called when a held flight plan's voice capability changes, whether by its remarks or by a voice-type message.
    /// </summary>
    public virtual void OnFlightplanVoiceTypeChanged(Flightplan.FlightplanVoiceTypeChangedEvent evt) { }

    /// <summary>
    /// Called when a flight plan's route or its parsed form changes.
    /// </summary>
    public virtual void OnFlightplanRouteChanged(Flightplan.FlightplanRouteChangedEvent evt) { }

    // FSD events
    /// <summary>
    /// Called when the FSD connection reports an error.
    /// </summary>
    public virtual void OnFsdError(Fsd.FsdErrorEvent evt) { }

    /// <summary>
    /// Called when the FSD connection state changes.
    /// </summary>
    public virtual void OnFsdConnectionStateChange(Fsd.FsdConnectionStateChangeEvent evt) { }

    /// <summary>
    /// Called when the user connects to FSD.
    /// </summary>
    public virtual void OnFsdConnected(Fsd.FsdConnectedEvent evt) { }

    /// <summary>
    /// Called when the user disconnects from FSD.
    /// </summary>
    public virtual void OnFsdDisconnected(Fsd.FsdDisconnectedEvent evt) { }

    /// <summary>
    /// Called when the user's own connection details change.
    /// </summary>
    public virtual void OnFsdConnectionModelUpdated(Fsd.FsdConnectionModelUpdatedEvent evt) { }

    // Chat events
    /// <summary>
    /// Called when an ATIS information message is received.
    /// </summary>
    public virtual void OnATISInfoMessageReceived(Chat.ATISInfoMessageReceivedEvent evt) { }

    /// <summary>
    /// Called for a direct message to or from the user.
    /// </summary>
    public virtual void OnPrivateMessageReceived(Chat.PrivateMessageReceivedEvent evt) { }

    /// <summary>
    /// Called for a message on a frequency the user monitors, including the user's own.
    /// </summary>
    public virtual void OnFrequencyMessageReceived(Chat.FrequencyMessageReceivedEvent evt) { }

    /// <summary>
    /// Called for a network-wide broadcast, including the user's own.
    /// </summary>
    public virtual void OnBroadcastMessageReceived(Chat.BroadcastMessageReceivedEvent evt) { }

    /// <summary>
    /// Called for a supervisor or wallop message, including the user's own.
    /// </summary>
    public virtual void OnSupervisorMessageReceived(Chat.SupervisorMessageReceivedEvent evt) { }

    /// <summary>
    /// Called for a message from the FSD server itself.
    /// </summary>
    public virtual void OnServerMessageReceived(Chat.ServerMessageReceivedEvent evt) { }

    /// <summary>
    /// Called for a message on the controller-only channel.
    /// </summary>
    public virtual void OnAtcMessageReceived(Chat.AtcMessageReceivedEvent evt) { }

    // Squawk events
    /// <summary>
    /// Called when a squawk provider assigns a code to an aircraft.
    /// </summary>
    public virtual void OnSquawkAssigned(Squawk.SquawkAssignedEvent evt) { }

    // Tag events
    /// <summary>
    /// Called when a controller clicks a tag item bound to one of the plugin's actions.
    /// </summary>
    public virtual void OnTagAction(Tag.TagActionEvent evt) { }

    /// <summary>
    /// Called when a controller picks an entry in one of the plugin's tag dropdowns.
    /// </summary>
    public virtual void OnTagDropdownAction(Tag.DropdownActionEvent evt) { }

    /// <summary>
    /// Called on the plugin that owns a tag action before its dropdown opens.
    /// </summary>
    /// <returns><c>false</c> to stop the dropdown opening.</returns>
    public virtual bool OnTagShowDropdown(Tag.TagShowDropdownEvent evt) => true;

    // Window events
    /// <summary>
    /// Called for an input or lifecycle event on one of the plugin's windows.
    /// </summary>
    public virtual void OnWindowEvent(Window.WindowEvent evt) { }

    // Focus events
    /// <summary>
    /// Called when the selected aircraft changes or the selection is cleared.
    /// </summary>
    /// <remarks>Also called on the UI thread after the selected aircraft disconnects or is lost.</remarks>
    public virtual void OnSelectedAircraftChanged(Focus.SelectedAircraftChangedEvent evt) { }

    // Settings events
    /// <summary>
    /// Called after the client loads a different profile, so profile-scoped settings should be read again.
    /// </summary>
    public virtual void OnActiveProfileChanged(Settings.ActiveProfileChangedEvent evt) { }

    // Timer events
    /// <summary>
    /// Called on each whole UTC second, on a host thread, for periodic work.
    /// </summary>
    /// <remarks>
    /// <see cref="Timer.TimerEvent.Tick"/> counts the beats from 0, a skipped beat included. The handler is never re-entered,
    /// so a tick is skipped while the previous one runs, and a slow tick counts as a strike. Once every plugin's OnTimer
    /// returns, the host asks for a frame on each view showing a plugin screen, so a screen needs no RequestRedraw after it.
    /// </remarks>
    public virtual void OnTimer(Timer.TimerEvent evt) { }

    // Menu events
    /// <summary>
    /// Called when a controller clicks one of the plugin's menu entries.
    /// </summary>
    public virtual void OnMenuItemClicked(Menu.MenuItemClickedEvent evt) { }
}
