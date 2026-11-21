namespace NeoRadar.PluginSdk.Timer;

/// <summary>
/// Represents one tick of the host timer.
/// </summary>
/// <param name="Tick">The beat's count: the first beat is 0 and each beat after it one more, a skipped beat included; restarts at zero each run.</param>
public sealed record TimerEvent(long Tick);
