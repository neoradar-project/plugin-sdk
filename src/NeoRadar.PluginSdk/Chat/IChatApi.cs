using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Chat;

/// <summary>
/// Provides the handler the host calls when a command registered with <see cref="IChatApi.RegisterCommand"/> runs.
/// </summary>
public interface ICommandProvider
{
    /// <summary>
    /// Runs a registered command with the arguments the controller typed.
    /// </summary>
    CommandResult Execute(string commandId, IReadOnlyList<string> args);
}

/// <summary>
/// Provides chat commands and local chat messages.
/// </summary>
public interface IChatApi
{
    /// <summary>
    /// Registers a chat command.
    /// </summary>
    /// <remarks>Reserved names are hello, wx, clear, dev, rings, vis, wallop, chat and center.</remarks>
    /// <returns>The command id, or an empty string if the name is invalid, reserved or already taken.</returns>
    string RegisterCommand(string name, CommandDefinition definition, ICommandProvider provider);

    /// <summary>
    /// Removes a chat command this plugin registered.
    /// </summary>
    /// <returns>False if the command id is unknown.</returns>
    bool UnregisterCommand(string commandId);

    /// <summary>
    /// Posts a message to this client's chat without sending it to the network.
    /// </summary>
    /// <remarks>Rate limited to roughly one call per second per plugin.</remarks>
    /// <returns>False if the call was throttled, so the plugin can retry or queue the message.</returns>
    bool SendClientMessage(ClientTextMessageEvent message);
}
