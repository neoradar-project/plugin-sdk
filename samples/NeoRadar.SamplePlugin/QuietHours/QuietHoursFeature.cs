using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Expressions;
using NeoRadar.PluginSdk.Focus;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.QuietHours.Events;

namespace NeoRadar.SamplePlugin.QuietHours;

/// <summary>A session-wide "quiet hours" flag and a per-aircraft flag, for a package's showWhen and conditions.</summary>
internal sealed class QuietHoursFeature : IFeature, IQuietHours
{
    public const string QuietHoursName = "Quiet Hours";
    public const string FlaggedName = "Flagged";

    private readonly IExpressionApi _expressions;
    private readonly IFocusApi _focus;
    private readonly IPluginEventBus _bus;
    private readonly RecentLines _pushes = new(6);
    private readonly HashSet<string> _flagged = new(StringComparer.OrdinalIgnoreCase);
    private string _quietHoursId = string.Empty;
    private string _flaggedId = string.Empty;

    public QuietHoursFeature(IExpressionApi expressions, IFocusApi focus, IPluginEventBus bus)
    {
        _expressions = expressions;
        _focus = focus;
        _bus = bus;
    }

    public void Initialize()
    {
        ExpressionDefinition quietHours = ExpressionDefinition.Boolean(QuietHoursName, defaultValue: false, "True while the controller has quiet hours on");
        _quietHoursId = _expressions.Declare(quietHours with { Scope = ExpressionScope.Session });
        _flaggedId = _expressions.Declare(
            ExpressionDefinition.Boolean(FlaggedName, defaultValue: false, "True for an aircraft the controller flagged in the sample"));
    }

    public void Shutdown()
    {
    }

    // UI thread, from a Showcase button.
    public void SetQuiet(bool quiet)
    {
        bool pushed = _expressions.PushSession(_quietHoursId, quiet);
        Report($"{_quietHoursId} = {quiet}: {(pushed ? "pushed" : "refused")}");
    }

    public void ToggleFlagOnSelected()
    {
        string? callsign = _focus.GetSelectedAircraft();
        if (callsign is null)
        {
            Report("Select an aircraft to flag it");
            return;
        }

        bool flagged;
        lock (_flagged)
        {
            flagged = !_flagged.Contains(callsign);
            if (flagged) _flagged.Add(callsign);
            else _flagged.Remove(callsign);
        }
        bool pushed = _expressions.Push(_flaggedId, callsign, flagged);
        Report($"{_flaggedId} for {callsign} = {flagged}: {(pushed ? "pushed" : "refused")}");
    }

    private void Report(string line)
    {
        _pushes.Add(line);
        _bus.Publish(new ExpressionsPushed(_pushes.Snapshot()));
    }
}
