using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.TestCommands;

internal sealed class PingCommand : ITestCommand
{
    public string Name => "samplePing";

    public CommandDefinition Definition => new(
        Name: Name,
        Description: "Replies with pong + roundtrip timestamp.",
        Parameters: Array.Empty<CommandParameter>());

    public CommandResult Execute(PluginContext context, IReadOnlyList<string> args)
    {
        _ = args;
        return new CommandResult(true, $"pong @ {DateTimeOffset.UtcNow:HH:mm:ss}Z");
    }
}
