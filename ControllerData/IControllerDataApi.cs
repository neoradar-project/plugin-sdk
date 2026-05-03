using System.Collections.Generic;

namespace NeoRadar.PluginSdk.ControllerData;

public interface IControllerDataApi
{
    IReadOnlyList<ControllerDataModel> GetAll();

    ControllerDataModel? GetByCallsign(string callsign);

    /// <summary>Set ground status. Rate-limited to 1 call per second per callsign.</summary>
    bool SetGroundStatus(string callsign, GroundStatus groundStatus);
}
