using System.Collections.Generic;
using NeoRadar.PluginSdk.Flightplan;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeFlightplanApi : IFlightplanApi
{
    public List<Flightplan> All { get; } = new();

    public IReadOnlyList<Flightplan> GetAll() => All.ToArray();

    public Flightplan? GetByCallsign(string callsign) => All.Find(f => f.Callsign == callsign);
}
