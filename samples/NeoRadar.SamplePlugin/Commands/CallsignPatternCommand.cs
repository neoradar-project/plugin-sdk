using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;

namespace NeoRadar.SamplePlugin.Commands;

/// <summary>Takes a callsign the host checks against a pattern, anchored at both ends, before the plugin sees it.</summary>
internal sealed class CallsignPatternCommand : ISampleCommand
{
    public const string CallsignPattern = "[A-Z]{3}[0-9]+[A-Z]?";

    public string Name => "sampleCallsign";

    public CommandDefinition Definition => new(
        Description: "Echoes a callsign of three letters, digits and an optional letter.",
        Parameters: [new CommandParameter("callsign", ParameterType.String, Required: true, Pattern: CallsignPattern)]);

    public CommandResult Execute(IReadOnlyList<string> args) => new(true, $"{args[0]} matches {CallsignPattern}");
}
