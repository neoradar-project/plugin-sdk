namespace NeoRadar.PluginSdk.Feedback;

/// <summary>
/// Transient on-screen confirmations. One toast is visible at a time and a new one replaces the
/// current one, so this is for a confirmation the controller may miss, never for a notice that
/// must be reviewable.
/// </summary>
public interface IFeedbackApi
{
    /// <summary>
    /// Show a transient toast in the client for <paramref name="durationMs"/> milliseconds. Rate
    /// limited to roughly one call per second per plugin; a throttled call is dropped silently.
    /// </summary>
    void Show(string text, FeedbackKind kind = FeedbackKind.Info, int durationMs = 2000);
}
