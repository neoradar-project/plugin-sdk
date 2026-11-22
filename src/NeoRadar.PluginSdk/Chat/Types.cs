using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Chat;

/// <summary>
/// Specifies the type a command parameter must parse as.
/// </summary>
public enum ParameterType
{
    /// <summary>
    /// Any text.
    /// </summary>
    String,

    /// <summary>
    /// A number in invariant form.
    /// </summary>
    Number,

    /// <summary>
    /// One of true, false, 1, 0, y or n.
    /// </summary>
    Boolean
}

/// <summary>
/// Represents one parameter of a chat command, which the host validates as the controller types and before the command runs.
/// </summary>
/// <remarks>
/// Every rule that is set applies, and a value that breaks one marks the chat input invalid and never reaches the plugin.
/// The host anchors <paramref name="Pattern"/> at both ends, matches it with the invariant culture and refuses, without
/// matching it again, a value whose match takes longer than 100 ms. An empty or uncompilable pattern, or an allowed value
/// holding a space anywhere but the last parameter of a <see cref="CommandDefinition.LastParameterHasSpaces"/> command,
/// refuses the registration.
/// </remarks>
/// <param name="Name">The parameter name shown in the command's signature.</param>
/// <param name="Type">The type the value must parse as.</param>
/// <param name="TypeExplicit">True to refuse a value containing digits or special characters.</param>
/// <param name="Required">True if the command refuses to run without a value.</param>
/// <param name="Length">The exact length required, or 0 for any.</param>
/// <param name="MinLength">The minimum length, or 0 for none.</param>
/// <param name="MaxLength">The maximum length, or 0 for none.</param>
/// <param name="AllowedValues">The values accepted, compared ignoring case and passed on as declared, or null or empty for any.</param>
/// <param name="Pattern">A .NET regular expression the whole value must match, or null for none.</param>
public sealed record CommandParameter(
    string Name,
    ParameterType Type,
    bool TypeExplicit = false,
    bool Required = false,
    int Length = 0,
    int MinLength = 0,
    int MaxLength = 0,
    IReadOnlyList<string>? AllowedValues = null,
    string? Pattern = null);

/// <summary>
/// Represents a chat command a plugin registers under the name given to <see cref="IChatApi.RegisterCommand"/>.
/// </summary>
/// <param name="Description">The description shown in command help.</param>
/// <param name="Parameters">The parameters in the order they are typed.</param>
/// <param name="LastParameterHasSpaces">True if the last parameter takes the rest of the line, spaces included.</param>
public sealed record CommandDefinition(
    string Description,
    IReadOnlyList<CommandParameter> Parameters,
    bool LastParameterHasSpaces = false);

/// <summary>
/// Represents the outcome of running a command.
/// </summary>
/// <param name="Success">True if the command succeeded.</param>
/// <param name="Message">A message shown to the controller, or null for none.</param>
public sealed record CommandResult(bool Success, string? Message = null);

/// <summary>
/// Represents a message a plugin posts to this client's chat.
/// </summary>
/// <remarks>
/// A plugin channel shows in the chat panel as <c>[name]</c> with a tooltip naming the plugin. Names differing only in
/// case are one channel, and another plugin's channel of the same name is a separate one. A plugin may use up to 8
/// names until it unloads. <see cref="IChatApi.SendClientMessage"/> refuses a name that is empty, has leading or trailing
/// whitespace, holds a colon or a control, format or separator character, is longer than 32 characters, names a built-in
/// channel (System, Server, ATC, Supervisor, Broadcast, Frequency) or a frequency, or is a 9th new name.
/// </remarks>
/// <param name="SentFrom">The sender name shown with the message.</param>
/// <param name="Message">The message text.</param>
/// <param name="Colour">The text colour, or null for the default.</param>
/// <param name="IsUrgent">True to mark the message as urgent.</param>
/// <param name="Channel">The name of the plugin's own channel to post in, or null for the System channel.</param>
public sealed record ClientTextMessageEvent(
    string SentFrom,
    string Message,
    (int R, int G, int B)? Colour = null,
    bool IsUrgent = false,
    string? Channel = null);
