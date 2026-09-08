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
