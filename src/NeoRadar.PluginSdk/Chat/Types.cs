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

/// <summary>
/// What a flightplan message, a controller client's text message to the network's <c>FP</c>
/// recipient, asks for about one aircraft.
/// </summary>
public enum RequestType
{
    /// <summary>The client could not tell what the message asks for.</summary>
    Unknown = 0,

    /// <summary>A controller asks for a beacon code to be allocated to the aircraft (<c>GET</c>).</summary>
    RequestBeaconCode = 1,

    /// <summary>A controller sets the aircraft's beacon code (<c>SET</c>); the code is the request.</summary>
    SetBeaconCode = 2,

    /// <summary>A controller releases the aircraft's flightplan (<c>release</c>).</summary>
    ReleaseFlightplan = 3,
}

public sealed record ValidationResult(bool IsValid, string? Message = null);

public sealed record CommandResult(bool Success, string? Message = null);

public sealed record ClientTextMessageEvent(
    string SentFrom,
    string Message,
    (int R, int G, int B)? Colour = null,
    bool IsUrgent = false,
    bool UseDedicatedChannel = false);
