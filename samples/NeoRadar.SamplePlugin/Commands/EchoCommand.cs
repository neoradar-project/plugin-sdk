using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;

namespace NeoRadar.SamplePlugin.Commands;

internal sealed class EchoCommand : ISampleCommand
{
    public string Name => "sampleEcho";

    public CommandDefinition Definition => new(
        Description: "Echoes the rest of the line back.",
        Parameters: new[]
        {
            new CommandParameter(Name: "text", Type: ParameterType.String, Required: true),
        },
        LastParameterHasSpaces: true);

    public CommandResult Execute(IReadOnlyList<string> args)
    {
        if (args.Count == 0) return new CommandResult(false, "Usage: .sampleEcho <text>");
        return new CommandResult(true, string.Join(' ', args));
    }
}
