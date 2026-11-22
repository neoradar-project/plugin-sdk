using NeoRadar.PluginSdk.Tag;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeTagApi : ITagApi
{
    public FakeTagInterface Interface { get; } = new();

    public ITagInterface GetInterface() => Interface;
}
