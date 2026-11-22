using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;

namespace NeoRadar.SamplePlugin.Commands;

// The host allows a plugin a burst of five client messages, then one a second, so each probe sends one.
internal sealed class ChatSystemProbeCommand : ISampleCommand
{
    private readonly IChatApi _chat;

    public ChatSystemProbeCommand(IChatApi chat) => _chat = chat;

    public string Name => "sampleChatProbe";

    public CommandDefinition Definition => new(
        Description: "Sends one orange message to SYSTEM.",
        Parameters: Array.Empty<CommandParameter>());

    public CommandResult Execute(IReadOnlyList<string> args)
    {
        _ = args;
        bool sent = _chat.SendClientMessage(
            new ClientTextMessageEvent("SamplePlugin", "Orange on SYSTEM", Colour: (255, 140, 0)));
        return new CommandResult(sent, sent ? "Sent the SYSTEM probe" : "Rate limited, try again in a second");
    }
}
