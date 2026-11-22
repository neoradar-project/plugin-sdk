using NeoRadar.PluginSdk.Airport;
using NeoRadar.PluginSdk.Navdata;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;

namespace NeoRadar.SamplePlugin.RunwayBoard;

/// <summary>The airports the controller has configured, their runways from the navdata, and a navaid lookup.</summary>
internal sealed class RunwayBoardFeature : IFeature, IRunwayBoard, IAirportEventHandler
{
    private readonly RunwayBoardActions _actions;

    public RunwayBoardFeature(IAirportApi airports, INavdataApi navdata, DemoWritesGate gate, IPluginEventBus bus)
        => _actions = new RunwayBoardActions(airports, navdata, gate, bus);

    public void Initialize()
    {
    }

    public void Shutdown()
    {
    }

    public void Refresh() => _actions.Refresh();

    public void ToggleCentrelines() => _actions.ToggleCentrelines();

    public void ActivateDepartureRunway() => _actions.ActivateDepartureRunway();

    public void DeactivateDepartureRunway() => _actions.DeactivateDepartureRunway();

    public void SwapRunwaysInOneBatch() => _actions.SwapRunwaysInOneBatch();

    public void ToggleObserved() => _actions.ToggleObserved();

    public void DeleteAirport() => _actions.DeleteAirport();

    public void LookUpNavaid(string identifier) => _actions.LookUpNavaid(identifier);

    // Client threads.
    public void OnAirportAdded(AirportAddedEvent evt) => _actions.Refresh();

    public void OnAirportRemoved(AirportRemovedEvent evt) => _actions.Refresh();

    public void OnAirportStatusChanged(AirportStatusChangedEvent evt) => _actions.Refresh();

    public void OnRunwayStatusChanged(RunwayStatusChangedEvent evt) => _actions.Refresh();

    public void OnAirportConfigurationsUpdated(AirportConfigurationsUpdatedEvent evt) => _actions.Refresh();
}
