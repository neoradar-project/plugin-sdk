using System;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Flightplan;
using NeoRadar.PluginSdk.Focus;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.RouteInspector.Events;

namespace NeoRadar.SamplePlugin.RouteInspector;

/// <summary>Explains the selected aircraft's filed route, again whenever its plan changes.</summary>
internal sealed class RouteInspectorFeature : IFeature, IRouteInspector, IFocusEventHandler, IFlightplanEventHandler
{
    private readonly IFocusApi _focus;
    private readonly IFlightplanApi _flightplans;
    private readonly IAircraftApi _aircraft;
    private readonly IPluginEventBus _bus;

    // Written on the UI thread, read on client threads.
    private volatile string _planCallsign = string.Empty;

    public RouteInspectorFeature(IFocusApi focus, IFlightplanApi flightplans, IAircraftApi aircraft, IPluginEventBus bus)
    {
        _focus = focus;
        _flightplans = flightplans;
        _aircraft = aircraft;
        _bus = bus;
    }

    public void Initialize()
    {
    }

    public void Shutdown()
    {
    }

    public void InspectSelected()
    {
        _planCallsign = _focus.GetSelectedPlanCallsign();
        Inspect();
    }

    // UI thread.
    public void OnSelectedAircraftChanged(SelectedAircraftChangedEvent evt) => InspectSelected();

    // Client threads.
    public void OnFlightplanUpdated(FlightplanUpdatedEvent evt) => InspectIfSelected(evt.Callsign);

    public void OnFlightplanRouteChanged(FlightplanRouteChangedEvent evt) => InspectIfSelected(evt.Callsign);

    public void OnFlightplanVoiceTypeChanged(FlightplanVoiceTypeChangedEvent evt) => InspectIfSelected(evt.Callsign);

    public void OnFlightplanRemoved(FlightplanRemovedEvent evt) => InspectIfSelected(evt.Callsign);

    private void InspectIfSelected(string callsign)
    {
        if (!string.Equals(callsign, _planCallsign, StringComparison.OrdinalIgnoreCase)) return;
        Inspect();
    }

    private void Inspect()
    {
        string callsign = _planCallsign;
        if (callsign.Length == 0)
        {
            _bus.Publish(new RouteInspected(["Select an aircraft on the radar to inspect its route."]));
            return;
        }

        Flightplan? plan = _flightplans.GetByCallsign(callsign);
        double? fromOrigin = _aircraft.GetDistanceFromOrigin(callsign);
        double? toDestination = _aircraft.GetDistanceToDestination(callsign);
        _bus.Publish(new RouteInspected(RouteReportFormatter.Describe(callsign, plan, fromOrigin, toDestination)));
    }
}
