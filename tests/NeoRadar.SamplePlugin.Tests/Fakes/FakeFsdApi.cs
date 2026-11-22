using NeoRadar.PluginSdk.Fsd;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeFsdApi : IFsdApi
{
    public ConnectionInfo? Connection { get; set; }

    public ConnectionInfo? GetConnection() => Connection;
}
