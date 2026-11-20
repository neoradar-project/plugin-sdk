namespace NeoRadar.PluginSdk.Feedback;

/// <summary>
/// Provides transient on-screen toasts for confirmations the controller may safely miss.
/// </summary>
/// <remarks>One toast shows at a time and a new one replaces it, so use chat for anything that must be reviewable.</remarks>
public interface IFeedbackApi
{
    /// <summary>
    /// Shows a toast for <paramref name="durationMs"/> milliseconds.
    /// </summary>
    /// <remarks>Rate limited to roughly one call per second per plugin, and a throttled call is dropped silently.</remarks>
    void Show(string text, FeedbackKind kind = FeedbackKind.Info, int durationMs = 2000);
}
