using System;
using System.Threading;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Airport;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.PluginSdk.Controller;
using NeoRadar.PluginSdk.ControllerData;
using NeoRadar.PluginSdk.Flightplan;
using NeoRadar.PluginSdk.Focus;
using NeoRadar.PluginSdk.Fsd;
using NeoRadar.PluginSdk.Logger;
using NeoRadar.PluginSdk.Menu;
using NeoRadar.PluginSdk.Settings;
using NeoRadar.PluginSdk.Squawk;
using NeoRadar.PluginSdk.Tag;
using NeoRadar.PluginSdk.Timer;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;

namespace NeoRadar.SamplePlugin.EventLog;

/// <summary>Hears every one of the host's 53 events, counts each, and keeps a few lines of what just happened.</summary>
internal sealed class EventLogFeature :
    IFeature,
    IEventLog,
    IAircraftEventHandler,
    IAirportEventHandler,
    IControllerEventHandler,
    IControllerDataEventHandler,
    IFlightplanEventHandler,
    IFsdEventHandler,
    IChatEventHandler,
    ISquawkEventHandler,
    ITagEventHandler,
    IWindowEventHandler,
    IFocusEventHandler,
    IProfileEventHandler,
    ITimerHandler,
    IMenuEventHandler
{
    private readonly ILoggerApi _logger;
    private readonly IPluginEventBus _bus;
    private readonly EventCountStore _counts = new();
    private long _publishedVersion = -1;

    public EventLogFeature(ILoggerApi logger, IPluginEventBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public void Initialize()
    {
    }

    public void Shutdown() => _logger.Info($"Sample shut down after hearing {_counts.Snapshot().Counts.Count} kinds of event");

    public void WriteEachLogLevel()
    {
        foreach (LogLevel level in Enum.GetValues<LogLevel>())
            _logger.Log(level, $"Sample writes one line at {level}; Verbose reaches only the plugin's own log");
        _counts.Note($"{Now()} wrote one line at each of the six log levels");
        PublishIfChanged();
    }

    // Client threads.
    public void OnAircraftConnected(AircraftConnectedEvent evt) => Heard(nameof(OnAircraftConnected), evt.Callsign);

    public void OnAircraftDisconnected(AircraftDisconnectedEvent evt) => Heard(nameof(OnAircraftDisconnected), evt.Callsign);

    public void OnPositionUpdate(PositionUpdateEvent evt) => _counts.Count(nameof(OnPositionUpdate));

    // Client threads.
    public void OnAirportAdded(AirportAddedEvent evt) => Heard(nameof(OnAirportAdded), evt.Icao);

    public void OnAirportRemoved(AirportRemovedEvent evt) => Heard(nameof(OnAirportRemoved), evt.Icao);

    public void OnAirportStatusChanged(AirportStatusChangedEvent evt) => Heard(nameof(OnAirportStatusChanged), $"{evt.Icao} {evt.Status}");

    public void OnRunwayStatusChanged(RunwayStatusChangedEvent evt) => Heard(nameof(OnRunwayStatusChanged), $"{evt.Icao} {evt.Runway} active {evt.Active}");

    public void OnAirportConfigurationsUpdated(AirportConfigurationsUpdatedEvent evt) => Heard(nameof(OnAirportConfigurationsUpdated), $"{evt.ConfigurationCount} airports");

    // Client threads.
    public void OnControllerConnected(ControllerConnectedEvent evt) => Heard(nameof(OnControllerConnected), evt.Callsign);

    public void OnControllerDisconnected(ControllerDisconnectedEvent evt) => Heard(nameof(OnControllerDisconnected), evt.Callsign);

    public void OnAtisLinesUpdate(AtisLinesUpdateEvent evt) => Heard(nameof(OnAtisLinesUpdate), evt.Callsign);

    public void OnCapabilitiesUpdate(CapabilitiesUpdateEvent evt) => Heard(nameof(OnCapabilitiesUpdate), evt.Callsign);

    public void OnIsControllerATC(IsControllerATCEvent evt) => Heard(nameof(OnIsControllerATC), evt.Callsign);

    // Client threads.
    public void OnControllerDataUpdated(ControllerDataUpdatedEvent evt) => _counts.Count(nameof(OnControllerDataUpdated));

    public void OnAircraftBeaconCodeChanged(AircraftBeaconCodeChangedEvent evt) => Heard(nameof(OnAircraftBeaconCodeChanged), evt.Callsign);

    public void OnAircraftHandoffCancelled(AircraftHandoffCancelledEvent evt) => Heard(nameof(OnAircraftHandoffCancelled), evt.Callsign);

    public void OnAircraftOwnedByChanged(AircraftOwnedByChangedEvent evt) => Heard(nameof(OnAircraftOwnedByChanged), evt.Callsign);

    public void OnAircraftHandoffRejected(AircraftHandoffRejectedEvent evt) => Heard(nameof(OnAircraftHandoffRejected), evt.Callsign);

    public void OnAircraftHandoffInitiated(AircraftHandoffInitiatedEvent evt) => Heard(nameof(OnAircraftHandoffInitiated), evt.Callsign);

    public void OnAircraftTerminatedTracking(AircraftTerminatedTrackingEvent evt) => Heard(nameof(OnAircraftTerminatedTracking), evt.Callsign);

    public void OnAircraftInitiatedTracking(AircraftInitiatedTrackingEvent evt) => Heard(nameof(OnAircraftInitiatedTracking), evt.Callsign);

    public void OnAircraftTemporaryAltitudeChanged(AircraftTemporaryAltitudeChangedEvent evt) => Heard(nameof(OnAircraftTemporaryAltitudeChanged), evt.Callsign);

    public void OnAircraftCDMStatusChanged(AircraftCDMStatusChangedEvent evt) => Heard(nameof(OnAircraftCDMStatusChanged), evt.Callsign);

    public void OnAircraftScratchpadUpdated(AircraftScratchpadUpdatedEvent evt) => Heard(nameof(OnAircraftScratchpadUpdated), evt.Callsign);

    public void OnAircraftHeadingChanged(AircraftHeadingChangedEvent evt) => Heard(nameof(OnAircraftHeadingChanged), evt.Callsign);

    public void OnAircraftAssignedSpeedChanged(AircraftAssignedSpeedChangedEvent evt) => Heard(nameof(OnAircraftAssignedSpeedChanged), evt.Callsign);

    public void OnFlightStripAnnotationsChanged(FlightStripAnnotationsChangedEvent evt) => Heard(nameof(OnFlightStripAnnotationsChanged), evt.Callsign);

    public void OnFlightStripPushed(FlightStripPushedEvent evt) => Heard(nameof(OnFlightStripPushed), evt.Callsign);

    // Client threads.
    public void OnFlightplanUpdated(FlightplanUpdatedEvent evt) => Heard(nameof(OnFlightplanUpdated), evt.Callsign);

    public void OnFlightplanRemoved(FlightplanRemovedEvent evt) => Heard(nameof(OnFlightplanRemoved), evt.Callsign);

    public void OnFlightplanVoiceTypeChanged(FlightplanVoiceTypeChangedEvent evt) => Heard(nameof(OnFlightplanVoiceTypeChanged), evt.Callsign);

    public void OnFlightplanRouteChanged(FlightplanRouteChangedEvent evt) => Heard(nameof(OnFlightplanRouteChanged), evt.Callsign);

    // Client threads.
    public void OnFsdError(FsdErrorEvent evt) => Heard(nameof(OnFsdError), evt.Message);

    public void OnFsdConnectionStateChange(FsdConnectionStateChangeEvent evt) => Heard(nameof(OnFsdConnectionStateChange), evt.State.ToString());

    public void OnFsdConnected(FsdConnectedEvent evt) => Heard(nameof(OnFsdConnected), evt.Callsign);

    public void OnFsdDisconnected(FsdDisconnectedEvent evt) => Heard(nameof(OnFsdDisconnected), evt.Reason);

    public void OnFsdConnectionModelUpdated(FsdConnectionModelUpdatedEvent evt) => Heard(nameof(OnFsdConnectionModelUpdated), evt.Callsign);

    // Client threads.
    public void OnATISInfoMessageReceived(ATISInfoMessageReceivedEvent evt) => Heard(nameof(OnATISInfoMessageReceived), evt.Callsign);

    public void OnPrivateMessageReceived(PrivateMessageReceivedEvent evt) => Heard(nameof(OnPrivateMessageReceived), evt.SentFrom);

    public void OnFrequencyMessageReceived(FrequencyMessageReceivedEvent evt) => Heard(nameof(OnFrequencyMessageReceived), evt.SentFrom);

    public void OnBroadcastMessageReceived(BroadcastMessageReceivedEvent evt) => Heard(nameof(OnBroadcastMessageReceived), evt.SentFrom);

    public void OnSupervisorMessageReceived(SupervisorMessageReceivedEvent evt) => Heard(nameof(OnSupervisorMessageReceived), evt.SentFrom);

    public void OnServerMessageReceived(ServerMessageReceivedEvent evt) => Heard(nameof(OnServerMessageReceived), evt.SentFrom);

    public void OnAtcMessageReceived(AtcMessageReceivedEvent evt) => Heard(nameof(OnAtcMessageReceived), evt.SentFrom);

    // Client threads.
    public void OnSquawkAssigned(SquawkAssignedEvent evt) => Heard(nameof(OnSquawkAssigned), evt.Callsign);

    // Client threads.
    public void OnTagAction(TagActionEvent evt) => Heard(nameof(OnTagAction), $"{evt.ActionId} on {evt.Callsign}");

    public void OnTagDropdownAction(DropdownActionEvent evt) => Heard(nameof(OnTagDropdownAction), $"{evt.ComponentId} on {evt.Callsign}");

    public bool OnTagShowDropdown(TagShowDropdownEvent evt)
    {
        Heard(nameof(OnTagShowDropdown), $"{evt.ActionId} on {evt.Callsign}");
        return true;
    }

    // UI thread.
    public void OnWindowEvent(WindowEvent evt) => Heard(nameof(OnWindowEvent), $"{evt.EventType} {evt.ElementId}");

    // UI thread.
    public void OnSelectedAircraftChanged(SelectedAircraftChangedEvent evt) => Heard(nameof(OnSelectedAircraftChanged), evt.Callsign ?? "cleared");

    // Client threads.
    public void OnActiveProfileChanged(ActiveProfileChangedEvent evt) => Heard(nameof(OnActiveProfileChanged), evt.ProfileName ?? "none");

    // A client thread, once a second, never re-entered.
    public void OnTimer(TimerEvent evt)
    {
        _counts.Count(nameof(OnTimer));
        PublishIfChanged();
    }

    // Client threads.
    public void OnMenuItemClicked(MenuItemClickedEvent evt) => Heard(nameof(OnMenuItemClicked), evt.ItemId);

    private void Heard(string eventName, string detail)
    {
        _counts.Count(eventName);
        _counts.Note($"{Now()} {eventName} {detail} [thread {Environment.CurrentManagedThreadId}]");
    }

    private void PublishIfChanged()
    {
        long version = _counts.Version;
        if (Interlocked.Exchange(ref _publishedVersion, version) == version) return;
        _bus.Publish(_counts.Snapshot());
    }

    private static string Now() => DateTimeOffset.UtcNow.ToString("HH:mm:ss");
}
