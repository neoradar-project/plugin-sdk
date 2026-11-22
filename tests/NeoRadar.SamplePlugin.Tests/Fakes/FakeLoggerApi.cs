using System.Collections.Generic;
using NeoRadar.PluginSdk.Logger;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeLoggerApi : ILoggerApi
{
    private readonly object _sync = new();
    private readonly List<(LogLevel Level, string Message)> _lines = new();

    public IReadOnlyList<(LogLevel Level, string Message)> Lines
    {
        get { lock (_sync) return _lines.ToArray(); }
    }

    public void Log(LogLevel level, string message)
    {
        lock (_sync) _lines.Add((level, message));
    }

    public void Fatal(string message) => Log(LogLevel.Fatal, message);

    public void Error(string message) => Log(LogLevel.Error, message);

    public void Warning(string message) => Log(LogLevel.Warning, message);

    public void Info(string message) => Log(LogLevel.Info, message);

    public void Debug(string message) => Log(LogLevel.Debug, message);

    public void Verbose(string message) => Log(LogLevel.Verbose, message);
}
