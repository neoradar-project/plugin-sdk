using System;
using Microsoft.Extensions.DependencyInjection;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Airport;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.PluginSdk.Controller;
using NeoRadar.PluginSdk.ControllerData;
using NeoRadar.PluginSdk.Flightplan;
using NeoRadar.PluginSdk.Focus;
using NeoRadar.PluginSdk.Fsd;
using NeoRadar.PluginSdk.Menu;
using NeoRadar.PluginSdk.Settings;
using NeoRadar.PluginSdk.Squawk;
using NeoRadar.PluginSdk.Tag;
using NeoRadar.PluginSdk.Timer;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin;

/// <summary>The plugin's entry point: builds the feature host, then forwards every host event to it.</summary>
public sealed class SamplePlugin : BasePlugin
{
    private ServiceProvider? _provider;
    private FeatureHost? _features;

    public override PluginManifest Manifest { get; } = new(
        Name: "Sample",
        Author: "NeoRadar",
        Version: "1.0.0",
        SdkMajorVersion: PluginSdkVersion.Major,
        Description: "The SDK showcase: a guide window and a live demo of every API and event",
        Website: "https://github.com/neoradar-project/plugin-sdk");

    public override void Initialize(PluginManifest metadata, ICoreApi coreApi, ClientInformation clientInfo)
    {
        coreApi.Logger.Info($"SamplePlugin initialised for {clientInfo.ClientName} {clientInfo.Version}");

        try
        {
            ServiceCollection services = new();
            services.AddSamplePlugin(coreApi, clientInfo);
            _provider = services.BuildServiceProvider();

            _features = _provider.GetRequiredService<FeatureHost>();
            _features.Initialize();
        }
        catch (Exception ex)
        {
            coreApi.Logger.Error($"SamplePlugin failed to compose its features: {ex}");
        }
    }

    public override void Shutdown()
    {
        _features?.Shutdown();
        _features = null;
        _provider?.Dispose();
        _provider = null;
    }

    public override void OnAircraftConnected(AircraftConnectedEvent evt) => _features?.OnAircraftConnected(evt);

    public override void OnAircraftDisconnected(AircraftDisconnectedEvent evt) => _features?.OnAircraftDisconnected(evt);

    public override void OnPositionUpdate(PositionUpdateEvent evt) => _features?.OnPositionUpdate(evt);

    public override void OnAirportAdded(AirportAddedEvent evt) => _features?.OnAirportAdded(evt);

    public override void OnAirportRemoved(AirportRemovedEvent evt) => _features?.OnAirportRemoved(evt);

    public override void OnAirportStatusChanged(AirportStatusChangedEvent evt) => _features?.OnAirportStatusChanged(evt);

    public override void OnRunwayStatusChanged(RunwayStatusChangedEvent evt) => _features?.OnRunwayStatusChanged(evt);

    public override void OnAirportConfigurationsUpdated(AirportConfigurationsUpdatedEvent evt) => _features?.OnAirportConfigurationsUpdated(evt);

    public override void OnControllerConnected(ControllerConnectedEvent evt) => _features?.OnControllerConnected(evt);

    public override void OnControllerDisconnected(ControllerDisconnectedEvent evt) => _features?.OnControllerDisconnected(evt);

    public override void OnAtisLinesUpdate(AtisLinesUpdateEvent evt) => _features?.OnAtisLinesUpdate(evt);

    public override void OnCapabilitiesUpdate(CapabilitiesUpdateEvent evt) => _features?.OnCapabilitiesUpdate(evt);

    public override void OnIsControllerATC(IsControllerATCEvent evt) => _features?.OnIsControllerATC(evt);

    public override void OnControllerDataUpdated(ControllerDataUpdatedEvent evt) => _features?.OnControllerDataUpdated(evt);

    public override void OnAircraftBeaconCodeChanged(AircraftBeaconCodeChangedEvent evt) => _features?.OnAircraftBeaconCodeChanged(evt);

    public override void OnAircraftHandoffCancelled(AircraftHandoffCancelledEvent evt) => _features?.OnAircraftHandoffCancelled(evt);

    public override void OnAircraftOwnedByChanged(AircraftOwnedByChangedEvent evt) => _features?.OnAircraftOwnedByChanged(evt);

    public override void OnAircraftHandoffRejected(AircraftHandoffRejectedEvent evt) => _features?.OnAircraftHandoffRejected(evt);

    public override void OnAircraftHandoffInitiated(AircraftHandoffInitiatedEvent evt) => _features?.OnAircraftHandoffInitiated(evt);

