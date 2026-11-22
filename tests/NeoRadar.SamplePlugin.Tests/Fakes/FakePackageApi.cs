using NeoRadar.PluginSdk.Package;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakePackageApi : IPackageApi
{
    public string? Path { get; set; } = @"C:\Packages\Test";

    public string? GetPackagePath() => Path;
}
