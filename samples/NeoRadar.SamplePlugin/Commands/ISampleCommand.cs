using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;

namespace NeoRadar.SamplePlugin.Commands;

/// <summary>One chat command: the name typed after the dot, what the host validates, and what it does.</summary>
internal interface ISampleCommand
{
    string Name { get; }

    CommandDefinition Definition { get; }

    CommandResult Execute(IReadOnlyList<string> args);
}
