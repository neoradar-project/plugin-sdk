using NeoRadar.PluginSdk.Fsd;

namespace NeoRadar.SamplePlugin.WhoIsOnline;

/// <summary>Notes what happened to the user's own FSD connection.</summary>
internal sealed class FsdActivityProcessor
{
    private readonly NetworkActivity _activity;

    public FsdActivityProcessor(NetworkActivity activity) => _activity = activity;

    // Client threads, every handler below.
    public void HandleConnected(FsdConnectedEvent evt) => _activity.Note($"FSD connected as {evt.Callsign} to {evt.Server}");

    public void HandleDisconnected(FsdDisconnectedEvent evt) => _activity.Note($"FSD disconnected ({evt.Reason}, by you: {evt.UserInitiated})");

    public void HandleConnectionStateChange(FsdConnectionStateChangeEvent evt) => _activity.Note($"FSD state {evt.State}");

    public void HandleConnectionModelUpdated(FsdConnectionModelUpdatedEvent evt) => _activity.Note($"Your connection now {evt.Callsign}");

    public void HandleError(FsdErrorEvent evt) => _activity.Note($"FSD error: {evt.Message}");
}
