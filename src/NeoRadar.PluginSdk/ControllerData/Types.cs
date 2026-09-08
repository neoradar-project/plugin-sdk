namespace NeoRadar.PluginSdk.ControllerData;

public enum AssignedSpeedVariance
{
    None = 0,
    Plus = 1,
    Minus = 2,
}

public enum AttentionState
{
    Unconcerned = 0,
    Notified = 1,
    IncomingTransfer = 2,
    Assumed = 3,
    OutgoingTransfer = 4,
    Intruder = 5,
    Concerned = 6,
}

public enum GroundStatus
{
    None = 0,
    Start = 1,
    Push = 2,
    Taxi = 3,
    Dep = 4,
}

public sealed record ControllerDataModel(
    string Callsign,
    int ClearedFlightLevel,
    string AssignedDirect,
    int AssignedHeading,
    int AssignedSpeed,
    int AssignedMach,
    int AssignedVerticalRate,
    string AssignedSquawk,
    string Scratchpad,
    string RawScratchpad,
    bool ClearanceIssued,
    bool OwnedByMe,
    string OwnedByCallsign,
    string FutureOwnerCallsign,
    AssignedSpeedVariance AssignedSpeedVariance,
    AttentionState AttentionState,
    GroundStatus GroundStatus,
    int ClearanceQueuePosition);
