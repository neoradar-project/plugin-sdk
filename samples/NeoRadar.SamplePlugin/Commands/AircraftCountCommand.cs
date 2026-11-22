using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Chat;

namespace NeoRadar.SamplePlugin.Commands;

internal sealed class AircraftCountCommand : ISampleCommand
{
    private readonly IAircraftApi _aircraft;

    public AircraftCountCommand(IAircraftApi aircraft) => _aircraft = aircraft;

    public string Name => "sampleCount";

    public CommandDefinition Definition => new(
        Description: "Reports the live aircraft count from the host.",
        Parameters: Array.Empty<CommandParameter>());

    public CommandResult Execute(IReadOnlyList<string> args)
    {
        _ = args;
        int count = _aircraft.GetAll().Count;
        return new CommandResult(true, $"{count} aircraft connected");
    }
}
