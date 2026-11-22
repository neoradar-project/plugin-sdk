using NeoRadar.PluginSdk.Chat;

namespace NeoRadar.SamplePlugin.WhoIsOnline;

/// <summary>Notes each chat message the client received, by kind, and each ATIS broadcast.</summary>
internal sealed class ChatActivityProcessor
{
    private readonly NetworkActivity _activity;

    public ChatActivityProcessor(NetworkActivity activity) => _activity = activity;

    // Client threads, every handler below.
    public void HandlePrivateMessage(PrivateMessageReceivedEvent evt) => _activity.Note($"PM {evt.SentFrom} to {evt.SentTo}: {evt.Message}");

    public void HandleFrequencyMessage(FrequencyMessageReceivedEvent evt) => _activity.Note($"Frequency {evt.SentFrom}: {evt.Message}");

    public void HandleBroadcastMessage(BroadcastMessageReceivedEvent evt) => _activity.Note($"Broadcast {evt.SentFrom}: {evt.Message}");

    public void HandleSupervisorMessage(SupervisorMessageReceivedEvent evt) => _activity.Note($"Supervisor {evt.SentFrom}: {evt.Message}");

    public void HandleServerMessage(ServerMessageReceivedEvent evt) => _activity.Note($"Server {evt.SentFrom}: {evt.Message}");

    public void HandleAtcMessage(AtcMessageReceivedEvent evt) => _activity.Note($"ATC {evt.SentFrom}: {evt.Message}");

    public void HandleATISInfoMessage(ATISInfoMessageReceivedEvent evt) => _activity.Note($"{evt.Callsign} ATIS {evt.Icao} information {evt.AtisLetter}");
}
