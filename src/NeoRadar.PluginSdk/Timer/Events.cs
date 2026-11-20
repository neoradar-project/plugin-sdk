namespace NeoRadar.PluginSdk.Timer;

/// <summary>
/// Represents one tick of the host timer.
/// </summary>
/// <param name="Tick">Whole seconds since the host started ticking; a monotonic counter that restarts at zero each run.</param>
public sealed record TimerEvent(long Tick);
