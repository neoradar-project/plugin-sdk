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
    /// <remarks>Rate limited per plugin to a burst of five, then one call per second; a throttled call is dropped silently.</remarks>
    /// <param name="text">The toast's text; an empty one is dropped.</param>
    /// <param name="kind">The severity, which picks the toast's theme colours.</param>
    /// <param name="durationMs">How long the toast shows, in milliseconds; a call with 0 or less is dropped without spending the rate limit.</param>
    void Show(string text, FeedbackKind kind = FeedbackKind.Info, int durationMs = 2000);
}
