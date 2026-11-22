using System.Collections.Generic;
using NeoRadar.PluginSdk.ControllerData;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

// Accepts AllowedWrites calls, then refuses every later one, as the host's rate limit does after a burst.
internal sealed class FakeControllerDataApi : IControllerDataApi
{
    public int AllowedWrites { get; set; } = int.MaxValue;

    public List<string> Writes { get; } = new();

    public List<ControllerDataModel> All { get; } = new();

    public IReadOnlyList<ControllerDataModel> GetAll() => All.ToArray();

    public ControllerDataModel? GetByCallsign(string callsign) => All.Find(d => d.Callsign == callsign);

    public bool SetGroundStatus(string callsign, GroundStatus groundStatus) => Write($"ground {callsign} {groundStatus}");

    public bool SetAssignedSquawk(string callsign, string squawk) => Write($"squawk {callsign} {squawk}");

    public bool SetClearedFlightLevel(string callsign, int clearedFlightLevel) => Write($"cfl {callsign} {clearedFlightLevel}");

    public bool SetAssignedHeading(string callsign, int heading) => Write($"heading {callsign} {heading}");

    public bool SetAssignedSpeed(string callsign, int speed, bool isMach, AssignedSpeedVariance variance) => Write($"speed {callsign} {speed}");

    public bool ClearAssignedSpeed(string callsign) => Write($"clear speed {callsign}");

    public bool SetScratchpad(string callsign, string scratchpad) => Write($"scratchpad {callsign} {scratchpad}");

    public bool SetAssignedDirect(string callsign, string waypointIdentifier) => Write($"direct {callsign} {waypointIdentifier}");

    public bool ClearAssignedDirect(string callsign) => Write($"clear direct {callsign}");

    public bool SetNextController(string callsign, string nextControllerCallsign) => Write($"next {callsign} {nextControllerCallsign}");

    public bool SetClearanceIssued(string callsign, bool issued) => Write($"clearance {callsign} {issued}");

    public bool AssumeAircraft(string callsign) => Write($"assume {callsign}");

    public bool ReleaseAircraft(string callsign) => Write($"release {callsign}");

    public bool HandoffAircraft(string callsign, string toController) => Write($"handoff {callsign} {toController}");

    public bool AcceptHandoff(string callsign, string fromController) => Write($"accept {callsign} {fromController}");

    public bool RejectHandoff(string callsign, string fromController) => Write($"reject {callsign} {fromController}");

    public bool CancelHandoff(string callsign, string targetController) => Write($"cancel {callsign} {targetController}");

    public IReadOnlyList<string>? GetFlightStripAnnotations(string callsign) => null;

    public bool SetFlightStripAnnotation(string callsign, int index, string text) => Write($"strip {callsign} {index} {text}");

    public bool PushFlightStrip(string callsign, string toController) => Write($"push strip {callsign} {toController}");

    private bool Write(string call)
    {
        Writes.Add(call);
        if (AllowedWrites <= 0) return false;
        AllowedWrites--;
        return true;
    }
}
