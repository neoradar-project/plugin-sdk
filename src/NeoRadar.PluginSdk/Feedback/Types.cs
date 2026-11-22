namespace NeoRadar.PluginSdk.Feedback;

/// <summary>
/// Specifies the severity of a toast, from which the client picks its colour.
/// </summary>
public enum FeedbackKind
{
    /// <summary>
    /// Neutral information.
    /// </summary>
    Info,

    /// <summary>
    /// An action completed.
    /// </summary>
    Success,

    /// <summary>
    /// Something needs the controller's attention but nothing failed.
    /// </summary>
    Warning,

    /// <summary>
    /// An action failed.
    /// </summary>
    Error,
}
