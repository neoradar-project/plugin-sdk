using System.Collections.Generic;
using System.Globalization;
using NeoRadar.PluginSdk.Airport;
using NeoRadar.PluginSdk.Navdata;

namespace NeoRadar.SamplePlugin.RunwayBoard;

/// <summary>Describes airports, their runways and navaids, one line each.</summary>
internal static class RunwayBoardFormatter
{
    public static void DescribeAirport(List<string> lines, AirportConfig config, AirportInfo? info, IReadOnlyList<RunwayInfo> runways)
    {
        string name = info is null ? "not in the navdata" : $"{info.Name}, elevation {(info.ElevationFeet.Length == 0 ? "unknown" : info.ElevationFeet + " ft")}";
        lines.Add($"{config.Icao} ({name}) {config.Status}, centrelines {(config.ShowCenterlines ? "shown" : "hidden")}");
        lines.Add($"  departures {List(config.DepRunways)}, arrivals {List(config.ArrRunways)}");
        foreach (RunwayInfo runway in runways)
        {
            lines.Add($"  {runway.LowEndIdent}/{runway.HighEndIdent} {Feet(runway.LengthFeet)} {runway.Surface}, " +
                $"thresholds {Feet(runway.LowEndElevationFeet)} and {Feet(runway.HighEndElevationFeet)}");
        }
    }

    public static string Navaid(NavaidInfo navaid)
        => $"{navaid.Ident} {navaid.Type} {navaid.Name} {Frequency(navaid.FrequencyKhz)} at {Degrees(navaid.Latitude)} {Degrees(navaid.Longitude)}, elevation {Feet(navaid.ElevationFeet)}";

    /// <summary>The navdata gives NaN for an elevation it does not know.</summary>
    public static string Feet(double feet)
        => double.IsNaN(feet) ? "unknown" : feet.ToString("#,0", CultureInfo.InvariantCulture) + " ft";

    /// <summary>A VHF navaid reads in MHz, an NDB in kHz.</summary>
    public static string Frequency(double kilohertz)
        => kilohertz >= 30_000
            ? (kilohertz / 1000.0).ToString("0.000", CultureInfo.InvariantCulture) + " MHz"
            : kilohertz.ToString("0.#", CultureInfo.InvariantCulture) + " kHz";

    private static string Degrees(double value) => value.ToString("0.000", CultureInfo.InvariantCulture);

    private static string List(IReadOnlyList<string> runways) => runways.Count == 0 ? "none" : string.Join(" ", runways);
}
