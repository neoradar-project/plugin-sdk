using System;
using System.Collections.Generic;
using System.Linq;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.EventLog.Events;

namespace NeoRadar.SamplePlugin.EventLog;

/// <summary>How many times each host event has arrived, on any thread, and the last few worth reading.</summary>
internal sealed class EventCountStore
{
    private readonly object _sync = new();
    private readonly Dictionary<string, int> _counts = new(StringComparer.Ordinal);
    private readonly RecentLines _recent = new(14);
    private long _version;

    /// <summary>Bumped on every change, so a reader can tell whether a snapshot is still current.</summary>
    public long Version => System.Threading.Interlocked.Read(ref _version);

    public void Count(string eventName)
    {
        lock (_sync) _counts[eventName] = _counts.GetValueOrDefault(eventName) + 1;
        System.Threading.Interlocked.Increment(ref _version);
    }

    public void Note(string line)
    {
        _recent.Add(line);
        System.Threading.Interlocked.Increment(ref _version);
    }

    public EventsCounted Snapshot()
    {
        EventCount[] counts;
        lock (_sync)
            counts = _counts.Select(c => new EventCount(c.Key, c.Value)).OrderBy(c => c.EventName, StringComparer.Ordinal).ToArray();
        return new EventsCounted(counts, _recent.Snapshot());
    }
}
