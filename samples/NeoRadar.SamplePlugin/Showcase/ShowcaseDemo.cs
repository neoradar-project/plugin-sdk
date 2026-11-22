using System;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>One demo a Showcase button or input runs, by its id; an input passes its text, a button an empty one.</summary>
internal sealed record ShowcaseDemo(string Id, Action<string> Run)
{
    public ShowcaseDemo(string id, Action run)
        : this(id, _ => run())
    {
    }
}
