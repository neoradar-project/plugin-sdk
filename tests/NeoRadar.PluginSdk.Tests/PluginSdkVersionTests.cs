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
    public void IsCompatible_RequiresTheExactMajorAndMinor()
    {
        Assert.True(PluginSdkVersion.IsCompatible(PluginSdkVersion.Major, PluginSdkVersion.Minor));
        Assert.False(PluginSdkVersion.IsCompatible(PluginSdkVersion.Major, PluginSdkVersion.Minor + 1));
        Assert.False(PluginSdkVersion.IsCompatible(PluginSdkVersion.Major, PluginSdkVersion.Minor - 1));
        Assert.False(PluginSdkVersion.IsCompatible(PluginSdkVersion.Major + 1, PluginSdkVersion.Minor));
    }

    [Fact]
    public void BuildId_IsStampedFromTheCommit()
    {
        Assert.NotEqual("unknown", PluginSdkVersion.BuildId);
        Assert.True(PluginSdkVersion.BuildId.Length >= 7, PluginSdkVersion.BuildId);
    }

    [Fact]
    public void DescribeBuildMismatch_NamesBothBuilds()
    {
        string message = PluginSdkVersion.DescribeBuildMismatch("UK Radar Plugin", "0123456789ab");

        Assert.Contains("0123456789ab", message);
        Assert.Contains(PluginSdkVersion.BuildId, message);
    }

    [Fact]
    public void DescribeMismatch_NamesBothVersions()
    {
        string message = PluginSdkVersion.DescribeMismatch("UK Radar Plugin", new Version(0, 9, 0, 0));

        Assert.Contains("0.9", message);
        Assert.Contains(PluginSdkVersion.Current, message);
        Assert.DoesNotContain("\u2014", message);
    }
}
