namespace NeoRadar.PluginSdk.Timer;

/// <summary>
/// Represents one tick of the host timer.
/// </summary>
/// <param name="Tick">The beat's count from 0, a skipped beat included, which restarts at 0 each run.</param>
public sealed record TimerEvent(long Tick);
