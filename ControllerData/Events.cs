namespace NeoRadar.PluginSdk.ControllerData;

public sealed record ControllerDataUpdatedEvent(string Callsign);

public sealed record AircraftBeaconCodeChangedEvent(string Callsign, string OldCode, string NewCode);

public sealed record AircraftHandoffCancelledEvent(string Callsign, string OldOwner, string NewOwner);

public sealed record AircraftOwnedByChangedEvent(string Callsign, string OldOwner, string NewOwner);

public sealed record AircraftHandoffRejectedEvent(string Callsign, string From);

public sealed record AircraftTerminatedTrackingEvent(string Callsign);

public sealed record AircraftInitiatedTrackingEvent(string Callsign);

public sealed record AircraftTemporaryAltitudeChangedEvent(string Callsign, int OldAltitude, int NewAltitude);

public sealed record AircraftCDMStatusChangedEvent(string Callsign, bool ClearanceIssued, GroundStatus GroundStatus);

public sealed record AircraftScratchpadUpdatedEvent(string Callsign, string Scratchpad);

public sealed record AircraftHeadingChangedEvent(string Callsign, int OldHeading, int NewHeading);

public sealed record AircraftAssignedSpeedChangedEvent(string Callsign, int OldSpeed, int NewSpeed);
