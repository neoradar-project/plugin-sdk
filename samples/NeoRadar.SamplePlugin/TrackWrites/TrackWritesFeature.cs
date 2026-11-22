using NeoRadar.PluginSdk.ControllerData;
using NeoRadar.PluginSdk.Focus;
using NeoRadar.PluginSdk.Logger;
using NeoRadar.PluginSdk.Tag;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;

namespace NeoRadar.SamplePlugin.TrackWrites;

/// <summary>Writes a scratchpad, a level and the tracking calls through the host's rate limit, and shows what it refused.</summary>
internal sealed class TrackWritesFeature : IFeature, ITagEventHandler, ITrackWrites
{
    public const string ScratchpadActionName = "Scratchpad";

    private readonly ITagInterface _tags;
    private readonly ILoggerApi _logger;
    private readonly TrackWriteActions _actions;
    private string _scratchpadActionId = string.Empty;

    public TrackWritesFeature(HostSurfaces surfaces, IControllerDataApi data, IFocusApi focus, DemoWritesGate gate, IPluginEventBus bus, ILoggerApi logger)
    {
        _tags = surfaces.Tag;
        _logger = logger;
        _actions = new TrackWriteActions(data, focus, gate, bus);
    }

    public void Initialize()
    {
        _scratchpadActionId = _tags.RegisterTagAction(
            new TagActionDefinition(ScratchpadActionName, RequiresInput: true, "Sets the typed text as the scratchpad, once Demo writes is ticked")
            {
                Title = "Set scratchpad",
            });
        if (_scratchpadActionId.Length == 0) _logger.Warning("TrackWrites: the Scratchpad tag action was refused");
    }

    public void Shutdown()
    {
    }

    // Client thread; the text arrives once the controller submits the cell's input box.
    public void OnTagAction(TagActionEvent evt)
    {
        if (evt.ActionId != _scratchpadActionId || evt.UserInput is null) return;
        string callsign = evt.PlanCallsign.Length > 0 ? evt.PlanCallsign : evt.Callsign;
        _actions.SetScratchpadFromInput(callsign, evt.UserInput);
    }

    public void SetScratchpad() => _actions.SetScratchpad();

    public void SetClearedLevel() => _actions.SetClearedLevel();

    public void BurstScratchpad() => _actions.BurstScratchpad();

    public void Assume() => _actions.Assume();

    public void Release() => _actions.Release();

    public void AcceptHandoffWithNoOffer() => _actions.AcceptHandoffWithNoOffer();

    public void RejectHandoffWithNoOffer() => _actions.RejectHandoffWithNoOffer();

    public void CancelHandoff() => _actions.CancelHandoff();

    public void SetNextController() => _actions.SetNextController();
}
