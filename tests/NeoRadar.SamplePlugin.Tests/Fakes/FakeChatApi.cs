using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

// Refuses the built-in channel names and, past AllowedMessages, every message, as the host does.
internal sealed class FakeChatApi : IChatApi
{
    private static readonly string[] s_reserved = ["System", "Server", "ATC", "Sup", "Supervisor", "Broadcast", "Frequency", "AFV"];

    public Dictionary<string, (string Name, CommandDefinition Definition, ICommandProvider Provider)> Commands { get; } = new();

    public List<ClientTextMessageEvent> Sent { get; } = new();

    public int AllowedMessages { get; set; } = int.MaxValue;

    public string RegisterCommand(string name, CommandDefinition definition, ICommandProvider provider)
    {
        string id = "plugin:Sample:CMD_" + name;
        Commands[id] = (name, definition, provider);
        return id;
    }

    public bool UnregisterCommand(string commandId) => Commands.Remove(commandId);

    public bool SendClientMessage(ClientTextMessageEvent message)
    {
        if (message.Channel is not null && Array.Exists(s_reserved, r => string.Equals(r, message.Channel, StringComparison.OrdinalIgnoreCase))) return false;
        if (AllowedMessages <= 0) return false;
        AllowedMessages--;
        Sent.Add(message);
        return true;
    }

    public CommandResult Run(string name, params string[] args)
    {
        foreach (KeyValuePair<string, (string Name, CommandDefinition Definition, ICommandProvider Provider)> entry in Commands)
            if (entry.Value.Name == name) return entry.Value.Provider.Execute(entry.Key, args);
        throw new InvalidOperationException($".{name} is not registered");
    }
}
