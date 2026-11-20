using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.TestCommands;

internal sealed class AircraftCountCommand : ITestCommand
{
    public string Name => "sampleCount";

    public CommandDefinition Definition => new(
        Name: Name,
        Description: "Reports the live aircraft count from the host.",
        Parameters: Array.Empty<CommandParameter>());

    public CommandResult Execute(PluginContext context, IReadOnlyList<string> args)
    {
        _ = args;
        try
        {
            int count = context.Core.Aircraft.GetAll().Count;
            return new CommandResult(true, $"{count} aircraft connected");
        }
        catch (Exception ex)
        {
            return new CommandResult(false, $"Failed: {ex.Message}");
        }
    }
}
