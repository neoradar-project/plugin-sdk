using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;

namespace NeoRadar.SamplePlugin.Commands;

internal sealed class ChatChannelProbeCommand : ISampleCommand
{
    private readonly IChatApi _chat;

    public ChatChannelProbeCommand(IChatApi chat) => _chat = chat;

    public string Name => "sampleChatChannelProbe";

    public CommandDefinition Definition => new(
        Description: "Sends one green message to a channel of its own.",
        Parameters: Array.Empty<CommandParameter>());

    public CommandResult Execute(IReadOnlyList<string> args)
    {
        _ = args;
        bool sent = _chat.SendClientMessage(
            new ClientTextMessageEvent("SamplePlugin", "Green in its own channel", Colour: (60, 200, 90), Channel: "Sample"));
        return new CommandResult(sent, sent ? "Sent the channel probe" : "Rate limited, try again in a second");
    }
}
