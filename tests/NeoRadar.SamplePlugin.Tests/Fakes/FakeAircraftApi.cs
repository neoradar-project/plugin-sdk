using System.Collections.Generic;
using NeoRadar.PluginSdk.Aircraft;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeAircraftApi : IAircraftApi
{
    public List<Aircraft> All { get; } = new();

    public Dictionary<string, double> FromOrigin { get; } = new();

    public Dictionary<string, double> ToDestination { get; } = new();

    public int GetAllCalls { get; private set; }

    public IReadOnlyList<Aircraft> GetAll()
    {
        GetAllCalls++;
        return All.ToArray();
    }

    public Aircraft? GetByCallsign(string callsign) => All.Find(a => a.Callsign == callsign);

    public double? GetDistanceFromOrigin(string callsign) => FromOrigin.TryGetValue(callsign, out double d) ? d : null;

    public double? GetDistanceToDestination(string callsign) => ToDestination.TryGetValue(callsign, out double d) ? d : null;
}
