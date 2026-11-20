using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.TestCommands;

internal sealed class TestCommandsFeature : IFeature, ICommandProvider
{
    private readonly Dictionary<string, ITestCommand> _byId = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ITestCommand> _byName = new(StringComparer.OrdinalIgnoreCase);
    private PluginContext? _context;

    private static readonly ITestCommand[] Commands =
    {
        new PingCommand(),
        new EchoCommand(),
        new AircraftCountCommand(),
        new ChatProbeCommand(),
        new ChatChannelProbeCommand(),
    };

    public void Initialize(PluginContext context)
    {
        _context = context;

        for (int i = 0; i < Commands.Length; i++)
        {
            ITestCommand cmd = Commands[i];
            string id = context.Chat.RegisterCommand(cmd.Name, cmd.Definition, this);
            if (string.IsNullOrEmpty(id))
            {
                context.Logger.Warning($"TestCommands: failed to register .{cmd.Name} (reserved or duplicate)");
                continue;
            }
            _byId[id] = cmd;
            _byName[cmd.Name] = cmd;
        }
    }

    public void Shutdown()
    {
        if (_context is null) return;
        foreach (string id in _byId.Keys)
            _context.Chat.UnregisterCommand(id);
        _byId.Clear();
        _byName.Clear();
    }

    public CommandResult Execute(string commandId, IReadOnlyList<string> args)
    {
        if (_context is null) return new CommandResult(false, "Plugin not initialised");
        if (!_byId.TryGetValue(commandId, out ITestCommand? cmd))
            return new CommandResult(false, "Unknown command");
        try { return cmd.Execute(_context, args); }
        catch (Exception ex) { return new CommandResult(false, $"Error: {ex.Message}"); }
    }
}
