using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Controller;

/// <summary>
/// Specifies a controller's network rating.
/// </summary>
public enum Rating
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
/// Specifies the facility a controller is working.
/// </summary>
public enum FacilityType
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
    /// Departure.
    /// </summary>
    DEP = 6,

    /// <summary>
    /// Area control centre.
    /// </summary>
    CTR = 7,
}

/// <summary>
/// Represents a snapshot of a connected controller.
/// </summary>
/// <param name="Callsign">The controller's callsign.</param>
/// <param name="Name">The controller's real name.</param>
/// <param name="Cid">The controller's network ID.</param>
/// <param name="Rating">The controller's network rating.</param>
/// <param name="Facility">The facility the controller is working.</param>
/// <param name="Latitude">The latitude in decimal degrees.</param>
/// <param name="Longitude">The longitude in decimal degrees.</param>
/// <param name="Frequencies">The controller's frequencies in kHz.</param>
/// <param name="AtisLines">The controller's ATIS lines.</param>
/// <param name="AtisLetter">The current ATIS letter, or empty when there is none.</param>
/// <param name="IsATC">Whether the controller is a working ATC position.</param>
/// <param name="IsATIS">Whether the connection is an ATIS station.</param>
/// <param name="SectorFileName">The sector file the controller reports using.</param>
public sealed record Controller(
    string Callsign,
    string Name,
    string Cid,
    Rating Rating,
    FacilityType Facility,
    double Latitude,
    double Longitude,
    IReadOnlyList<int> Frequencies,
    IReadOnlyList<string> AtisLines,
    string AtisLetter,
    bool IsATC,
    bool IsATIS,
    string SectorFileName);
