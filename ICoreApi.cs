namespace NeoRadar.PluginSdk;

/// <summary>
/// Modular Core API surface plugins receive in <see cref="BasePlugin.Initialize"/>.
/// Mirrors <c>PluginSDK::CoreAPI</c> in the C++ neoradar-sdk one-to-one.
/// </summary>
public interface ICoreApi
{
    Package.IPackageApi Package { get; }

    Aircraft.IAircraftApi Aircraft { get; }

    Flightplan.IFlightplanApi Flightplan { get; }

    Controller.IControllerApi Controller { get; }

    ControllerData.IControllerDataApi ControllerData { get; }

    Airport.IAirportApi Airport { get; }

    Squawk.ISquawkApi Squawk { get; }

    Tag.ITagApi Tag { get; }

    Logger.ILoggerApi Logger { get; }

    Fsd.IFsdApi Fsd { get; }

    Chat.IChatApi Chat { get; }

    Rendering.IRenderingApi Rendering { get; }

    Window.IWindowApi Window { get; }
}
