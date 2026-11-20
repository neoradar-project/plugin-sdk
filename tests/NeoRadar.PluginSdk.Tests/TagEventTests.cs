using System.Collections.Generic;
using NeoRadar.PluginSdk.Tag;
using Xunit;

namespace NeoRadar.PluginSdk.Tests;

public sealed class TagEventTests
{
    // A plugin built before the plan callsign existed constructs both events positionally and still compiles.
    [Fact]
    public void BothEvents_TakeThePlanCallsignByInit_AndLeaveItEmptyOtherwise()
    {
        Dictionary<string, string> data = new();

        TagActionEvent action = new("a", "t", "ABC123", 0, null, data) { PlanCallsign = "BAW456" };
        DropdownActionEvent dropdown = new("a", "c", "t", "ABC123", null, data);

        Assert.Equal("ABC123", action.Callsign);
        Assert.Equal("BAW456", action.PlanCallsign);
        Assert.Equal("", dropdown.PlanCallsign);
    }

    [Fact]
    public void TheShowDropdownEvent_CarriesTheTrackAndThePlanItStandsFor()
    {
        TagShowDropdownEvent evt = new() { ActionId = "a", Callsign = "ABC123", PlanCallsign = "BAW456" };

        Assert.Equal("a", evt.ActionId);
        Assert.Equal("ABC123", evt.Callsign);
        Assert.Equal("BAW456", evt.PlanCallsign);
    }
}
