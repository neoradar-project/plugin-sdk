using System;
using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.TagDemo;

/// <summary>Each aircraft's dropdown choices, read and written on whichever client thread a tag event arrives.</summary>
internal sealed class TagChoiceStore
{
    private readonly object _sync = new();
    private readonly Dictionary<string, TagChoice> _byCallsign = new(StringComparer.OrdinalIgnoreCase);

    public TagChoice For(string callsign)
    {
        lock (_sync) return _byCallsign.GetValueOrDefault(callsign, TagChoice.None);
    }

    public TagChoice Change(string callsign, Func<TagChoice, TagChoice> change)
    {
        lock (_sync)
        {
            TagChoice next = change(_byCallsign.GetValueOrDefault(callsign, TagChoice.None));
            _byCallsign[callsign] = next;
            return next;
        }
    }
}
