using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Chat;

public sealed record FrequencyMessageReceivedEvent(
    string SentFrom,
    string Message,
    IReadOnlyList<int> Frequencies,
    bool FromMe);

/// <summary>A flightplan message seen on the network.</summary>
/// <param name="RequestType">What the message asks for.</param>
/// <param name="Callsign">Callsign of the aircraft the request is about, not of the sender.</param>
/// <param name="Request">
/// The request's argument: the text that followed <c>SET</c>, as sent, for
/// <see cref="Chat.RequestType.SetBeaconCode"/>; empty for every other request.
/// </param>
public sealed record FlightplanMessageReceivedEvent(
    RequestType RequestType,
    string Callsign,
    string Request);

public sealed record ATISInfoMessageReceivedEvent(string Callsign, string Icao, string AtisLetter);

public sealed record PrivateMessageReceivedEvent(
    string SentFrom,
    string SentTo,
    string Message,
    bool FromMe);

public sealed record BroadcastMessageReceivedEvent(string SentFrom, string Message, bool FromMe);

public sealed record SupervisorMessageReceivedEvent(string SentFrom, string Message, bool FromMe);

public sealed record ServerMessageReceivedEvent(string SentFrom, string Message);

public sealed record AtcMessageReceivedEvent(string SentFrom, string Message);
