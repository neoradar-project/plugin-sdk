using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;

namespace NeoRadar.SamplePlugin.Commands;

internal sealed class PingCommand : ISampleCommand
{
    public string Name => "samplePing";

    public CommandDefinition Definition => new(
        Description: "Replies with pong + roundtrip timestamp.",
        Parameters: Array.Empty<CommandParameter>());

    public CommandResult Execute(IReadOnlyList<string> args)
    {
        _ = args;
        return new CommandResult(true, $"pong @ {DateTimeOffset.UtcNow:HH:mm:ss}Z");
    }
}
