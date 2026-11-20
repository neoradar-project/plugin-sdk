using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.TestCommands;

internal interface ITestCommand
{
    string Name { get; }
    CommandDefinition Definition { get; }
    CommandResult Execute(PluginContext context, IReadOnlyList<string> args);
}
