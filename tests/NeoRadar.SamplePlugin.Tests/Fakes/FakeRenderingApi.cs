using NeoRadar.PluginSdk.Rendering;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeRenderingApi : IRenderingApi
{
    public FakeRenderingInterface Interface { get; } = new();

    public IRenderingInterface GetInterface() => Interface;
}
