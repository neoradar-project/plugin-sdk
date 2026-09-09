using System;
using Xunit;

namespace NeoRadar.PluginSdk.Tests;

public sealed class PluginSdkVersionTests
{
    // The host compares the SDK version a plugin was compiled against with this assembly's version,
    // so the csproj Version and the constants must never drift.
    [Fact]
    public void AssemblyVersion_MatchesTheConstants()
    {
        Version assembly = typeof(PluginSdkVersion).Assembly.GetName().Version!;

        Assert.Equal(PluginSdkVersion.Major, assembly.Major);
        Assert.Equal(PluginSdkVersion.Minor, assembly.Minor);
        Assert.Equal(PluginSdkVersion.Patch, assembly.Build);
    }

    // The released contract version. Changing it is a deliberate edit that moves the csproj
    // Version, both consumers' submodule pins and the v<version> tag in the same round.
    [Fact]
    public void Current_IsTheReleasedVersion()
    {
        Assert.Equal(0, PluginSdkVersion.Major);
        Assert.Equal(5, PluginSdkVersion.Minor);
        Assert.Equal(0, PluginSdkVersion.Patch);
        Assert.Equal("0.5.0", PluginSdkVersion.Current);
    }

    [Fact]
    public void Current_MatchesTheConstants()
        => Assert.Equal(
            $"{PluginSdkVersion.Major}.{PluginSdkVersion.Minor}.{PluginSdkVersion.Patch}",
            PluginSdkVersion.Current);

    [Fact]
    public void BuildId_IsStampedFromTheCommit()
    {
        Assert.NotEqual("unknown", PluginSdkVersion.BuildId);
        Assert.True(PluginSdkVersion.BuildId.Length >= 7, PluginSdkVersion.BuildId);
    }
}
