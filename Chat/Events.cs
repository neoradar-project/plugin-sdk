using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Chat;

public sealed record FrequencyMessageReceivedEvent(
    string SentFrom,
    string Message,
    IReadOnlyList<int> Frequencies,
    bool FromMe);

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
