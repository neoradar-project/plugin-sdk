namespace NeoRadar.PluginSdk.Timer;

/// <summary>
/// One tick of the host timer. <c>Tick</c> counts whole seconds since the host started ticking,
/// so it is a monotonic counter, not a wall clock, and it restarts at zero on every client run.
/// </summary>
public sealed record TimerEvent(long Tick);
