namespace NeoRadar.PluginSdk.ControllerData;

/// <summary>
/// Raised when any part of an aircraft's controller data changes.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
public sealed record ControllerDataUpdatedEvent(string Callsign);

/// <summary>
/// Raised when an aircraft's assigned beacon code changes.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="OldCode">The previous beacon code.</param>
/// <param name="NewCode">The new beacon code.</param>
public sealed record AircraftBeaconCodeChangedEvent(string Callsign, string OldCode, string NewCode);

/// <summary>
/// Raised when a pending handoff of an aircraft is withdrawn, whether the offering controller or the user withdraws it.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="OldOwner">The controller who owned the aircraft before the handoff was withdrawn.</param>
/// <param name="NewOwner">The controller who owns the aircraft once the handoff is withdrawn.</param>
public sealed record AircraftHandoffCancelledEvent(string Callsign, string OldOwner, string NewOwner);

/// <summary>
/// Raised when the controller tracking an aircraft changes.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="OldOwner">The previous tracking controller, or empty when none.</param>
/// <param name="NewOwner">The new tracking controller, or empty when none.</param>
public sealed record AircraftOwnedByChangedEvent(string Callsign, string OldOwner, string NewOwner);

/// <summary>
/// Raised when a handoff of an aircraft is rejected, whether the user rejects one offered to them or the receiver refuses the user's.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="From">The controller who rejected the handoff, which is the user for a handoff the user rejects.</param>
public sealed record AircraftHandoffRejectedEvent(string Callsign, string From);

/// <summary>
/// Raised when a handoff is offered, whether the user starts it or it is offered to the user.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="FromController">The offering controller, which is the user for a handoff the user starts.</param>
/// <param name="ToController">The receiving controller, which is the user for a handoff offered to the user.</param>
public sealed record AircraftHandoffInitiatedEvent(string Callsign, string FromController, string ToController);

/// <summary>
/// Raised when a tracked aircraft is dropped, by any controller or because its controller left the network.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
public sealed record AircraftTerminatedTrackingEvent(string Callsign);

/// <summary>
/// Raised when the local user starts tracking an aircraft.
/// </summary>
/// <remarks>Another controller starting to track one raises <see cref="AircraftOwnedByChangedEvent"/>.</remarks>
/// <param name="Callsign">The aircraft's callsign.</param>
public sealed record AircraftInitiatedTrackingEvent(string Callsign);

/// <summary>
/// Raised when an aircraft's temporary altitude changes.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="OldAltitude">The previous altitude in feet, or 0 for none, 1 for ILS, 2 for visual.</param>
/// <param name="NewAltitude">The new altitude in feet, or 0 for none, 1 for ILS, 2 for visual.</param>
public sealed record AircraftTemporaryAltitudeChangedEvent(string Callsign, int OldAltitude, int NewAltitude);

/// <summary>
/// Raised when an aircraft's clearance flag or ground status changes.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="ClearanceIssued">True if the clearance has been issued.</param>
/// <param name="GroundStatus">The aircraft's ground status.</param>
public sealed record AircraftCDMStatusChangedEvent(string Callsign, bool ClearanceIssued, GroundStatus GroundStatus);

/// <summary>
/// Raised when an aircraft's scratchpad text changes.
/// </summary>
/// <remarks>A command line that leaves the scratchpad text unchanged raises no event.</remarks>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="Scratchpad">The new scratchpad text.</param>
public sealed record AircraftScratchpadUpdatedEvent(string Callsign, string Scratchpad);

/// <summary>
/// Raised when an aircraft's assigned heading changes.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="OldHeading">The previous heading in degrees, or 0 for none.</param>
/// <param name="NewHeading">The new heading in degrees, or 0 for none.</param>
public sealed record AircraftHeadingChangedEvent(string Callsign, int OldHeading, int NewHeading);

/// <summary>
/// Raised when an aircraft's assigned speed changes.
/// </summary>
/// <param name="Callsign">The aircraft's callsign.</param>
/// <param name="OldSpeed">The previous speed in knots, or in hundredths of Mach for a Mach assignment.</param>
/// <param name="NewSpeed">The new speed in knots, or in hundredths of Mach for a Mach assignment.</param>
public sealed record AircraftAssignedSpeedChangedEvent(string Callsign, int OldSpeed, int NewSpeed);
