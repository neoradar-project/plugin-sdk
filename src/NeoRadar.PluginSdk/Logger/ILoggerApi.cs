namespace NeoRadar.PluginSdk.Logger;

/// <summary>
/// Specifies the severity of a log message.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// A failure the plugin cannot recover from.
    /// </summary>
    Fatal = 0,

    /// <summary>
    /// A failure of one operation.
    /// </summary>
    Error = 1,

    /// <summary>
    /// An unexpected condition that did not stop the operation.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// A normal event worth recording.
    /// </summary>
    Info = 3,

    /// <summary>
    /// Detail useful when diagnosing a problem.
    /// </summary>
    Debug = 4,

    /// <summary>
    /// Fine-grained tracing.
    /// </summary>
    Verbose = 5,
}

/// <summary>
/// Provides writing to the client's log.
/// </summary>
public interface ILoggerApi
{
    /// <summary>
    /// Writes a message at the given level.
    /// </summary>
    void Log(LogLevel level, string message);

    /// <summary>
    /// Writes a message at <see cref="LogLevel.Fatal"/>.
    /// </summary>
    void Fatal(string message);

    /// <summary>
    /// Writes a message at <see cref="LogLevel.Error"/>.
    /// </summary>
    void Error(string message);

    /// <summary>
    /// Writes a message at <see cref="LogLevel.Warning"/>.
    /// </summary>
    void Warning(string message);

    /// <summary>
    /// Writes a message at <see cref="LogLevel.Info"/>.
    /// </summary>
    void Info(string message);

    /// <summary>
    /// Writes a message at <see cref="LogLevel.Debug"/>.
    /// </summary>
    void Debug(string message);

    /// <summary>
    /// Writes a message at <see cref="LogLevel.Verbose"/>.
    /// </summary>
    void Verbose(string message);
}