    public override void OnAircraftTerminatedTracking(AircraftTerminatedTrackingEvent evt) => _features?.OnAircraftTerminatedTracking(evt);

    public override void OnAircraftInitiatedTracking(AircraftInitiatedTrackingEvent evt) => _features?.OnAircraftInitiatedTracking(evt);

    public override void OnAircraftTemporaryAltitudeChanged(AircraftTemporaryAltitudeChangedEvent evt) => _features?.OnAircraftTemporaryAltitudeChanged(evt);

    public override void OnAircraftCDMStatusChanged(AircraftCDMStatusChangedEvent evt) => _features?.OnAircraftCDMStatusChanged(evt);

    public override void OnAircraftScratchpadUpdated(AircraftScratchpadUpdatedEvent evt) => _features?.OnAircraftScratchpadUpdated(evt);

    public override void OnAircraftHeadingChanged(AircraftHeadingChangedEvent evt) => _features?.OnAircraftHeadingChanged(evt);

    public override void OnAircraftAssignedSpeedChanged(AircraftAssignedSpeedChangedEvent evt) => _features?.OnAircraftAssignedSpeedChanged(evt);

    public override void OnFlightplanUpdated(FlightplanUpdatedEvent evt) => _features?.OnFlightplanUpdated(evt);

    public override void OnFlightplanRemoved(FlightplanRemovedEvent evt) => _features?.OnFlightplanRemoved(evt);

    public override void OnFlightplanVoiceTypeChanged(FlightplanVoiceTypeChangedEvent evt) => _features?.OnFlightplanVoiceTypeChanged(evt);

    public override void OnFlightplanRouteChanged(FlightplanRouteChangedEvent evt) => _features?.OnFlightplanRouteChanged(evt);

    public override void OnFsdError(FsdErrorEvent evt) => _features?.OnFsdError(evt);

    public override void OnFsdConnectionStateChange(FsdConnectionStateChangeEvent evt) => _features?.OnFsdConnectionStateChange(evt);

    public override void OnFsdConnected(FsdConnectedEvent evt) => _features?.OnFsdConnected(evt);

    public override void OnFsdDisconnected(FsdDisconnectedEvent evt) => _features?.OnFsdDisconnected(evt);

    public override void OnFsdConnectionModelUpdated(FsdConnectionModelUpdatedEvent evt) => _features?.OnFsdConnectionModelUpdated(evt);

    public override void OnATISInfoMessageReceived(ATISInfoMessageReceivedEvent evt) => _features?.OnATISInfoMessageReceived(evt);

    public override void OnPrivateMessageReceived(PrivateMessageReceivedEvent evt) => _features?.OnPrivateMessageReceived(evt);

    public override void OnFrequencyMessageReceived(FrequencyMessageReceivedEvent evt) => _features?.OnFrequencyMessageReceived(evt);

    public override void OnBroadcastMessageReceived(BroadcastMessageReceivedEvent evt) => _features?.OnBroadcastMessageReceived(evt);

    public override void OnSupervisorMessageReceived(SupervisorMessageReceivedEvent evt) => _features?.OnSupervisorMessageReceived(evt);

    public override void OnServerMessageReceived(ServerMessageReceivedEvent evt) => _features?.OnServerMessageReceived(evt);

    public override void OnAtcMessageReceived(AtcMessageReceivedEvent evt) => _features?.OnAtcMessageReceived(evt);

    public override void OnSquawkAssigned(SquawkAssignedEvent evt) => _features?.OnSquawkAssigned(evt);

    public override void OnTagAction(TagActionEvent evt) => _features?.OnTagAction(evt);

    public override void OnTagDropdownAction(DropdownActionEvent evt) => _features?.OnTagDropdownAction(evt);

    public override bool OnTagShowDropdown(TagShowDropdownEvent evt) => _features?.OnTagShowDropdown(evt) ?? true;

    public override void OnWindowEvent(WindowEvent evt) => _features?.OnWindowEvent(evt);

    public override void OnSelectedAircraftChanged(SelectedAircraftChangedEvent evt) => _features?.OnSelectedAircraftChanged(evt);

    public override void OnActiveProfileChanged(ActiveProfileChangedEvent evt) => _features?.OnActiveProfileChanged(evt);

    public override void OnTimer(TimerEvent evt) => _features?.OnTimer(evt);

    public override void OnMenuItemClicked(MenuItemClickedEvent evt) => _features?.OnMenuItemClicked(evt);
}
