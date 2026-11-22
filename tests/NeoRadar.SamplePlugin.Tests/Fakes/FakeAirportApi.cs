using System.Collections.Generic;
using System.Linq;
using NeoRadar.PluginSdk.Airport;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeAirportApi : IAirportApi
{
    public List<AirportConfig> Configurations { get; } = new();

    public List<string> Calls { get; } = new();

    public bool Answer { get; set; } = true;

    public IReadOnlyList<AirportConfig> GetConfigurations() => Configurations.ToArray();

    public AirportConfig? GetConfigurationByIcao(string icao) => Configurations.Find(c => c.Icao == icao);

    public bool IsDepRunwayActive(string icao, string runway) => GetConfigurationByIcao(icao)?.DepRunways.Contains(runway) ?? false;

    public bool IsArrRunwayActive(string icao, string runway) => GetConfigurationByIcao(icao)?.ArrRunways.Contains(runway) ?? false;

    public bool SetRunwayStatus(string icao, string runway, RunwayType type) => Call($"set {icao} {runway} {type}");

    public bool SetShowRunwayCenterlines(string icao, bool show) => Call($"centrelines {icao} {show}");

    public bool RemoveRunwayStatus(string icao, string runway, RunwayType type) => Call($"remove {icao} {runway} {type}");

    public BatchUpdateRunwaysResult BatchUpdateRunways(IReadOnlyList<RunwayStatusChange> toAdd, IReadOnlyList<RunwayStatusChange> toRemove)
    {
        Call($"batch {toAdd.Count} {toRemove.Count}");
        return new BatchUpdateRunwaysResult(toAdd, [], toRemove, []);
    }

    public bool DeleteAirport(string icao) => Call($"delete {icao}");

    public bool SetAirportStatus(string icao, AirportStatus status) => Call($"status {icao} {status}");

    private bool Call(string call)
    {
        Calls.Add(call);
        return Answer;
    }
}
