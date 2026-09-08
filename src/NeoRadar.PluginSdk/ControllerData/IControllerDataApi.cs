using System.Collections.Generic;

namespace NeoRadar.PluginSdk.ControllerData;

public interface IControllerDataApi
{
    IReadOnlyList<ControllerDataModel> GetAll();

    ControllerDataModel? GetByCallsign(string callsign);

    /// <summary>Set ground status. Rate-limited to roughly one call per second per plugin; excess calls return false.</summary>
    bool SetGroundStatus(string callsign, GroundStatus groundStatus);
}
