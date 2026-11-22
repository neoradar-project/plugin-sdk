using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.PluginSdk.Logger;
using NeoRadar.SamplePlugin.Commands.Events;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;

namespace NeoRadar.SamplePlugin.Commands;

/// <summary>Chat commands whose parameters the host checks before the plugin sees them.</summary>
internal sealed class CommandsFeature : IFeature, ICommandProvider
{
    private readonly IChatApi _chat;
    private readonly ILoggerApi _logger;
    private readonly IPluginEventBus _bus;
    private readonly ISampleCommand[] _commands;
    private readonly Dictionary<string, ISampleCommand> _byId = new(StringComparer.Ordinal);

    public CommandsFeature(IChatApi chat, IAircraftApi aircraft, ILoggerApi logger, IPluginEventBus bus)
    {
        _chat = chat;
        _logger = logger;
        _bus = bus;
        _commands =
        [
            new PingCommand(),
            new EchoCommand(),
            new AircraftCountCommand(aircraft),
            new ChatSystemProbeCommand(chat),
            new ChatChannelProbeCommand(chat),
            new SwitchCommand(),
            new CallsignPatternCommand(),
        ];
    }

    public void Initialize()
    {
        foreach (ISampleCommand command in _commands)
        {
            string id = _chat.RegisterCommand(command.Name, command.Definition, this);
            if (id.Length == 0)
            {
                _logger.Warning($"Commands: .{command.Name} was refused (reserved, taken, or a parameter rule is invalid)");
                continue;
            }
            _byId[id] = command;
        }

        List<string> lines = [$"{_byId.Count} of {_commands.Length} commands registered:"];
        foreach (ISampleCommand command in _commands)
            lines.Add($"  .{command.Name}: {command.Definition.Description}");
        _bus.Publish(new CommandsListed(lines));
    }

    public void Shutdown()
    {
        foreach (string id in _byId.Keys) _chat.UnregisterCommand(id);
        _byId.Clear();
    }

    // Client thread; a throw here is a fault and 250 ms is a slow strike, so it catches and stays short.
    public CommandResult Execute(string commandId, IReadOnlyList<string> args)
    {
        if (!_byId.TryGetValue(commandId, out ISampleCommand? command))
            return new CommandResult(false, "Unknown command");

        try
        {
            return command.Execute(args);
        }
        catch (Exception ex)
        {
            _logger.Error($"Commands: .{command.Name} failed: {ex}");
            return new CommandResult(false, $"Error: {ex.Message}");
        }
    }
}
