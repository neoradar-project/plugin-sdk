namespace NeoRadar.PluginSdk.Fsd;

/// <summary>
/// Represents an error reported on the FSD connection.
/// </summary>
/// <param name="Message">The error message.</param>
public sealed record FsdErrorEvent(string Message);

/// <summary>
/// Represents the FSD connection state changing.
/// </summary>
/// <param name="State">The new connection state.</param>
public sealed record FsdConnectionStateChangeEvent(ConnectionState State);

/// <summary>
/// Represents the user connecting to FSD.
/// </summary>
/// <param name="Callsign">The callsign the user connected as.</param>
/// <param name="Server">The name of the <see cref="ServerType"/> connected to, such as "Live".</param>
/// <param name="FacilityType">The numeric facility the user connected as.</param>
public sealed record FsdConnectedEvent(string Callsign, string Server, int FacilityType);

/// <summary>
/// Represents the user disconnecting from FSD.
/// </summary>
/// <param name="Reason">The reason given for the disconnection.</param>
/// <param name="UserInitiated">Whether the user chose to disconnect.</param>
public sealed record FsdDisconnectedEvent(string Reason, bool UserInitiated);

/// <summary>
/// Represents the user's own connection details changing.
/// </summary>
/// <param name="Callsign">The user's callsign.</param>
public sealed record FsdConnectionModelUpdatedEvent(string Callsign);
