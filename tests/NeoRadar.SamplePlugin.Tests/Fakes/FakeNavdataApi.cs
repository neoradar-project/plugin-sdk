using System.Collections.Generic;
using NeoRadar.PluginSdk.Navdata;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeNavdataApi : INavdataApi
{
    public Dictionary<string, List<RunwayInfo>> Runways { get; } = new();

    public Dictionary<string, List<NavaidInfo>> Navaids { get; } = new();

    public Dictionary<string, AirportInfo> Airports { get; } = new();

    public IReadOnlyList<RunwayInfo> GetRunways(string icao) => Runways.TryGetValue(icao, out List<RunwayInfo>? r) ? r : [];

    public IReadOnlyList<NavaidInfo> GetNavaids(string identifier) => Navaids.TryGetValue(identifier, out List<NavaidInfo>? n) ? n : [];

    public AirportInfo? GetAirport(string icao) => Airports.GetValueOrDefault(icao);
}
