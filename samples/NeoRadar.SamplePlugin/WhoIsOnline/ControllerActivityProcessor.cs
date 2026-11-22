using NeoRadar.PluginSdk.Controller;

namespace NeoRadar.SamplePlugin.WhoIsOnline;

/// <summary>Notes what other controllers did: connecting, leaving, their ATIS and capabilities.</summary>
internal sealed class ControllerActivityProcessor
{
    private readonly NetworkActivity _activity;

    public ControllerActivityProcessor(NetworkActivity activity) => _activity = activity;

    // Client threads, every handler below.
    public void HandleControllerConnected(ControllerConnectedEvent evt) => _activity.Note($"{evt.Callsign} connected as {evt.Facility}");

    public void HandleControllerDisconnected(ControllerDisconnectedEvent evt) => _activity.Note($"{evt.Callsign} disconnected");

    public void HandleAtisLinesUpdate(AtisLinesUpdateEvent evt) => _activity.Note($"{evt.Callsign} ATIS now {evt.LineCount} lines");

    public void HandleCapabilitiesUpdate(CapabilitiesUpdateEvent evt) => _activity.Note($"{evt.Callsign} capabilities changed");

    public void HandleIsControllerATC(IsControllerATCEvent evt) => _activity.Note($"{evt.Callsign} is ATC: {evt.IsATC}");
}
