namespace NeoRadar.PluginSdk;

/// <summary>
/// Provides the host API a plugin receives in <see cref="BasePlugin.Initialize"/>.
/// </summary>
/// <remarks>
/// No member throws into a plugin: queries return null or empty, registrations return an empty id, commands return false.
/// </remarks>
public interface ICoreApi
{
    /// <summary>
    /// Gets the API for the active package.
    /// </summary>
    Package.IPackageApi Package { get; }

    /// <summary>
    /// Gets the API for connected aircraft.
    /// </summary>
    Aircraft.IAircraftApi Aircraft { get; }

    /// <summary>
    /// Gets the API for filed flight plans.
    /// </summary>
    Flightplan.IFlightplanApi Flightplan { get; }

    /// <summary>
    /// Gets the API for connected controllers.
    /// </summary>
    Controller.IControllerApi Controller { get; }

    /// <summary>
    /// Gets the API for reading and writing controller-assigned aircraft state.
    /// </summary>
    ControllerData.IControllerDataApi ControllerData { get; }

    /// <summary>
    /// Gets the API for airport and runway configuration.
    /// </summary>
    Airport.IAirportApi Airport { get; }

    /// <summary>
    /// Gets the API for squawk providers.
    /// </summary>
    Squawk.ISquawkApi Squawk { get; }

    /// <summary>
    /// Gets the API for tag items and tag actions.
    /// </summary>
    Tag.ITagApi Tag { get; }

    /// <summary>
    /// Gets the API for writing to the plugin's log.
    /// </summary>
    Logger.ILoggerApi Logger { get; }

    /// <summary>
    /// Gets the API for the user's FSD connection.
    /// </summary>
    Fsd.IFsdApi Fsd { get; }

    /// <summary>
    /// Gets the API for text messages.
    /// </summary>
    Chat.IChatApi Chat { get; }

    /// <summary>
    /// Gets the API for drawing on radar screens.
    /// </summary>
    Rendering.IRenderingApi Rendering { get; }

    /// <summary>
    /// Gets the API for plugin windows.
    /// </summary>
    Window.IWindowApi Window { get; }

    /// <summary>
    /// Gets the API for playing sound files the plugin ships with.
    /// </summary>
    Sound.ISoundApi Sound { get; }

    /// <summary>
    /// Gets the API for reading and setting the selected aircraft.
    /// </summary>
    Focus.IFocusApi Focus { get; }

    /// <summary>
    /// Gets the API for read-only runway, navaid and airport lookups.
    /// </summary>
    Navdata.INavdataApi Navdata { get; }

    /// <summary>
    /// Gets the API for showing toasts in the client.
    /// </summary>
    Feedback.IFeedbackApi Feedback { get; }

    /// <summary>
    /// Gets the API for persisted settings scoped to the plugin or the active profile.
    /// </summary>
    Settings.ISettingsApi Settings { get; }

    /// <summary>
    /// Gets the API for entries the plugin adds to the client's plugin menu.
    /// </summary>
    Menu.IMenuApi Menu { get; }

    /// <summary>
    /// Gets the API for the client's theme colours.
    /// </summary>
    Theme.IThemeApi Theme { get; }

    /// <summary>
    /// Gets the API for typed values the plugin exports to package conditions.
    /// </summary>
    Expressions.IExpressionApi Expressions { get; }
}
