using NeoRadar.PluginSdk.Chat;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Inbound text-message events, on client threads.</summary>
internal interface IChatEventHandler
{
    void OnATISInfoMessageReceived(ATISInfoMessageReceivedEvent evt) { }

    void OnPrivateMessageReceived(PrivateMessageReceivedEvent evt) { }

    void OnFrequencyMessageReceived(FrequencyMessageReceivedEvent evt) { }

    void OnBroadcastMessageReceived(BroadcastMessageReceivedEvent evt) { }

    void OnSupervisorMessageReceived(SupervisorMessageReceivedEvent evt) { }

    void OnServerMessageReceived(ServerMessageReceivedEvent evt) { }

    void OnAtcMessageReceived(AtcMessageReceivedEvent evt) { }
}
