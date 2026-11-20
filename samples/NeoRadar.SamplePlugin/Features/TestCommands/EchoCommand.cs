using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.TestCommands;

internal sealed class EchoCommand : ITestCommand
{
    public string Name => "sampleEcho";

    public CommandDefinition Definition => new(
        Name: Name,
        Description: "Echoes the rest of the line back.",
        Parameters: new[]
        {
            new CommandParameter(Name: "text", Type: ParameterType.String, Required: true),
        },
        LastParameterHasSpaces: true);

    public CommandResult Execute(PluginContext context, IReadOnlyList<string> args)
    {
        if (args.Count == 0) return new CommandResult(false, "Usage: .sampleEcho <text>");
        return new CommandResult(true, string.Join(' ', args));
    }
}
