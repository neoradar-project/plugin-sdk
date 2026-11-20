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
/// Represents one parameter of a chat command, which the host validates before the command runs.
/// </summary>
/// <param name="Name">The parameter name shown in the command's signature.</param>
/// <param name="Type">The type the value must parse as.</param>
/// <param name="TypeExplicit">True to refuse a value containing digits or special characters.</param>
/// <param name="Required">True if the command refuses to run without a value.</param>
/// <param name="Length">The exact length required, or 0 for any.</param>
/// <param name="MinLength">The minimum length, or 0 for none.</param>
/// <param name="MaxLength">The maximum length, or 0 for none.</param>
public sealed record CommandParameter(
    string Name,
    ParameterType Type,
    bool TypeExplicit = false,
    bool Required = false,
    int Length = 0,
    int MinLength = 0,
    int MaxLength = 0);

/// <summary>
/// Represents a chat command a plugin registers.
/// </summary>
/// <param name="Name">The command name the controller types.</param>
/// <param name="Description">The description shown in command help.</param>
/// <param name="Parameters">The parameters in the order they are typed.</param>
/// <param name="LastParameterHasSpaces">True if the last parameter takes the rest of the line, spaces included.</param>
public sealed record CommandDefinition(
    string Name,
    string Description,
    IReadOnlyList<CommandParameter> Parameters,
    bool LastParameterHasSpaces = false);

/// <summary>
/// Represents the outcome of validating command input.
/// </summary>
/// <param name="IsValid">True if the input is valid.</param>
/// <param name="Message">The reason the input is invalid, or null.</param>
public sealed record ValidationResult(bool IsValid, string? Message = null);

/// <summary>
/// Represents the outcome of running a command.
/// </summary>
/// <param name="Success">True if the command succeeded.</param>
/// <param name="Message">A message shown to the controller, or null for none.</param>
public sealed record CommandResult(bool Success, string? Message = null);

/// <summary>
/// Represents a message a plugin posts to this client's chat.
/// </summary>
/// <param name="SentFrom">The sender name shown with the message.</param>
/// <param name="Message">The message text.</param>
/// <param name="Colour">The text colour, or null for the default.</param>
/// <param name="IsUrgent">True to mark the message as urgent.</param>
/// <param name="UseDedicatedChannel">True to post in a channel named after the plugin.</param>
public sealed record ClientTextMessageEvent(
    string SentFrom,
    string Message,
    (int R, int G, int B)? Colour = null,
    bool IsUrgent = false,
    bool UseDedicatedChannel = false);
