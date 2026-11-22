using System.Collections.Generic;
using System.Globalization;
using NeoRadar.PluginSdk.Flightplan;

namespace NeoRadar.SamplePlugin.RouteInspector;

/// <summary>Describes a filed flight plan leg by leg, as text a controller reads top to bottom.</summary>
internal static class RouteReportFormatter
{
    public static IReadOnlyList<string> Describe(string callsign, Flightplan? plan, double? fromOrigin, double? toDestination)
    {
        if (plan is null) return [$"{callsign}: no flight plan filed"];

        List<string> lines =
        [
            $"{plan.Callsign} {plan.Origin}-{plan.Destination} {plan.AcType} ({plan.FlightRule}), voice {plan.VoiceType}",
            $"Filed route: {Or(plan.Route.RawRoute, "none")}",
            $"From origin {Nm(fromOrigin)}, to destination {Nm(toDestination)}",
            plan.Route.HasDirectApplied
                ? $"Direct to {Point(plan.Route.CurrentDirectWaypoint)}"
                : "No direct-to applied",
        ];

        lines.Add(plan.Route.Segments.Count == 0
            ? "Legs: none parsed (the route is parsed by the NeoRadar server)"
            : $"Legs ({plan.Route.Segments.Count}):");
        foreach (ParsedRouteSegment leg in plan.Route.Segments)
            lines.Add($"  {Point(leg.From)} -{Or(leg.Airway, "DCT")}-> {Point(leg.To)}");

        lines.Add(plan.Route.Errors.Count == 0 ? "Parsing errors: none" : $"Parsing errors ({plan.Route.Errors.Count}):");
        foreach (ParsingError error in plan.Route.Errors)
            lines.Add($"  {error.Type} at token {error.TokenIndex} '{error.Token}': {error.Message}");

        lines.Add($"Remarks: {Or(plan.Remarks, "none")}");
        return lines;
    }

    public static string Point(Waypoint point)
    {
        if (point.Identifier.Length == 0) return "(unset)";
        return point.FrequencyHz == 0
            ? $"{point.Identifier} [{point.Type}]"
            : $"{point.Identifier} [{point.Type} {Frequency(point.FrequencyHz)}]";
    }

    /// <summary>A VHF navaid reads in MHz, an NDB in kHz.</summary>
    public static string Frequency(int hertz)
        => hertz >= 30_000_000
            ? (hertz / 1_000_000.0).ToString("0.000", CultureInfo.InvariantCulture) + " MHz"
            : (hertz / 1_000.0).ToString("0.#", CultureInfo.InvariantCulture) + " kHz";

    private static string Nm(double? distance)
        => distance is double nm ? nm.ToString("0.0", CultureInfo.InvariantCulture) + " NM" : "unknown";

    private static string Or(string value, string fallback) => value.Length == 0 ? fallback : value;
}
