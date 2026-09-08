namespace NeoRadar.PluginSdk.Logger;

public enum LogLevel
{
    Fatal = 0,
    Error = 1,
    Warning = 2,
    Info = 3,
    Debug = 4,
    Verbose = 5,
}

public interface ILoggerApi
{
    void Log(LogLevel level, string message);

    void Fatal(string message);

    void Error(string message);

    void Warning(string message);

    void Info(string message);

    void Debug(string message);

    void Verbose(string message);
}
