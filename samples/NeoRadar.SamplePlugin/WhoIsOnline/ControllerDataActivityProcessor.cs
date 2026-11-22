using NeoRadar.PluginSdk.ControllerData;

namespace NeoRadar.SamplePlugin.WhoIsOnline;

/// <summary>Notes changes to an aircraft's controller data: clearance, scratchpad, heading, speed, level and squawk.</summary>
internal sealed class ControllerDataActivityProcessor
{
    private readonly NetworkActivity _activity;

    public ControllerDataActivityProcessor(NetworkActivity activity) => _activity = activity;

    // Client threads, every handler below.
    public void HandleCDMStatusChanged(AircraftCDMStatusChangedEvent evt) => _activity.Note($"{evt.Callsign} clearance {evt.ClearanceIssued}, ground {evt.GroundStatus}");

    public void HandleScratchpadUpdated(AircraftScratchpadUpdatedEvent evt) => _activity.Note($"{evt.Callsign} scratchpad '{evt.Scratchpad}'");

    public void HandleHeadingChanged(AircraftHeadingChangedEvent evt) => _activity.Note($"{evt.Callsign} heading {evt.OldHeading} to {evt.NewHeading}");

    public void HandleAssignedSpeedChanged(AircraftAssignedSpeedChangedEvent evt) => _activity.Note($"{evt.Callsign} speed {evt.OldSpeed} to {evt.NewSpeed}");

    public void HandleTemporaryAltitudeChanged(AircraftTemporaryAltitudeChangedEvent evt)
        => _activity.Note($"{evt.Callsign} cleared level {evt.OldAltitude} to {evt.NewAltitude} ft");

    public void HandleBeaconCodeChanged(AircraftBeaconCodeChangedEvent evt)
        => _activity.Note($"{evt.Callsign} squawk {NetworkActivity.OrNobody(evt.OldCode)} to {evt.NewCode}");
}
