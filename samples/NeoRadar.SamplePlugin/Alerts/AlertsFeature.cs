using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.ControllerData;
using NeoRadar.PluginSdk.Feedback;
using NeoRadar.PluginSdk.Focus;
using NeoRadar.PluginSdk.Fsd;
using NeoRadar.PluginSdk.Sound;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Common.Scheduling;

namespace NeoRadar.SamplePlugin.Alerts;

/// <summary>Plays a sound when a handoff is offered to the user, shows toasts, and follows the selected aircraft.</summary>
internal sealed class AlertsFeature : IFeature, IAlerts, IFocusEventHandler, IControllerDataEventHandler
{
    private readonly AlertActions _actions;
    private readonly AlertsProcessor _processor;

    public AlertsFeature(
        ClientInformation client,
        IFeedbackApi feedback,
        ISoundApi sound,
        IFocusApi focus,
        IAircraftApi aircraft,
        IFsdApi fsd,
        ITimedTaskScheduler scheduler,
        IPluginEventBus bus)
    {
        AlertSounds sounds = AlertSounds.In(client);
        AlertActivity activity = new(bus);
        _actions = new AlertActions(feedback, sound, focus, aircraft, scheduler, sounds, activity);
        _processor = new AlertsProcessor(sound, aircraft, fsd, bus, sounds, _actions, activity);
    }

    public void Initialize()
    {
    }

    public void Shutdown()
    {
    }

    public void Toast(FeedbackKind kind) => _actions.Toast(kind);

    public void PlayWav() => _actions.PlayWav();

    public void PlayMp3() => _actions.PlayMp3();

    public void SetFollowSelected(bool follow) => _actions.SetFollowSelected(follow);

    public void SelectNextAircraft() => _actions.SelectNextAircraft();

    public void OnSelectedAircraftChanged(SelectedAircraftChangedEvent evt) => _processor.HandleSelectedAircraftChanged(evt);

    public void OnAircraftHandoffInitiated(AircraftHandoffInitiatedEvent evt) => _processor.HandleHandoffInitiated(evt);
}
