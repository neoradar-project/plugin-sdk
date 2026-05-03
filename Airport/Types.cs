using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Airport;

public enum AirportStatus
{
    Active = 0,
    Observed = 1,
}

public enum RunwayType
{
    Departure = 0,
    Arrival = 1,
}

public sealed record RunwayStatusChange(string Icao, string Runway, RunwayType Type);

public sealed record AirportConfig(
    string Icao,
    IReadOnlyList<string> ArrRunways,
    IReadOnlyList<string> DepRunways,
    AirportStatus Status,
    bool ShowCenterlines = true);
