using System.Collections.Generic;
using NeoRadar.PluginSdk.Focus;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeFocusApi : IFocusApi
{
    public string? Selected { get; set; }

    public List<string> SelectCalls { get; } = new();

    public string? GetSelectedAircraft() => Selected;

    public string GetSelectedPlanCallsign() => Selected ?? string.Empty;

    public bool SelectAircraft(string callsign)
    {
        SelectCalls.Add(callsign);
        Selected = callsign;
        return true;
    }
}
