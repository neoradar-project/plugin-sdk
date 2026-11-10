using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NeoRadar.PluginSdk.Tests.PublicApi;

// Each pair differs in one way a plugin would notice; the baseline only catches what the describer writes down.
public sealed class PublicApiSurfaceTests
{
    [Fact]
    public void TheSameShape_DescribesTheSameMembers()
        => Assert.Equal(Members(typeof(TextStamp)), Members(typeof(TextStampAgain)));

    [Fact]
    public void ARetypedProperty_ChangesItsLine()
        => Assert.NotEqual(Members(typeof(TextStamp)), Members(typeof(TimeStamp)));

    [Fact]
    public void AReturnTurnedNonNullable_ChangesItsLine()
        => Assert.NotEqual(Members(typeof(INullableLookup)), Members(typeof(INonNullableLookup)));

    [Fact]
    public void InitTurnedSet_ChangesItsLine()
        => Assert.NotEqual(Members(typeof(TextStamp)), Members(typeof(SettableTextStamp)));

    [Fact]
    public void AChangedParameterDefault_ChangesItsLine()
        => Assert.NotEqual(Members(typeof(IDefaultTwelve)), Members(typeof(IDefaultFourteen)));

    [Fact]
    public void AnInterfaceMemberLosingItsDefaultBody_ChangesItsLine()
        => Assert.NotEqual(Members(typeof(IWithDefault)), Members(typeof(IWithoutDefault)));

    [Fact]
    public void ARemovedMember_LeavesItsLineOut()
    {
        string[] full = Members(typeof(TwoMembers));
        string[] reduced = Members(typeof(OneMember));

        Assert.Single(full.Except(reduced));
        Assert.Empty(reduced.Except(full));
    }

    private static string[] Members(Type type) =>
        PublicApiSurface.Describe(new[] { type })
            .Where(l => l.Contains(" :: ", StringComparison.Ordinal))
            .Select(l => l[(l.IndexOf(" :: ", StringComparison.Ordinal) + 4)..])
            .ToArray();

    private sealed class TextStamp
    {
        public string Stamp { get; init; } = string.Empty;
    }

    private sealed class TextStampAgain
    {
        public string Stamp { get; init; } = string.Empty;
    }

    private sealed class TimeStamp
    {
        public DateTimeOffset Stamp { get; init; }
    }

    private sealed class SettableTextStamp
    {
        public string Stamp { get; set; } = string.Empty;
    }

    private interface INullableLookup
    {
        string? Find(string key);
    }

    private interface INonNullableLookup
    {
        string Find(string key);
    }

    private interface IDefaultTwelve
    {
        void Write(string text, float size = 12f);
    }

    private interface IDefaultFourteen
    {
        void Write(string text, float size = 14f);
    }

    private interface IWithDefault
    {
        bool Raise(int id) => false;
    }

    private interface IWithoutDefault
    {
        bool Raise(int id);
    }

    private sealed class TwoMembers
    {
        public int First { get; init; }
        public IReadOnlyList<int> Second { get; init; } = Array.Empty<int>();
    }

    private sealed class OneMember
    {
        public int First { get; init; }
    }
}
