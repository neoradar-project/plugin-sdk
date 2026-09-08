using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Chat;

public enum ParameterType { String, Number, Boolean }

public sealed record CommandParameter(
    string Name,
    ParameterType Type,
    bool TypeExplicit = false,
    bool Required = false,
    int Length = 0,
    int MinLength = 0,
    int MaxLength = 0);

public sealed record CommandDefinition(
    string Name,
    string Description,
    IReadOnlyList<CommandParameter> Parameters,
    bool LastParameterHasSpaces = false);

public enum RequestType
{
    Unknown = 0,
    Clearance = 1,
    Pushback = 2,
    TaxiOut = 3,
    TaxiIn = 4,
    Startup = 5,
}

public sealed record ValidationResult(bool IsValid, string? Message = null);

public sealed record CommandResult(bool Success, string? Message = null);

public sealed record ClientTextMessageEvent(
    string SentFrom,
    string Message,
    (int R, int G, int B)? Colour = null,
    bool IsUrgent = false,
    bool UseDedicatedChannel = false);
