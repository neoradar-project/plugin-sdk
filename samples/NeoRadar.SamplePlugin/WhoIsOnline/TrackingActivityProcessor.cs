using NeoRadar.PluginSdk.ControllerData;

namespace NeoRadar.SamplePlugin.WhoIsOnline;

/// <summary>Notes who tracks which aircraft: assumes, drops, owner changes and handoffs.</summary>
internal sealed class TrackingActivityProcessor
{
    private readonly NetworkActivity _activity;

    public TrackingActivityProcessor(NetworkActivity activity) => _activity = activity;

    // Client threads, every handler below.
    public void HandleInitiatedTracking(AircraftInitiatedTrackingEvent evt) => _activity.Note($"You assumed {evt.Callsign}");

    public void HandleOwnedByChanged(AircraftOwnedByChangedEvent evt)
        => _activity.Note($"{evt.Callsign} tracked by {NetworkActivity.OrNobody(evt.NewOwner)}, was {NetworkActivity.OrNobody(evt.OldOwner)}");

    public void HandleTerminatedTracking(AircraftTerminatedTrackingEvent evt) => _activity.Note($"{evt.Callsign} dropped");

    public void HandleHandoffInitiated(AircraftHandoffInitiatedEvent evt) => _activity.Note($"{evt.Callsign} handoff {evt.FromController} to {evt.ToController}");

    public void HandleHandoffRejected(AircraftHandoffRejectedEvent evt) => _activity.Note($"{evt.Callsign} handoff rejected by {evt.From}");

    public void HandleHandoffCancelled(AircraftHandoffCancelledEvent evt) => _activity.Note($"{evt.Callsign} handoff withdrawn");
}
