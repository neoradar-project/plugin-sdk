namespace NeoRadar.SamplePlugin.EventLog.Events;

/// <summary>How many times one host event has arrived.</summary>
internal sealed record EventCount(string EventName, int Count);
