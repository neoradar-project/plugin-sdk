using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Airport;

public sealed record BatchUpdateRunwaysResult(
    IReadOnlyList<RunwayStatusChange> AddedSuccess,
    IReadOnlyList<RunwayStatusChange> AddedFailed,
    IReadOnlyList<RunwayStatusChange> RemovedSuccess,
    IReadOnlyList<RunwayStatusChange> RemovedFailed);

public interface IAirportApi
{
    IReadOnlyList<AirportConfig> GetConfigurations();

    AirportConfig? GetConfigurationByIcao(string icao);

    bool IsDepRunwayActive(string icao, string runway);

    bool IsArrRunwayActive(string icao, string runway);

    bool SetRunwayStatus(string icao, string runway, RunwayType type);

    bool SetShowRunwayCenterlines(string icao, bool show);

    bool RemoveRunwayStatus(string icao, string runway, RunwayType type);

    BatchUpdateRunwaysResult BatchUpdateRunways(
        IReadOnlyList<RunwayStatusChange> toAdd,
        IReadOnlyList<RunwayStatusChange> toRemove);

    bool DeleteAirport(string icao);

    bool SetAirportStatus(string icao, AirportStatus status);
}
