using NeoRadar.PluginSdk.Window;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeWindowApi : IWindowApi
{
    public FakeWindowInterface Interface { get; } = new();

    public IWindowInterface GetInterface() => Interface;
}
