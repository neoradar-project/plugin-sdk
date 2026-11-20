using System;
using Ac = NeoRadar.PluginSdk.Aircraft.Aircraft;

namespace NeoRadar.SamplePlugin.Common;

internal static class DemoFakery
{
    public const double EgllLat = 51.4775;
    public const double EgllLon = -0.4614;

    public static int EtaMinutesTo(Ac ac, double targetLat = EgllLat, double targetLon = EgllLon)
    {
        double dist = HaversineNm(ac.Position.Latitude, ac.Position.Longitude, targetLat, targetLon);
        if (dist < 2.0) return 0;
        int speed = Math.Max(ac.Position.GroundSpeed, 120);
        return (int)Math.Round(dist / speed * 60.0);
    }

    public static double DistanceNmTo(Ac ac, double targetLat = EgllLat, double targetLon = EgllLon)
        => HaversineNm(ac.Position.Latitude, ac.Position.Longitude, targetLat, targetLon);

    public static char GuessWake(string? callsign)
    {
        if (string.IsNullOrEmpty(callsign)) return 'M';
        char tail = char.ToUpperInvariant(callsign[^1]);
        return tail switch
        {
            'A' or 'B' or 'C' => 'H',
            'X' or 'Y' or 'Z' => 'L',
            _ => 'M',
        };
    }

    public static string GuessType(string? callsign)
    {
        if (string.IsNullOrEmpty(callsign)) return "A320";
        string p = callsign.Length >= 3
            ? callsign.Substring(0, 3).ToUpperInvariant()
            : callsign.ToUpperInvariant();
        return p switch
        {
            "BAW" => "A320",
            "RYR" => "B738",
            "EZY" => "A320",
            "DLH" => "A319",
            "KLM" => "B737",
            "AFR" => "A321",
            "VIR" => "A35K",
            "QFA" => "B789",
            "UAE" => "A388",
            _ => "A320",
        };
    }

    private static double HaversineNm(double lat1, double lon1, double lat2, double lon2)
    {
        const double r = 3440.065;
        double dLat = (lat2 - lat1) * Math.PI / 180.0;
        double dLon = (lon2 - lon1) * Math.PI / 180.0;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0)
            * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return r * 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
    }
}
