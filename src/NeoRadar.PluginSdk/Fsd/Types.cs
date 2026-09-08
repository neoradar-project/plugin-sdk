using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Fsd;

public enum ConnectionState
{
    Disconnected = 0,
    Connecting = 1,
    Connected = 2,
    Failed = 3,
}

public enum NetworkRating
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

public enum NetworkFacility
{
    OBS = 0,
    FSS = 1,
    DEL = 2,
    GND = 3,
    TWR = 4,
    APP = 5,
    CTR = 6,
}

public enum ServerType
{
    Unknown = 0,
    Live = 1,
    Test = 2,
    Sweatbox = 3,
}

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
