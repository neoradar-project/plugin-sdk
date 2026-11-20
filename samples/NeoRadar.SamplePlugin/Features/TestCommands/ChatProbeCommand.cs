using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.TestCommands;

// The host allows a plugin one client message a second, so each probe sends one.
internal sealed class ChatProbeCommand : ITestCommand
{
    public string Name => "sampleChatProbe";

    public CommandDefinition Definition => new(
        Name: Name,
        Description: "Sends one orange message to SYSTEM.",
        Parameters: Array.Empty<CommandParameter>());

    public CommandResult Execute(PluginContext context, IReadOnlyList<string> args)
    {
        _ = args;
        bool sent = context.Chat.SendClientMessage(
            new ClientTextMessageEvent("SamplePlugin", "Orange on SYSTEM", Colour: (255, 140, 0)));
        return new CommandResult(sent, sent ? "Sent the SYSTEM probe" : "Rate limited, try again in a second");
    }
}

internal sealed class ChatChannelProbeCommand : ITestCommand
{
    public string Name => "sampleChatChannelProbe";

    public CommandDefinition Definition => new(
        Name: Name,
        Description: "Sends one green message to a channel of its own.",
        Parameters: Array.Empty<CommandParameter>());

    public CommandResult Execute(PluginContext context, IReadOnlyList<string> args)
    {
        _ = args;
        bool sent = context.Chat.SendClientMessage(
            new ClientTextMessageEvent("SamplePlugin", "Green in its own channel", Colour: (60, 200, 90), UseDedicatedChannel: true));
        return new CommandResult(sent, sent ? "Sent the channel probe" : "Rate limited, try again in a second");
    }
}
