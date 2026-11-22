using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.WhoIsOnline;

/// <summary>What the network just did, newest first and timed, shown under who is online at the next refresh.</summary>
internal sealed class NetworkActivity
{
    private readonly RecentLines _lines = new(12);

    public void Note(string line) => _lines.Add($"{DateTimeOffset.UtcNow:HH:mm:ss} {line}");

    public IReadOnlyList<string> Snapshot() => _lines.Snapshot();

    public static string OrNobody(string value) => value.Length == 0 ? "nobody" : value;
}
