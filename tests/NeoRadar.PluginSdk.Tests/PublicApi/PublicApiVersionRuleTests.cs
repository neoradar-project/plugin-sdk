using System;
using Xunit;

namespace NeoRadar.PluginSdk.Tests.PublicApi;

public sealed class PublicApiVersionRuleTests
{
    private static readonly Version s_frozen = new(0, 9, 2);

    [Theory]
    [InlineData("0.9.2", 0, 0)]
    [InlineData("0.9.3", 1, 0)]
    [InlineData("0.10.0", 1, 0)]
    [InlineData("1.0.0", 1, 0)]
    [InlineData("0.10.0", 0, 1)]
    [InlineData("0.10.0", 3, 2)]
    [InlineData("1.0.0", 0, 1)]
    public void AStepThatFitsTheChange_IsAccepted(string current, int added, int removed)
        => Assert.Null(PublicApiVersionRule.Refusal(s_frozen, Version.Parse(current), added, removed));

    [Theory]
    [InlineData("0.9.2", 1, 0)]
    [InlineData("0.9.4", 1, 0)]
    [InlineData("0.9.1", 1, 0)]
    public void AnAddition_WithoutTheNextPatch_IsRefused(string current, int added, int removed)
    {
        string? refusal = PublicApiVersionRule.Refusal(s_frozen, Version.Parse(current), added, removed);

        Assert.NotNull(refusal);
        Assert.Contains("0.9.3", refusal);
    }

    [Theory]
    [InlineData("0.9.2", 0, 1)]
    [InlineData("0.9.3", 0, 1)]
    [InlineData("0.9.3", 4, 1)]
    [InlineData("0.10.1", 0, 1)]
    [InlineData("0.11.0", 0, 1)]
    public void ABreak_WithoutTheNextMinor_IsRefused(string current, int added, int removed)
    {
        string? refusal = PublicApiVersionRule.Refusal(s_frozen, Version.Parse(current), added, removed);

        Assert.NotNull(refusal);
        Assert.Contains("0.10.0", refusal);
    }

    [Theory]
    [InlineData("0.9.3")]
    [InlineData("0.10.0")]
    public void AVersionMove_WithNoPublicChange_IsRefused(string current)
        => Assert.NotNull(PublicApiVersionRule.Refusal(s_frozen, Version.Parse(current), 0, 0));
}
