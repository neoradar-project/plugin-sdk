using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Controller;

public interface IControllerApi
{
    IReadOnlyList<Controller> GetAll();

    Controller? GetByCallsign(string callsign);
}
