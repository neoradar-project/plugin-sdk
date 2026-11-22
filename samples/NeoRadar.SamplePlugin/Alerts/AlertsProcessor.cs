using System;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.ControllerData;
using NeoRadar.PluginSdk.Focus;
using NeoRadar.PluginSdk.Fsd;
using NeoRadar.PluginSdk.Sound;
using NeoRadar.SamplePlugin.Alerts.Events;
using NeoRadar.SamplePlugin.Common.Events;

namespace NeoRadar.SamplePlugin.Alerts;

/// <summary>What the client says that the alerts demo reacts to: a new selection to follow, a handoff offered to the user.</summary>
internal sealed class AlertsProcessor
{
    private readonly ISoundApi _sound;
    private readonly IAircraftApi _aircraft;
    private readonly IFsdApi _fsd;
    private readonly IPluginEventBus _bus;
    private readonly AlertSounds _sounds;
    private readonly AlertActions _actions;
    private readonly AlertActivity _activity;

    public AlertsProcessor(
        ISoundApi sound,
        IAircraftApi aircraft,
        IFsdApi fsd,
        IPluginEventBus bus,
        AlertSounds sounds,
        AlertActions actions,
        AlertActivity activity)
    {
        _sound = sound;
        _aircraft = aircraft;
        _fsd = fsd;
        _bus = bus;
        _sounds = sounds;
        _actions = actions;
        _activity = activity;
    }

    // UI thread, and also after the selected aircraft has left.
    public void HandleSelectedAircraftChanged(SelectedAircraftChangedEvent evt)
    {
        _activity.Record($"Selected {evt.Callsign ?? "nothing"}");
        if (!_actions.IsFollowingSelection || evt.Callsign is null) return;
        Aircraft? aircraft = _aircraft.GetByCallsign(evt.Callsign);
        if (aircraft is null) return;
        _bus.Publish(new SelectedAircraftFollowed(aircraft.Callsign, aircraft.Position.Latitude, aircraft.Position.Longitude));
    }

    // Client thread.
    public void HandleHandoffInitiated(AircraftHandoffInitiatedEvent evt)
    {
        string? me = _fsd.GetConnection()?.Callsign;
        if (me is null || !string.Equals(evt.ToController, me, StringComparison.OrdinalIgnoreCase)) return;
        bool played = _sound.Play(_sounds.WavPath);
        _activity.Record($"{evt.Callsign} offered to you: sound {(played ? "played" : "not played (play the .wav once first)")}");
    }
}
