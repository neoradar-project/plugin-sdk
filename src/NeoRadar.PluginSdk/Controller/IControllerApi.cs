using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Controller;

/// <summary>
/// Provides read access to connected controllers.
/// </summary>
public interface IControllerApi
{
    /// <summary>
    /// Gets every connected controller.
    /// </summary>
    IReadOnlyList<Controller> GetAll();

    /// <summary>
    /// Gets a controller by callsign, or null when it is not connected.
    /// </summary>
    Controller? GetByCallsign(string callsign);
}
