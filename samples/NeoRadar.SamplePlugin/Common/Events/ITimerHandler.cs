using NeoRadar.PluginSdk.Timer;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>The host's one-second tick, on a client thread and never re-entered.</summary>
internal interface ITimerHandler
{
    void OnTimer(TimerEvent evt) { }
}
