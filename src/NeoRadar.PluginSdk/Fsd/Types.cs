using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Fsd;

/// <summary>
/// Specifies the state of the FSD connection.
/// </summary>
public enum ConnectionState
{
    /// <summary>
    /// Not connected.
    /// </summary>
    Disconnected = 0,

    /// <summary>
    /// A connection is being made.
    /// </summary>
    Connecting = 1,

    /// <summary>
    /// Connected.
    /// </summary>
    Connected = 2,

    /// <summary>
    /// The connection attempt failed.
    /// </summary>
    Failed = 3,
}

/// <summary>
/// Specifies the user's network rating.
/// </summary>
public enum NetworkRating
{
    /// <summary>
    /// Observer.
    /// </summary>
    OBS = 1,

    /// <summary>
    /// Tower trainee.
    /// </summary>
    S1 = 2,

    /// <summary>
    /// Tower controller.
    /// </summary>
    S2 = 3,

    /// <summary>
    /// Senior student, approach controller.
    /// </summary>
    S3 = 4,

    /// <summary>
    /// Enroute controller.
    /// </summary>
    C1 = 5,

    /// <summary>
    /// Controller 2, not issued on VATSIM.
    /// </summary>
    C2 = 6,

    /// <summary>
    /// Senior controller.
    /// </summary>
    C3 = 7,

    /// <summary>
    /// Instructor.
    /// </summary>
    I1 = 8,

    /// <summary>
    /// Instructor 2, not issued on VATSIM.
    /// </summary>
    I2 = 9,

    /// <summary>
    /// Senior instructor.
    /// </summary>
    I3 = 10,

    /// <summary>
    /// Supervisor.
    /// </summary>
    SUP = 11,

    /// <summary>
    /// Administrator.
    /// </summary>
    ADM = 12,
}

/// <summary>
/// Specifies the facility the user is connected as.
/// </summary>
public enum NetworkFacility
{
    /// <summary>
    /// Observer.
    /// </summary>
    OBS = 0,

    /// <summary>
    /// Flight service station.
    /// </summary>
    FSS = 1,

    /// <summary>
    /// Clearance delivery.
    /// </summary>
    DEL = 2,

    /// <summary>
    /// Ground.
    /// </summary>
    GND = 3,

    /// <summary>
    /// Tower.
    /// </summary>
    TWR = 4,

    /// <summary>
    /// Approach.
    /// </summary>
    APP = 5,

    /// <summary>
    /// Area control centre.
    /// </summary>
    CTR = 6,
}

/// <summary>
/// Specifies the kind of FSD server connected to.
/// </summary>
public enum ServerType
{
    /// <summary>
    /// The server kind is not known.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The live network.
    /// </summary>
    Live = 1,

    /// <summary>
    /// A test server.
    /// </summary>
    Test = 2,

    /// <summary>
    /// A sweatbox training server.
    /// </summary>
    Sweatbox = 3,
}

/// <summary>
/// Represents the user's FSD connection details.
/// </summary>
/// <param name="Callsign">The callsign the user is connected as.</param>
/// <param name="Cid">The user's network ID.</param>
/// <param name="Name">The user's real name.</param>
/// <param name="Rating">The user's network rating.</param>
/// <param name="IsATC">Whether the user is a working ATC position.</param>
/// <param name="Facility">The facility the user is connected as.</param>
/// <param name="ServerType">The kind of server connected to.</param>
/// <param name="IsConnected">Whether the connection is up.</param>
/// <param name="AtisLines">The user's ATIS lines.</param>
/// <param name="Frequencies">The user's frequencies in kHz.</param>
public sealed record ConnectionInfo(
    string Callsign,
    int Cid,
    string Name,
    NetworkRating Rating,
    bool IsATC,
    NetworkFacility Facility,
    ServerType ServerType,
    bool IsConnected,
    IReadOnlyList<string> AtisLines,
    IReadOnlyList<int> Frequencies);
