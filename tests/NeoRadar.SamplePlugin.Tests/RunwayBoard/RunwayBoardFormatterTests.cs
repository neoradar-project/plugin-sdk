using NeoRadar.SamplePlugin.RunwayBoard;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.RunwayBoard;

public sealed class RunwayBoardFormatterTests
{
    [Fact]
    public void ANaNElevation_ReadsUnknown_AndANumberInFeet()
    {
        Assert.Equal("unknown", RunwayBoardFormatter.Feet(double.NaN));
        Assert.Equal("1,234 ft", RunwayBoardFormatter.Feet(1234));
    }

    [Theory]
    [InlineData(114_350, "114.350 MHz")]
    [InlineData(339, "339 kHz")]
    public void ANavaidFrequency_ReadsInMHzForVhf_AndKHzForAnNdb(double kilohertz, string expected)
        => Assert.Equal(expected, RunwayBoardFormatter.Frequency(kilohertz));
}
