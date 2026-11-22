using NeoRadar.PluginSdk.Feedback;

namespace NeoRadar.SamplePlugin.Alerts;

/// <summary>Toasts, sounds and the selection; all of it stays on this client.</summary>
internal interface IAlerts
{
    void Toast(FeedbackKind kind);

    void PlayWav();

    void PlayMp3();

    void SetFollowSelected(bool follow);

    void SelectNextAircraft();
}
