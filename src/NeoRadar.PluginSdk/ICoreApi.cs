namespace NeoRadar.PluginSdk;

/// <summary>
/// Modular Core API surface plugins receive in <see cref="BasePlugin.Initialize"/>.
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

    /// <summary>Play a sound file the plugin ships with itself.</summary>
    Sound.ISoundApi Sound { get; }

    /// <summary>Read and set the client's selected aircraft.</summary>
    Focus.IFocusApi Focus { get; }

    /// <summary>Read-only runway, navaid and airport lookups.</summary>
    Navdata.INavdataApi Navdata { get; }

    /// <summary>Show a transient toast in the client.</summary>
    Feedback.IFeedbackApi Feedback { get; }

    /// <summary>Persisted string settings, scoped to the plugin or to the active profile.</summary>
    Settings.ISettingsApi Settings { get; }

    /// <summary>Named entries the plugin adds to the client's plugin list.</summary>
    Menu.IMenuApi Menu { get; }
}
