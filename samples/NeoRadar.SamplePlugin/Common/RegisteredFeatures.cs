using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.Common;

/// <summary>The names of the features FeatureRegistration added, in Initialize order; filled at composition only.</summary>
internal sealed class RegisteredFeatures
{
    private readonly List<string> _names = new();

    public IReadOnlyList<string> Names => _names;

    public void Add(string name) => _names.Add(name);
}
