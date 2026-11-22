using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.Common;

/// <summary>The last few lines of what a demo did, newest first, safe to add to from any thread.</summary>
internal sealed class RecentLines
{
    private readonly int _capacity;
    private readonly object _sync = new();
    private readonly LinkedList<string> _lines = new();

    public RecentLines(int capacity) => _capacity = capacity;

    public void Add(string line)
    {
        lock (_sync)
        {
            _lines.AddFirst(line);
            while (_lines.Count > _capacity) _lines.RemoveLast();
        }
    }

    public IReadOnlyList<string> Snapshot()
    {
        lock (_sync) return [.. _lines];
    }
}
