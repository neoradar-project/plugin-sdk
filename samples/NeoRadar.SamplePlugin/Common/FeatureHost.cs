using System;
using System.Collections.Generic;
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
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Common.Scheduling;

namespace NeoRadar.SamplePlugin.Common;

/// <summary>Runs every call into a feature inside a try and catch, so one feature's exception never faults the plugin.</summary>
/// <remarks>Initialize runs in registration order and Shutdown in reverse; an event reaches only its area's handlers.</remarks>
internal sealed class FeatureHost
{
    private readonly ILoggerApi _logger;
    private readonly TimedTaskScheduler _scheduler;
    private readonly List<IFeature> _features;
    private readonly List<IAircraftEventHandler> _aircraftHandlers;
    private readonly List<IAirportEventHandler> _airportHandlers;
    private readonly List<IControllerEventHandler> _controllerHandlers;
    private readonly List<IControllerDataEventHandler> _controllerDataHandlers;
    private readonly List<IFlightplanEventHandler> _flightplanHandlers;
    private readonly List<IFsdEventHandler> _fsdHandlers;
    private readonly List<IChatEventHandler> _chatHandlers;
    private readonly List<ISquawkEventHandler> _squawkHandlers;
    private readonly List<ITagEventHandler> _tagHandlers;
    private readonly List<IWindowEventHandler> _windowHandlers;
    private readonly List<IFocusEventHandler> _focusHandlers;
    private readonly List<IProfileEventHandler> _profileHandlers;
    private readonly List<ITimerHandler> _timerHandlers;
    private readonly List<IMenuEventHandler> _menuHandlers;
    private bool _initialized;

    public FeatureHost(
        ILoggerApi logger,
        TimedTaskScheduler scheduler,
        IEnumerable<IFeature> features,
        IEnumerable<IAircraftEventHandler> aircraftHandlers,
        IEnumerable<IAirportEventHandler> airportHandlers,
        IEnumerable<IControllerEventHandler> controllerHandlers,
        IEnumerable<IControllerDataEventHandler> controllerDataHandlers,
        IEnumerable<IFlightplanEventHandler> flightplanHandlers,
        IEnumerable<IFsdEventHandler> fsdHandlers,
        IEnumerable<IChatEventHandler> chatHandlers,
        IEnumerable<ISquawkEventHandler> squawkHandlers,
        IEnumerable<ITagEventHandler> tagHandlers,
        IEnumerable<IWindowEventHandler> windowHandlers,
        IEnumerable<IFocusEventHandler> focusHandlers,
        IEnumerable<IProfileEventHandler> profileHandlers,
        IEnumerable<ITimerHandler> timerHandlers,
        IEnumerable<IMenuEventHandler> menuHandlers)
    {
        _logger = logger;
        _scheduler = scheduler;
        _features = new List<IFeature>(features);
        _aircraftHandlers = new List<IAircraftEventHandler>(aircraftHandlers);
        _airportHandlers = new List<IAirportEventHandler>(airportHandlers);
        _controllerHandlers = new List<IControllerEventHandler>(controllerHandlers);
        _controllerDataHandlers = new List<IControllerDataEventHandler>(controllerDataHandlers);
        _flightplanHandlers = new List<IFlightplanEventHandler>(flightplanHandlers);
        _fsdHandlers = new List<IFsdEventHandler>(fsdHandlers);
        _chatHandlers = new List<IChatEventHandler>(chatHandlers);
        _squawkHandlers = new List<ISquawkEventHandler>(squawkHandlers);
        _tagHandlers = new List<ITagEventHandler>(tagHandlers);
        _windowHandlers = new List<IWindowEventHandler>(windowHandlers);
        _focusHandlers = new List<IFocusEventHandler>(focusHandlers);
        _profileHandlers = new List<IProfileEventHandler>(profileHandlers);
        _timerHandlers = new List<ITimerHandler>(timerHandlers);
        _menuHandlers = new List<IMenuEventHandler>(menuHandlers);
    }

