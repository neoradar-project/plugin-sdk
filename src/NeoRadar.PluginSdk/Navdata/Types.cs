namespace NeoRadar.PluginSdk.Navdata;

/// <summary>
/// Represents one physical runway with both of its ends.
/// </summary>
/// <remarks>Coordinates are WGS84 decimal degrees, headings are degrees true and distances are feet.</remarks>
/// <param name="AirportRef">The navdata reference of the runway's airport.</param>
/// <param name="AirportIdent">The runway airport's ICAO code, or its navdata identifier when it has none.</param>
/// <param name="LengthFeet">The runway length in feet.</param>
/// <param name="WidthFeet">The runway width in feet.</param>
/// <param name="Surface">The surface type as the source names it.</param>
/// <param name="Lighted">The source's lighting flag, "1" when the runway is lit.</param>
/// <param name="Closed">The source's closed flag, "1" when the runway is closed.</param>
/// <param name="LowEndIdent">The low end's designator.</param>
/// <param name="LowEndLatitude">The low end's latitude.</param>
/// <param name="LowEndLongitude">The low end's longitude.</param>
/// <param name="LowEndElevationFeet">The low end's elevation in feet, or NaN when the source has none.</param>
/// <param name="LowEndHeadingDegrees">The low end's heading in degrees true.</param>
/// <param name="LowEndDisplacedThresholdFeet">The low end's displaced threshold in feet from the end.</param>
/// <param name="HighEndIdent">The high end's designator.</param>
/// <param name="HighEndLatitude">The high end's latitude.</param>
/// <param name="HighEndLongitude">The high end's longitude.</param>
/// <param name="HighEndElevationFeet">The high end's elevation in feet, or NaN when the source has none.</param>
/// <param name="HighEndHeadingDegrees">The high end's heading in degrees true.</param>
/// <param name="HighEndDisplacedThresholdFeet">The high end's displaced threshold in feet from the end.</param>
public sealed record RunwayInfo(
    string AirportRef,
    string AirportIdent,
    double LengthFeet,
    double WidthFeet,
    string Surface,
    string Lighted,
    string Closed,
    string LowEndIdent,
    double LowEndLatitude,
    double LowEndLongitude,
    double LowEndElevationFeet,
    double LowEndHeadingDegrees,
    double LowEndDisplacedThresholdFeet,
    string HighEndIdent,
    double HighEndLatitude,
    double HighEndLongitude,
    double HighEndElevationFeet,
    double HighEndHeadingDegrees,
    double HighEndDisplacedThresholdFeet);

/// <summary>
/// Represents one navaid.
/// </summary>
/// <remarks>Coordinates are WGS84 decimal degrees. A navaid with no DME has a zero DME frequency and position and a NaN DME elevation.</remarks>
/// <param name="Type">The navaid type as the source names it.</param>
/// <param name="Ident">The navaid's identifier.</param>
/// <param name="Name">The navaid's name.</param>
/// <param name="FrequencyKhz">The frequency in kHz.</param>
/// <param name="Latitude">The latitude.</param>
/// <param name="Longitude">The longitude.</param>
/// <param name="ElevationFeet">The elevation in feet, or NaN when the source has none.</param>
/// <param name="IsoCountry">The ISO country code.</param>
/// <param name="DmeFrequencyKhz">The DME frequency in kHz.</param>
/// <param name="DmeLatitude">The DME's latitude.</param>
/// <param name="DmeLongitude">The DME's longitude.</param>
/// <param name="DmeElevationFeet">The DME's elevation in feet, or NaN when the source has none.</param>
/// <param name="SlavedVariationDegrees">The variation the navaid is aligned to, in degrees.</param>
/// <param name="MagneticVariationDegrees">The magnetic variation at the navaid, in degrees.</param>
/// <param name="UsageType">The usage type as the source names it.</param>
/// <param name="PowerWatts">The transmitter power as the source gives it.</param>
public sealed record NavaidInfo(
    string Type,
    string Ident,
    string Name,
    double FrequencyKhz,
    double Latitude,
    double Longitude,
    double ElevationFeet,
    string IsoCountry,
    double DmeFrequencyKhz,
    double DmeLatitude,
    double DmeLongitude,
    double DmeElevationFeet,
    double SlavedVariationDegrees,
    double MagneticVariationDegrees,
    string UsageType,
    string PowerWatts);

/// <summary>
/// Represents one airport.
/// </summary>
/// <remarks>The airport API names an airport by <c>IcaoCode</c>, or by <c>Ident</c> when it has none.</remarks>
/// <param name="Ident">The airport's navdata identifier.</param>
/// <param name="Type">The airport type as the source names it.</param>
/// <param name="Name">The airport's name.</param>
/// <param name="Latitude">The latitude in WGS84 decimal degrees.</param>
/// <param name="Longitude">The longitude in WGS84 decimal degrees.</param>
/// <param name="ElevationFeet">The elevation in feet as text, or empty when the source has none.</param>
/// <param name="Continent">The continent code.</param>
/// <param name="IsoCountry">The ISO country code.</param>
/// <param name="IsoRegion">The ISO region code.</param>
/// <param name="Municipality">The municipality the airport serves.</param>
/// <param name="ScheduledService">The source's scheduled-service flag.</param>
/// <param name="GpsCode">The GPS code.</param>
/// <param name="IcaoCode">The ICAO code.</param>
/// <param name="IataCode">The IATA code.</param>
/// <param name="LocalCode">The local code.</param>
/// <param name="HomeLink">The airport's website.</param>
/// <param name="WikipediaLink">The airport's Wikipedia page.</param>
/// <param name="Keywords">The source's search keywords.</param>
public sealed record AirportInfo(
    string Ident,
    string Type,
    string Name,
    double Latitude,
    double Longitude,
    string ElevationFeet,
    string Continent,
    string IsoCountry,
    string IsoRegion,
    string Municipality,
    string ScheduledService,
    string GpsCode,
    string IcaoCode,
    string IataCode,
    string LocalCode,
    string HomeLink,
    string WikipediaLink,
    string Keywords);
