using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Flightplan;

public interface IFlightplanApi
{
    IReadOnlyList<Flightplan> GetAll();

    Flightplan? GetByCallsign(string callsign);
}
