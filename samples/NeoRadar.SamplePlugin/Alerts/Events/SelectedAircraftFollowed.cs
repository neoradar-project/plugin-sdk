namespace NeoRadar.SamplePlugin.Alerts.Events;

/// <summary>The alerts demo is following the selection, and the selected aircraft is here.</summary>
internal sealed record SelectedAircraftFollowed(string Callsign, double Latitude, double Longitude);
