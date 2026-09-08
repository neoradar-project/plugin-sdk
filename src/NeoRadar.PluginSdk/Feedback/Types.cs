namespace NeoRadar.PluginSdk.Feedback;

/// <summary>Severity of a transient feedback toast; the client picks the colour from it.</summary>
public enum FeedbackKind
{
    /// <summary>Neutral information.</summary>
    Info,

    /// <summary>An action completed.</summary>
    Success,

    /// <summary>Something needs the controller's attention but nothing failed.</summary>
    Warning,

    /// <summary>An action failed.</summary>
    Error,
}
