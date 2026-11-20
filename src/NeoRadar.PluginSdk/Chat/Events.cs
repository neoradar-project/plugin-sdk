using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Chat;

/// <summary>
/// Raised when a text message is sent on one or more radio frequencies.
/// </summary>
/// <param name="SentFrom">The sender's callsign.</param>
/// <param name="Message">The message text.</param>
/// <param name="Frequencies">The frequencies the message was sent on, in kHz.</param>
/// <param name="FromMe">True if the user sent the message.</param>
public sealed record FrequencyMessageReceivedEvent(
    string SentFrom,
    string Message,
    IReadOnlyList<int> Frequencies,
    bool FromMe);

/// <summary>
/// Raised when a station's ATIS letter is received.
/// </summary>
/// <param name="Callsign">The callsign of the ATIS station.</param>
/// <param name="Icao">The ICAO code of the airport.</param>
/// <param name="AtisLetter">The current ATIS letter.</param>
public sealed record ATISInfoMessageReceivedEvent(string Callsign, string Icao, string AtisLetter);

/// <summary>
/// Raised when a private message is sent or received.
/// </summary>
/// <param name="SentFrom">The sender's callsign.</param>
/// <param name="SentTo">The recipient's callsign.</param>
/// <param name="Message">The message text.</param>
/// <param name="FromMe">True if the user sent the message.</param>
public sealed record PrivateMessageReceivedEvent(
    string SentFrom,
    string SentTo,
    string Message,
    bool FromMe);

/// <summary>
/// Raised when a broadcast message is sent or received.
/// </summary>
/// <param name="SentFrom">The sender's callsign.</param>
/// <param name="Message">The message text.</param>
/// <param name="FromMe">True if the user sent the message.</param>
public sealed record BroadcastMessageReceivedEvent(string SentFrom, string Message, bool FromMe);

/// <summary>
/// Raised when a message on the supervisor channel is sent or received.
/// </summary>
/// <param name="SentFrom">The sender's callsign.</param>
/// <param name="Message">The message text.</param>
/// <param name="FromMe">True if the user sent the message.</param>
public sealed record SupervisorMessageReceivedEvent(string SentFrom, string Message, bool FromMe);

/// <summary>
/// Raised when the network server sends a message.
/// </summary>
/// <param name="SentFrom">The server's name.</param>
/// <param name="Message">The message text.</param>
public sealed record ServerMessageReceivedEvent(string SentFrom, string Message);

/// <summary>
/// Raised when a message on the ATC channel is received.
/// </summary>
/// <param name="SentFrom">The sender's callsign.</param>
/// <param name="Message">The message text.</param>
public sealed record AtcMessageReceivedEvent(string SentFrom, string Message);
