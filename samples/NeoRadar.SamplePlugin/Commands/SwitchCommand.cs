using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;

namespace NeoRadar.SamplePlugin.Commands;

/// <summary>Takes on or off in any case; the host refuses anything else and passes the value as declared.</summary>
internal sealed class SwitchCommand : ISampleCommand
{
    private volatile string _state = "off";

    public string Name => "sampleSwitch";

    public CommandDefinition Definition => new(
        Description: "Turns the sample switch on or off.",
        Parameters: [new CommandParameter("state", ParameterType.String, Required: true, AllowedValues: ["on", "off"])]);

    public CommandResult Execute(IReadOnlyList<string> args)
    {
        _state = args[0];
        return new CommandResult(true, $"Sample switch is {_state} (received '{args[0]}')");
    }
}
