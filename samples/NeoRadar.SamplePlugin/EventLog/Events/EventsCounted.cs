using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.EventLog.Events;

/// <summary>The event log counted the host events heard so far; <c>Recent</c> is newest first.</summary>
internal sealed record EventsCounted(IReadOnlyList<EventCount> Counts, IReadOnlyList<string> Recent);
