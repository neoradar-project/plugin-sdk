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
/// Raised when a station's "ATIS info" text message, which carries its ATIS letter, is received.
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
/// Raised when the network server sends the user a message.
/// </summary>
/// <remarks>A line the user sends to the server raises no event.</remarks>
/// <param name="SentFrom">The server's name.</param>
/// <param name="Message">The message text.</param>
public sealed record ServerMessageReceivedEvent(string SentFrom, string Message);

/// <summary>
/// Raised when another station sends a message on the ATC channel.
/// </summary>
/// <param name="SentFrom">The sender's callsign.</param>
/// <param name="Message">The message text.</param>
public sealed record AtcMessageReceivedEvent(string SentFrom, string Message);
