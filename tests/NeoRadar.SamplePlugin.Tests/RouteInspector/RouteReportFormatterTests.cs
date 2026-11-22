using System.Collections.Generic;
using NeoRadar.PluginSdk.Flightplan;
using NeoRadar.SamplePlugin.RouteInspector;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.RouteInspector;

public sealed class RouteReportFormatterTests
{
    [Theory]
    [InlineData(114_350_000, "114.350 MHz")]
    [InlineData(339_000, "339 kHz")]
    [InlineData(338_500, "338.5 kHz")]
    public void AFrequency_ReadsInMHzForVhf_AndKHzForAnNdb(int hertz, string expected)
        => Assert.Equal(expected, RouteReportFormatter.Frequency(hertz));

    [Fact]
    public void AFiledPlan_ListsEachLegWithItsPointKind_TheDirect_TheErrors_AndTheRemarksWhole()
    {
        RouteWaypoint cpt = new() { Identifier = "CPT", Type = WaypointType.VORDME, FrequencyHz = 114_350_000 };
        RouteWaypoint kenet = new() { Identifier = "KENET", Type = WaypointType.FIX };
        Flightplan plan = new()
        {
            Callsign = "BAW1",
            Origin = "EGLL",
            Destination = "EGPH",
            AcType = "A320",
            FlightRule = "I",
            Remarks = "PBN/A1B1 /V/ RMK/LONG REMARKS STAY WHOLE",
            Route = new Route
            {
                RawRoute = "CPT UL9 KENET",
                Segments = [new ParsedRouteSegment(cpt, kenet, "UL9")],
                Errors = [new ParsingError(ParsingErrorType.AIRWAY_FIX_NOT_FOUND, "not on airway", 2, "XX", ParsingErrorLevel.PARSE_ERROR)],
                HasDirectApplied = true,
                CurrentDirectWaypoint = kenet,
            },
        };

        IReadOnlyList<string> lines = RouteReportFormatter.Describe("BAW1", plan, 12.34, null);

        Assert.Contains("From origin 12.3 NM, to destination unknown", lines);
        Assert.Contains("Direct to KENET [FIX]", lines);
        Assert.Contains("  CPT [VORDME 114.350 MHz] -UL9-> KENET [FIX]", lines);
        Assert.Contains("  AIRWAY_FIX_NOT_FOUND at token 2 'XX': not on airway", lines);
        Assert.Contains("Remarks: PBN/A1B1 /V/ RMK/LONG REMARKS STAY WHOLE", lines);
    }

    [Fact]
    public void NoPlan_SaysSo()
        => Assert.Equal(["BAW1: no flight plan filed"], RouteReportFormatter.Describe("BAW1", null, null, null));
}
