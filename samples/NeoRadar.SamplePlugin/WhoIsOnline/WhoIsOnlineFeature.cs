using System;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.PluginSdk.Controller;
using NeoRadar.PluginSdk.ControllerData;
using NeoRadar.PluginSdk.Fsd;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Common.Scheduling;

namespace NeoRadar.SamplePlugin.WhoIsOnline;

/// <summary>Who is online, what the network just did, and the plugin's own chat channels.</summary>
internal sealed class WhoIsOnlineFeature :
    IFeature,
    IWhoIsOnline,
    IControllerEventHandler,
    IControllerDataEventHandler,
    IFsdEventHandler,
    IChatEventHandler
{
    private static readonly TimeSpan RefreshPeriod = TimeSpan.FromSeconds(5);

    private readonly ITimedTaskScheduler _scheduler;
    private readonly WhoIsOnlineActions _actions;
    private readonly ControllerActivityProcessor _controllers;
    private readonly TrackingActivityProcessor _tracking;
    private readonly ControllerDataActivityProcessor _controllerData;
    private readonly FsdActivityProcessor _fsd;
    private readonly ChatActivityProcessor _chat;
    private IDisposable? _refreshJob;

    public WhoIsOnlineFeature(IControllerApi controllers, IFsdApi fsd, IChatApi chat, ITimedTaskScheduler scheduler, IPluginEventBus bus)
    {
        NetworkActivity activity = new();
        _scheduler = scheduler;
        _actions = new WhoIsOnlineActions(controllers, fsd, chat, bus, activity);
        _controllers = new ControllerActivityProcessor(activity);
        _tracking = new TrackingActivityProcessor(activity);
        _controllerData = new ControllerDataActivityProcessor(activity);
        _fsd = new FsdActivityProcessor(activity);
        _chat = new ChatActivityProcessor(activity);
    }

    public void Initialize() => _refreshJob = _scheduler.Every("who-is-online", RefreshPeriod, _actions.Refresh);

    public void Shutdown()
    {
        _refreshJob?.Dispose();
        _refreshJob = null;
    }

    public void Refresh() => _actions.Refresh();

    public void PostOnChannel(string channel) => _actions.PostOnChannel(channel);

    public void PostBurst() => _actions.PostBurst();

    public void OnControllerConnected(ControllerConnectedEvent evt) => _controllers.HandleControllerConnected(evt);

    public void OnControllerDisconnected(ControllerDisconnectedEvent evt) => _controllers.HandleControllerDisconnected(evt);

    public void OnAtisLinesUpdate(AtisLinesUpdateEvent evt) => _controllers.HandleAtisLinesUpdate(evt);

    public void OnCapabilitiesUpdate(CapabilitiesUpdateEvent evt) => _controllers.HandleCapabilitiesUpdate(evt);

    public void OnIsControllerATC(IsControllerATCEvent evt) => _controllers.HandleIsControllerATC(evt);

    public void OnAircraftInitiatedTracking(AircraftInitiatedTrackingEvent evt) => _tracking.HandleInitiatedTracking(evt);

    public void OnAircraftOwnedByChanged(AircraftOwnedByChangedEvent evt) => _tracking.HandleOwnedByChanged(evt);

    public void OnAircraftTerminatedTracking(AircraftTerminatedTrackingEvent evt) => _tracking.HandleTerminatedTracking(evt);

    public void OnAircraftHandoffInitiated(AircraftHandoffInitiatedEvent evt) => _tracking.HandleHandoffInitiated(evt);

    public void OnAircraftHandoffRejected(AircraftHandoffRejectedEvent evt) => _tracking.HandleHandoffRejected(evt);

    public void OnAircraftHandoffCancelled(AircraftHandoffCancelledEvent evt) => _tracking.HandleHandoffCancelled(evt);

    public void OnAircraftCDMStatusChanged(AircraftCDMStatusChangedEvent evt) => _controllerData.HandleCDMStatusChanged(evt);

    public void OnAircraftScratchpadUpdated(AircraftScratchpadUpdatedEvent evt) => _controllerData.HandleScratchpadUpdated(evt);

    public void OnAircraftHeadingChanged(AircraftHeadingChangedEvent evt) => _controllerData.HandleHeadingChanged(evt);

    public void OnAircraftAssignedSpeedChanged(AircraftAssignedSpeedChangedEvent evt) => _controllerData.HandleAssignedSpeedChanged(evt);

    public void OnAircraftTemporaryAltitudeChanged(AircraftTemporaryAltitudeChangedEvent evt) => _controllerData.HandleTemporaryAltitudeChanged(evt);

    public void OnAircraftBeaconCodeChanged(AircraftBeaconCodeChangedEvent evt) => _controllerData.HandleBeaconCodeChanged(evt);

    public void OnFsdConnected(FsdConnectedEvent evt) => _fsd.HandleConnected(evt);

    public void OnFsdDisconnected(FsdDisconnectedEvent evt) => _fsd.HandleDisconnected(evt);

    public void OnFsdConnectionStateChange(FsdConnectionStateChangeEvent evt) => _fsd.HandleConnectionStateChange(evt);

    public void OnFsdConnectionModelUpdated(FsdConnectionModelUpdatedEvent evt) => _fsd.HandleConnectionModelUpdated(evt);

    public void OnFsdError(FsdErrorEvent evt) => _fsd.HandleError(evt);

    public void OnPrivateMessageReceived(PrivateMessageReceivedEvent evt) => _chat.HandlePrivateMessage(evt);

    public void OnFrequencyMessageReceived(FrequencyMessageReceivedEvent evt) => _chat.HandleFrequencyMessage(evt);

    public void OnBroadcastMessageReceived(BroadcastMessageReceivedEvent evt) => _chat.HandleBroadcastMessage(evt);

    public void OnSupervisorMessageReceived(SupervisorMessageReceivedEvent evt) => _chat.HandleSupervisorMessage(evt);

    public void OnServerMessageReceived(ServerMessageReceivedEvent evt) => _chat.HandleServerMessage(evt);

    public void OnAtcMessageReceived(AtcMessageReceivedEvent evt) => _chat.HandleAtcMessage(evt);

    public void OnATISInfoMessageReceived(ATISInfoMessageReceivedEvent evt) => _chat.HandleATISInfoMessage(evt);
}
