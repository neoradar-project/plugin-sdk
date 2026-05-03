using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Controller;

public enum Rating
{
    OBS = 1,
    S1 = 2,
    S2 = 3,
    S3 = 4,
    C1 = 5,
    C2 = 6,
    C3 = 7,
    I1 = 8,
    I2 = 9,
    I3 = 10,
    SUP = 11,
    ADM = 12,
}

public enum FacilityType
{
    OBS = 0,
    FSS = 1,
    DEL = 2,
    GND = 3,
    TWR = 4,
    APP = 5,
    DEP = 6,
    CTR = 7,
}

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