    public void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        foreach (IFeature feature in _features)
            Run(feature, nameof(Initialize), feature.Initialize);
    }

    public void Shutdown()
    {
        if (!_initialized) return;
        _initialized = false;

        for (int i = _features.Count - 1; i >= 0; i--)
        {
            IFeature feature = _features[i];
            Run(feature, nameof(Shutdown), feature.Shutdown);
        }

        _scheduler.Stop();
    }

    public void OnAircraftConnected(AircraftConnectedEvent evt)
        => Dispatch(_aircraftHandlers, nameof(OnAircraftConnected), h => h.OnAircraftConnected(evt));

    public void OnAircraftDisconnected(AircraftDisconnectedEvent evt)
        => Dispatch(_aircraftHandlers, nameof(OnAircraftDisconnected), h => h.OnAircraftDisconnected(evt));

    public void OnPositionUpdate(PositionUpdateEvent evt)
        => Dispatch(_aircraftHandlers, nameof(OnPositionUpdate), h => h.OnPositionUpdate(evt));

    public void OnAirportAdded(AirportAddedEvent evt)
        => Dispatch(_airportHandlers, nameof(OnAirportAdded), h => h.OnAirportAdded(evt));

    public void OnAirportRemoved(AirportRemovedEvent evt)
        => Dispatch(_airportHandlers, nameof(OnAirportRemoved), h => h.OnAirportRemoved(evt));

    public void OnAirportStatusChanged(AirportStatusChangedEvent evt)
        => Dispatch(_airportHandlers, nameof(OnAirportStatusChanged), h => h.OnAirportStatusChanged(evt));

    public void OnRunwayStatusChanged(RunwayStatusChangedEvent evt)
        => Dispatch(_airportHandlers, nameof(OnRunwayStatusChanged), h => h.OnRunwayStatusChanged(evt));

    public void OnAirportConfigurationsUpdated(AirportConfigurationsUpdatedEvent evt)
        => Dispatch(_airportHandlers, nameof(OnAirportConfigurationsUpdated), h => h.OnAirportConfigurationsUpdated(evt));

    public void OnControllerConnected(ControllerConnectedEvent evt)
        => Dispatch(_controllerHandlers, nameof(OnControllerConnected), h => h.OnControllerConnected(evt));

    public void OnControllerDisconnected(ControllerDisconnectedEvent evt)
        => Dispatch(_controllerHandlers, nameof(OnControllerDisconnected), h => h.OnControllerDisconnected(evt));

    public void OnAtisLinesUpdate(AtisLinesUpdateEvent evt)
        => Dispatch(_controllerHandlers, nameof(OnAtisLinesUpdate), h => h.OnAtisLinesUpdate(evt));

    public void OnCapabilitiesUpdate(CapabilitiesUpdateEvent evt)
        => Dispatch(_controllerHandlers, nameof(OnCapabilitiesUpdate), h => h.OnCapabilitiesUpdate(evt));

    public void OnIsControllerATC(IsControllerATCEvent evt)
        => Dispatch(_controllerHandlers, nameof(OnIsControllerATC), h => h.OnIsControllerATC(evt));

    public void OnControllerDataUpdated(ControllerDataUpdatedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnControllerDataUpdated), h => h.OnControllerDataUpdated(evt));

    public void OnAircraftBeaconCodeChanged(AircraftBeaconCodeChangedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftBeaconCodeChanged), h => h.OnAircraftBeaconCodeChanged(evt));

    public void OnAircraftHandoffCancelled(AircraftHandoffCancelledEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftHandoffCancelled), h => h.OnAircraftHandoffCancelled(evt));

    public void OnAircraftOwnedByChanged(AircraftOwnedByChangedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftOwnedByChanged), h => h.OnAircraftOwnedByChanged(evt));

    public void OnAircraftHandoffRejected(AircraftHandoffRejectedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftHandoffRejected), h => h.OnAircraftHandoffRejected(evt));

    public void OnAircraftHandoffInitiated(AircraftHandoffInitiatedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftHandoffInitiated), h => h.OnAircraftHandoffInitiated(evt));

    public void OnAircraftTerminatedTracking(AircraftTerminatedTrackingEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftTerminatedTracking), h => h.OnAircraftTerminatedTracking(evt));

    public void OnAircraftInitiatedTracking(AircraftInitiatedTrackingEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftInitiatedTracking), h => h.OnAircraftInitiatedTracking(evt));

    public void OnAircraftTemporaryAltitudeChanged(AircraftTemporaryAltitudeChangedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftTemporaryAltitudeChanged), h => h.OnAircraftTemporaryAltitudeChanged(evt));

    public void OnAircraftCDMStatusChanged(AircraftCDMStatusChangedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftCDMStatusChanged), h => h.OnAircraftCDMStatusChanged(evt));

    public void OnAircraftScratchpadUpdated(AircraftScratchpadUpdatedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftScratchpadUpdated), h => h.OnAircraftScratchpadUpdated(evt));

    public void OnAircraftHeadingChanged(AircraftHeadingChangedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftHeadingChanged), h => h.OnAircraftHeadingChanged(evt));

    public void OnAircraftAssignedSpeedChanged(AircraftAssignedSpeedChangedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnAircraftAssignedSpeedChanged), h => h.OnAircraftAssignedSpeedChanged(evt));

    public void OnFlightStripAnnotationsChanged(FlightStripAnnotationsChangedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnFlightStripAnnotationsChanged), h => h.OnFlightStripAnnotationsChanged(evt));

    public void OnFlightStripPushed(FlightStripPushedEvent evt)
        => Dispatch(_controllerDataHandlers, nameof(OnFlightStripPushed), h => h.OnFlightStripPushed(evt));

    public void OnFlightplanUpdated(FlightplanUpdatedEvent evt)
        => Dispatch(_flightplanHandlers, nameof(OnFlightplanUpdated), h => h.OnFlightplanUpdated(evt));

    public void OnFlightplanRemoved(FlightplanRemovedEvent evt)
        => Dispatch(_flightplanHandlers, nameof(OnFlightplanRemoved), h => h.OnFlightplanRemoved(evt));

    public void OnFlightplanVoiceTypeChanged(FlightplanVoiceTypeChangedEvent evt)
        => Dispatch(_flightplanHandlers, nameof(OnFlightplanVoiceTypeChanged), h => h.OnFlightplanVoiceTypeChanged(evt));

    public void OnFlightplanRouteChanged(FlightplanRouteChangedEvent evt)
        => Dispatch(_flightplanHandlers, nameof(OnFlightplanRouteChanged), h => h.OnFlightplanRouteChanged(evt));

    public void OnFsdError(FsdErrorEvent evt)
        => Dispatch(_fsdHandlers, nameof(OnFsdError), h => h.OnFsdError(evt));

    public void OnFsdConnectionStateChange(FsdConnectionStateChangeEvent evt)
        => Dispatch(_fsdHandlers, nameof(OnFsdConnectionStateChange), h => h.OnFsdConnectionStateChange(evt));

    public void OnFsdConnected(FsdConnectedEvent evt)
        => Dispatch(_fsdHandlers, nameof(OnFsdConnected), h => h.OnFsdConnected(evt));

    public void OnFsdDisconnected(FsdDisconnectedEvent evt)
        => Dispatch(_fsdHandlers, nameof(OnFsdDisconnected), h => h.OnFsdDisconnected(evt));

    public void OnFsdConnectionModelUpdated(FsdConnectionModelUpdatedEvent evt)
        => Dispatch(_fsdHandlers, nameof(OnFsdConnectionModelUpdated), h => h.OnFsdConnectionModelUpdated(evt));

    public void OnATISInfoMessageReceived(ATISInfoMessageReceivedEvent evt)
        => Dispatch(_chatHandlers, nameof(OnATISInfoMessageReceived), h => h.OnATISInfoMessageReceived(evt));

    public void OnPrivateMessageReceived(PrivateMessageReceivedEvent evt)
        => Dispatch(_chatHandlers, nameof(OnPrivateMessageReceived), h => h.OnPrivateMessageReceived(evt));

    public void OnFrequencyMessageReceived(FrequencyMessageReceivedEvent evt)
        => Dispatch(_chatHandlers, nameof(OnFrequencyMessageReceived), h => h.OnFrequencyMessageReceived(evt));

    public void OnBroadcastMessageReceived(BroadcastMessageReceivedEvent evt)
        => Dispatch(_chatHandlers, nameof(OnBroadcastMessageReceived), h => h.OnBroadcastMessageReceived(evt));

    public void OnSupervisorMessageReceived(SupervisorMessageReceivedEvent evt)
        => Dispatch(_chatHandlers, nameof(OnSupervisorMessageReceived), h => h.OnSupervisorMessageReceived(evt));

    public void OnServerMessageReceived(ServerMessageReceivedEvent evt)
        => Dispatch(_chatHandlers, nameof(OnServerMessageReceived), h => h.OnServerMessageReceived(evt));

    public void OnAtcMessageReceived(AtcMessageReceivedEvent evt)
        => Dispatch(_chatHandlers, nameof(OnAtcMessageReceived), h => h.OnAtcMessageReceived(evt));

    public void OnSquawkAssigned(SquawkAssignedEvent evt)
        => Dispatch(_squawkHandlers, nameof(OnSquawkAssigned), h => h.OnSquawkAssigned(evt));

    public void OnTagAction(TagActionEvent evt)
        => Dispatch(_tagHandlers, nameof(OnTagAction), h => h.OnTagAction(evt));

    public void OnTagDropdownAction(DropdownActionEvent evt)
        => Dispatch(_tagHandlers, nameof(OnTagDropdownAction), h => h.OnTagDropdownAction(evt));

    /// <summary>Any feature may stop the dropdown opening; the answers are ANDed.</summary>
    public bool OnTagShowDropdown(TagShowDropdownEvent evt)
    {
        bool allow = true;
        foreach (ITagEventHandler handler in _tagHandlers)
            Run(handler, nameof(OnTagShowDropdown), () => allow &= handler.OnTagShowDropdown(evt));
        return allow;
    }

    public void OnWindowEvent(WindowEvent evt)
        => Dispatch(_windowHandlers, nameof(OnWindowEvent), h => h.OnWindowEvent(evt));

    public void OnSelectedAircraftChanged(SelectedAircraftChangedEvent evt)
        => Dispatch(_focusHandlers, nameof(OnSelectedAircraftChanged), h => h.OnSelectedAircraftChanged(evt));

    public void OnActiveProfileChanged(ActiveProfileChangedEvent evt)
        => Dispatch(_profileHandlers, nameof(OnActiveProfileChanged), h => h.OnActiveProfileChanged(evt));

    /// <summary>The tick drives the scheduler first, then the features that watch the clock.</summary>
    public void OnTimer(TimerEvent evt)
    {
        Run(_scheduler, nameof(OnTimer), _scheduler.Tick);
        Dispatch(_timerHandlers, nameof(OnTimer), h => h.OnTimer(evt));
    }

    public void OnMenuItemClicked(MenuItemClickedEvent evt)
        => Dispatch(_menuHandlers, nameof(OnMenuItemClicked), h => h.OnMenuItemClicked(evt));

    private void Dispatch<THandler>(List<THandler> handlers, string source, Action<THandler> work)
        where THandler : notnull
    {
        foreach (THandler handler in handlers)
            Run(handler, source, () => work(handler));
    }

    private void Run(object owner, string label, Action work)
    {
        try
        {
            work();
        }
        catch (Exception ex)
        {
            _logger.Error($"[{owner.GetType().Name}.{label}] {ex}");
        }
    }
}
