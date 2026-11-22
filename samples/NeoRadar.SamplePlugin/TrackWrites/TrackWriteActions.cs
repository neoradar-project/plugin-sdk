using System;
using NeoRadar.PluginSdk.ControllerData;
using NeoRadar.PluginSdk.Focus;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.TrackWrites.Events;

namespace NeoRadar.SamplePlugin.TrackWrites;

/// <summary>Each write: the gate and the selection first, then the call, then the tally published.</summary>
internal sealed class TrackWriteActions : ITrackWrites
{
    public const string DemoController = "SAMPLE_CTR";
    public const string DemoScratchpad = "SAMPLE";
    public const int DemoClearedLevelFeet = 12_000;
    public const int BurstSize = 7;

    private readonly IControllerDataApi _data;
    private readonly IFocusApi _focus;
    private readonly DemoWritesGate _gate;
    private readonly IPluginEventBus _bus;
    private readonly WriteTally _tally = new();

    public TrackWriteActions(IControllerDataApi data, IFocusApi focus, DemoWritesGate gate, IPluginEventBus bus)
    {
        _data = data;
        _focus = focus;
        _gate = gate;
        _bus = bus;
    }

    public void SetScratchpad() => OnSelected("SetScratchpad", cs => _data.SetScratchpad(cs, DemoScratchpad));

    public void SetClearedLevel() => OnSelected("SetClearedFlightLevel", cs => _data.SetClearedFlightLevel(cs, DemoClearedLevelFeet));

    public void Assume() => OnSelected("AssumeAircraft", _data.AssumeAircraft);

    public void Release() => OnSelected("ReleaseAircraft", _data.ReleaseAircraft);

    public void AcceptHandoffWithNoOffer() => OnSelected("AcceptHandoff", cs => _data.AcceptHandoff(cs, DemoController));

    public void RejectHandoffWithNoOffer() => OnSelected("RejectHandoff", cs => _data.RejectHandoff(cs, DemoController));

    public void CancelHandoff() => OnSelected("CancelHandoff", cs => _data.CancelHandoff(cs, DemoController));

    public void SetNextController() => OnSelected("SetNextController", cs => _data.SetNextController(cs, DemoController));

    public void BurstScratchpad()
    {
        for (int i = 0; i < BurstSize; i++)
            OnSelected("SetScratchpad (burst)", cs => _data.SetScratchpad(cs, $"{DemoScratchpad}{i}"));
    }

    /// <summary>Writes the text a controller typed into a tag's input box as the aircraft's scratchpad.</summary>
    public void SetScratchpadFromInput(string callsign, string text) => OnCallsign("SetScratchpad (typed)", callsign, cs => _data.SetScratchpad(cs, text));

    private void OnSelected(string what, Func<string, bool> write)
    {
        string callsign = _focus.GetSelectedPlanCallsign();
        if (callsign.Length == 0)
        {
            Publish(n => n.Note($"{what}: select an aircraft first"));
            return;
        }
        OnCallsign(what, callsign, write);
    }

    private void OnCallsign(string what, string callsign, Func<string, bool> write)
    {
        if (!_gate.IsOpen)
        {
            Publish(n => n.Note($"{what}: tick Demo writes first; nothing was sent"));
            return;
        }

        bool sent = write(callsign);
        Publish(n =>
        {
            n.Record($"{what} {callsign}", sent);
            n.Note(Describe(callsign));
        });
    }

    private string Describe(string callsign)
        => _data.GetByCallsign(callsign) is ControllerDataModel data
            ? $"{callsign} now: cleared {data.ClearedFlightLevel} ft, scratchpad '{data.Scratchpad}', tracked by {(data.OwnedByCallsign.Length == 0 ? "nobody" : data.OwnedByCallsign)}, {data.AttentionState}"
            : $"{callsign} has no controller data yet";

    private void Publish(Action<WriteTally> change)
    {
        change(_tally);
        _bus.Publish(new TrackWritesTallied(_tally.Lines()));
    }
}
