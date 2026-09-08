using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Chat;

/// <summary>
/// Provider implementation called by the host when a registered command fires.
/// Plugins implement this and pass an instance to <see cref="IChatApi.RegisterCommand"/>.
/// </summary>
public interface ICommandProvider
{
    CommandResult Execute(string commandId, IReadOnlyList<string> args);
}

public interface IChatApi
{
    /// <summary>
    /// Register a command. Returns the command ID on success or an empty string
    /// if the name is invalid, reserved, or already taken. Reserved names:
    /// "hello", "wx", "clear", "dev", "rings", "vis", "wallop", "chat", "center".
    /// </summary>
    string RegisterCommand(string name, CommandDefinition definition, ICommandProvider provider);

    bool UnregisterCommand(string commandId);

    /// <summary>
    /// Post a message into the client's chat. Returns false when the call was throttled, so a
    /// plugin can retry or queue it: sending is rate limited to roughly one call per second per
    /// plugin. The message is local to this client and is never sent to the network.
    /// </summary>
    bool SendClientMessage(ClientTextMessageEvent message);
}
