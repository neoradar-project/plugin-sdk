namespace NeoRadar.PluginSdk.Navdata;

/// <summary>
/// One physical runway, both ends. Field names and units mirror the client's navdata rows:
/// lengths in feet, latitudes and longitudes in decimal degrees (WGS84), elevations in feet,
/// headings in degrees true, displaced thresholds in feet from the end.
/// <c>Lighted</c> and <c>Closed</c> are the raw source flags, "1" when set.
/// </summary>
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
/// One navaid. Frequencies in kHz, latitudes and longitudes in decimal degrees (WGS84),
/// elevations in feet, variations in degrees. The <c>Dme*</c> fields are zero when the navaid
/// carries no DME.
/// </summary>
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
/// One airport. Latitude and longitude in decimal degrees (WGS84). <c>ElevationFeet</c> is the
/// raw source column: feet, as text, and empty when the source has no elevation.
/// </summary>
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
