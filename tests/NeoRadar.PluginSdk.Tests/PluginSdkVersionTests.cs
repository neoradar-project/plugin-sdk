using System;
using Xunit;

namespace NeoRadar.PluginSdk.Tests;

public sealed class PluginSdkVersionTests
{
    // The host compares plugin versions with this assembly, so the csproj Version and constants must match.
    [Fact]
    public void AssemblyVersion_MatchesTheConstants()
    {
        Version assembly = typeof(PluginSdkVersion).Assembly.GetName().Version!;

        Assert.Equal(PluginSdkVersion.Major, assembly.Major);
        Assert.Equal(PluginSdkVersion.Minor, assembly.Minor);
        Assert.Equal(PluginSdkVersion.Patch, assembly.Build);
    }

    // Changing this moves the csproj Version, both consumers' submodule pins and the v<version> tag together.
    [Fact]
    public void Current_IsTheReleasedVersion()
    {
        Assert.Equal(0, PluginSdkVersion.Major);
        Assert.Equal(4, PluginSdkVersion.Minor);
        Assert.Equal(0, PluginSdkVersion.Patch);
        Assert.Equal("0.4.0", PluginSdkVersion.Current);
    }

    [Fact]
    public void Current_MatchesTheConstants()
        => Assert.Equal(
            $"{PluginSdkVersion.Major}.{PluginSdkVersion.Minor}.{PluginSdkVersion.Patch}",
            PluginSdkVersion.Current);
}
