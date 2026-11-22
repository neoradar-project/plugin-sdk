using System;
using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>Every registered feature's demos, found by id; filled once at composition.</summary>
internal sealed class ShowcaseDemoCatalogue
{
    private readonly IReadOnlyList<IShowcaseDemos> _sources;
    private readonly Dictionary<string, ShowcaseDemo> _byId = new(StringComparer.Ordinal);

    /// <exception cref="InvalidOperationException">Two demos share an id.</exception>
    public ShowcaseDemoCatalogue(IEnumerable<IShowcaseDemos> sources)
    {
        _sources = [.. sources];
        foreach (IShowcaseDemos source in _sources)
            foreach (ShowcaseDemo demo in source.Demos)
                if (!_byId.TryAdd(demo.Id, demo)) throw new InvalidOperationException($"Two Showcase demos have the id {demo.Id}");
    }

    public IReadOnlyCollection<string> Ids => _byId.Keys;

    public ShowcaseDemo? Find(string id) => _byId.GetValueOrDefault(id);

    public List<IDisposable> ShowReports()
    {
        List<IDisposable> subscriptions = new();
        foreach (IShowcaseDemos source in _sources) subscriptions.AddRange(source.ShowReports());
        return subscriptions;
    }
}
