using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.TrackWrites;

/// <summary>How many writes the host took and refused, and what each recent one was.</summary>
internal sealed class WriteTally
{
    private readonly object _sync = new();
    private readonly RecentLines _recent = new(10);
    private int _sent;
    private int _refused;

    public void Record(string what, bool sent)
    {
        lock (_sync)
        {
            if (sent) _sent++;
            else _refused++;
        }
        _recent.Add($"{what}: {(sent ? "sent" : "refused")}");
    }

    public void Note(string line) => _recent.Add(line);

    public IReadOnlyList<string> Lines()
    {
        List<string> lines;
        lock (_sync) lines = [$"Sent {_sent}, refused {_refused}"];
        lines.AddRange(_recent.Snapshot());
        return lines;
    }
}
