using System;
using System.IO;
using System.Linq;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Feedback;
using NeoRadar.PluginSdk.Focus;
using NeoRadar.PluginSdk.Sound;
using NeoRadar.SamplePlugin.Common.Scheduling;

namespace NeoRadar.SamplePlugin.Alerts;

/// <summary>Toasts, sounds and the selection, as the controller asks for them from the Showcase.</summary>
internal sealed class AlertActions : IAlerts
{
    private readonly IFeedbackApi _feedback;
    private readonly ISoundApi _sound;
    private readonly IFocusApi _focus;
    private readonly IAircraftApi _aircraft;
    private readonly ITimedTaskScheduler _scheduler;
    private readonly AlertSounds _sounds;
    private readonly AlertActivity _activity;

    // Set on the UI thread by a Showcase button, read on the UI thread by the selection event.
    private volatile bool _follow;

    public AlertActions(
        IFeedbackApi feedback,
        ISoundApi sound,
        IFocusApi focus,
        IAircraftApi aircraft,
        ITimedTaskScheduler scheduler,
        AlertSounds sounds,
        AlertActivity activity)
    {
        _feedback = feedback;
        _sound = sound;
        _focus = focus;
        _aircraft = aircraft;
        _scheduler = scheduler;
        _sounds = sounds;
        _activity = activity;
    }

    public bool IsFollowingSelection => _follow;

    public void Toast(FeedbackKind kind)
    {
        _feedback.Show($"Sample {kind} toast", kind, durationMs: 3000);
        _activity.Record($"Toast {kind} shown (a throttled toast is dropped without telling the plugin)");
    }

    // File work goes to the thread pool, never the UI thread that clicked.
    public void PlayWav() => _scheduler.RunOnce("write-and-play-wav", () => Play(_sounds.WavPath, BeepWave.Build()));

    public void PlayMp3() => _scheduler.RunOnce("write-and-play-mp3", () => Play(_sounds.Mp3Path, [0xFF, 0xFB, 0x90, 0x00]));

    public void SetFollowSelected(bool follow)
    {
        _follow = follow;
        _activity.Record($"Follow the selected aircraft: {follow}");
    }

    public void SelectNextAircraft()
    {
        string current = _focus.GetSelectedAircraft() ?? string.Empty;
        string[] callsigns = _aircraft.GetAll().Select(a => a.Callsign).OrderBy(c => c, StringComparer.Ordinal).ToArray();
        string? next = callsigns.FirstOrDefault(c => string.CompareOrdinal(c, current) > 0) ?? callsigns.FirstOrDefault();
        if (next is null)
        {
            _activity.Record("No aircraft to select");
            return;
        }
        _activity.Record($"SelectAircraft({next}) returned {_focus.SelectAircraft(next)}");
    }

    private void Play(string path, byte[] content)
    {
        SoundFileWriter.WriteIfMissing(path, content);
        bool played = _sound.Play(path);
        _activity.Record($"Play({Path.GetFileName(path)}) returned {played}");
    }
}
