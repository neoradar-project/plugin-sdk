using System;
using System.Collections.Generic;
using System.Linq;
using NeoRadar.PluginSdk.Airport;
using NeoRadar.PluginSdk.Navdata;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.RunwayBoard.Events;

namespace NeoRadar.SamplePlugin.RunwayBoard;

/// <summary>Each runway board demo: the guards (an airport, a runway, Demo writes), then the call, then the board refreshed.</summary>
internal sealed class RunwayBoardActions : IRunwayBoard
{
    private readonly IAirportApi _airports;
    private readonly INavdataApi _navdata;
    private readonly DemoWritesGate _gate;
    private readonly IPluginEventBus _bus;
    private readonly RecentLines _results = new(6);

    public RunwayBoardActions(IAirportApi airports, INavdataApi navdata, DemoWritesGate gate, IPluginEventBus bus)
    {
        _airports = airports;
        _navdata = navdata;
        _gate = gate;
        _bus = bus;
    }

    public void Refresh()
    {
        List<string> lines = new();
        IReadOnlyList<AirportConfig> configs = _airports.GetConfigurations();
        if (configs.Count == 0) lines.Add("No airports configured: activate one in the Active Airports window.");
        foreach (AirportConfig config in configs.OrderBy(c => c.Icao, StringComparer.Ordinal))
            RunwayBoardFormatter.DescribeAirport(lines, config, _navdata.GetAirport(config.Icao), _navdata.GetRunways(config.Icao));
        lines.AddRange(_results.Snapshot());
        _bus.Publish(new RunwayBoardRefreshed(lines));
    }

    public void ToggleCentrelines()
        => OnFirstAirport("SetShowRunwayCenterlines", false, a => _airports.SetShowRunwayCenterlines(a.Icao, !a.ShowCenterlines));

    public void ActivateDepartureRunway()
        => OnFirstRunway("SetRunwayStatus", (a, rwy) => _airports.SetRunwayStatus(a.Icao, rwy, RunwayType.Departure));

    public void DeactivateDepartureRunway()
        => OnFirstRunway("RemoveRunwayStatus", (a, rwy) => _airports.RemoveRunwayStatus(a.Icao, rwy, RunwayType.Departure));

    public void SwapRunwaysInOneBatch()
        => OnFirstRunway("BatchUpdateRunways", (a, rwy) =>
        {
            RunwayStatusChange change = new(a.Icao, rwy, RunwayType.Arrival);
            bool active = a.ArrRunways.Contains(rwy);
            BatchUpdateRunwaysResult result = active
                ? _airports.BatchUpdateRunways([], [change])
                : _airports.BatchUpdateRunways([change], []);
            return result.AddedSuccess.Count + result.RemovedSuccess.Count > 0;
        });

    public void ToggleObserved()
        => OnFirstAirport("SetAirportStatus", true, a => _airports.SetAirportStatus(
            a.Icao, a.Status == AirportStatus.Active ? AirportStatus.Observed : AirportStatus.Active));

    public void DeleteAirport() => OnFirstAirport("DeleteAirport", true, a => _airports.DeleteAirport(a.Icao));

    public void LookUpNavaid(string identifier)
    {
        IReadOnlyList<NavaidInfo> navaids = _navdata.GetNavaids(identifier.Trim().ToUpperInvariant());
        if (navaids.Count == 0) _results.Add($"Navaid {identifier}: not in the navdata");
        foreach (NavaidInfo navaid in navaids.Take(3)) _results.Add(RunwayBoardFormatter.Navaid(navaid));
        Refresh();
    }

    private void OnFirstRunway(string what, Func<AirportConfig, string, bool> write)
        => OnFirstAirport(what, true, airport =>
        {
            string? runway = _navdata.GetRunways(airport.Icao).Select(r => r.HighEndIdent).FirstOrDefault();
            if (runway is null)
            {
                _results.Add($"{what}: {airport.Icao} has no runway in the navdata");
                return false;
            }
            return write(airport, runway);
        });

    private void OnFirstAirport(string what, bool needsDemoWrites, Func<AirportConfig, bool> write)
    {
        AirportConfig? airport = _airports.GetConfigurations().OrderBy(c => c.Icao, StringComparer.Ordinal).FirstOrDefault();
        if (airport is null) _results.Add($"{what}: no airport configured");
        else if (needsDemoWrites && !_gate.IsOpen) _results.Add($"{what}: tick Demo writes first; nothing was changed");
        else _results.Add($"{what} {airport.Icao}: {(write(airport) ? "applied" : "refused")}");
        Refresh();
    }
}
