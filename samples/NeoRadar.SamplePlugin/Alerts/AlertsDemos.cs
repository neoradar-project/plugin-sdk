using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Feedback;
using NeoRadar.SamplePlugin.Alerts.Events;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Showcase;

namespace NeoRadar.SamplePlugin.Alerts;

/// <summary>The sound and focus page's demos, the rate limits page's toast burst, and the alerts readout.</summary>
internal sealed class AlertsDemos : IShowcaseDemos
{
    public const int ToastBurstSize = 7;

    private readonly IAlerts _alerts;
    private readonly IPluginEventBus _bus;
    private readonly ShowcaseWindow _window;

    public AlertsDemos(IAlerts alerts, IPluginEventBus bus, ShowcaseWindow window)
    {
        _alerts = alerts;
        _bus = bus;
        _window = window;
    }

    public IReadOnlyList<ShowcaseDemo> Demos =>
    [
        new("ToastInfo", () => _alerts.Toast(FeedbackKind.Info)),
        new("ToastWarning", () => _alerts.Toast(FeedbackKind.Warning)),
        new("ToastError", () => _alerts.Toast(FeedbackKind.Error)),
        new("ToastBurst", ShowToastBurst),
        new("PlayWav", _alerts.PlayWav),
        new("PlayMp3", _alerts.PlayMp3),
        new("FollowOn", () => _alerts.SetFollowSelected(true)),
        new("FollowOff", () => _alerts.SetFollowSelected(false)),
        new("SelectNext", _alerts.SelectNextAircraft),
    ];

    public IReadOnlyList<IDisposable> ShowReports() => [_bus.Subscribe<AlertActivityRecorded>(e => _window.ShowReadout(ShowcaseGuide.Alerts, e.Lines))];

    private void ShowToastBurst()
    {
        for (int i = 0; i < ToastBurstSize; i++) _alerts.Toast(FeedbackKind.Info);
    }
}
