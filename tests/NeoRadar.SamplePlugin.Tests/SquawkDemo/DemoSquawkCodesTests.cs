using System.Collections.Generic;
using System.Linq;
using NeoRadar.SamplePlugin.SquawkDemo;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.SquawkDemo;

public sealed class DemoSquawkCodesTests
{
    [Fact]
    public void EveryCode_IsFourOctalDigits_From4601To4677_AndTheyWrap()
    {
        List<string> codes = Enumerable.Range(0, DemoSquawkCodes.Count).Select(i => DemoSquawkCodes.At(i)).ToList();

        Assert.All(codes, c => Assert.Matches("^46[0-7][0-7]$", c));
        Assert.Equal(DemoSquawkCodes.Count, codes.Distinct().Count());
        Assert.Equal("4601", codes[0]);
        Assert.Equal("4677", codes[^1]);
        Assert.Equal("4601", DemoSquawkCodes.At(DemoSquawkCodes.Count));
    }
}
