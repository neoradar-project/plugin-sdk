using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NeoRadar.PluginSdk.Controller;
using NeoRadar.PluginSdk.Fsd;

namespace NeoRadar.SamplePlugin.WhoIsOnline;

/// <summary>Describes the user's own connection and the controllers online, one line each.</summary>
internal static class StationSummaryFormatter
{
    public static List<string> Describe(ConnectionInfo? connection, IReadOnlyList<Controller> controllers)
    {
        List<string> lines = [Connection(connection), $"Controllers online: {controllers.Count}"];
        foreach (Controller controller in controllers.OrderBy(c => c.Callsign, System.StringComparer.Ordinal))
            lines.Add("  " + Station(controller));
        return lines;
    }

    public static string Connection(ConnectionInfo? connection)
        => connection is null || !connection.IsConnected
            ? "FSD: not connected"
            : $"FSD: {connection.Callsign} as {connection.Facility} ({connection.Rating}), {connection.ServerType} server, ATC {connection.IsATC}, {Frequencies(connection.Frequencies)}";

    public static string Station(Controller controller)
    {
        string atis = controller.AtisLetter.Length == 0 ? string.Empty : $", ATIS {controller.AtisLetter} ({controller.AtisLines.Count} lines)";
        return $"{controller.Callsign} {controller.Facility} {controller.Rating} {Frequencies(controller.Frequencies)}, ATC {controller.IsATC}{atis}";
    }

    /// <summary>Frequencies arrive in kHz.</summary>
    public static string Frequencies(IReadOnlyList<int> kilohertz)
        => kilohertz.Count == 0
            ? "no frequency"
            : string.Join(" ", kilohertz.Select(k => (k / 1000.0).ToString("0.000", CultureInfo.InvariantCulture)));
}
