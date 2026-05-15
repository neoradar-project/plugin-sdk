using System;
using System.Runtime.InteropServices;

namespace NeoRadar.PluginSdk.Hosting;

// Function-pointer ABI between AOT NeoRadar and the JIT NeoRadar.PluginRuntime
// (per doc 29). The types in this file are an internal product protocol —
// plugin authors do NOT see them.
//
// They live in PluginSdk for one reason: both AOT NeoRadar and JIT
// PluginRuntime need to agree on the layouts at compile time, and PluginSdk
// is the natural shared compile-time assembly. They are declared `internal`
// + InternalsVisibleTo so plugin authors who reference PluginSdk see only
// the public SDK surface (ICoreApi, BasePlugin, Aircraft, etc.).
//
// VERSION BUMPS: any time a struct member is added/removed/reordered, bump
// HostedClrAbi.Version. The AOT and JIT sides both check the version at
// boot — a mismatch aborts with a clear log instead of corrupting memory.

internal static class HostedClrAbi
{
    /// <summary>
    /// Increment on any layout change to <see cref="HostFunctionTable"/>,
    /// <see cref="PluginRuntimeTable"/>, or any POD struct that crosses the
    /// boundary. Both sides assert this matches at Bootstrap.Start.
    /// </summary>
    /// <remarks>
    /// v1 — initial: Ping, Aircraft_GetCount, Aircraft_TryGetByCallsign.
    /// v2 — adds <see cref="PluginRuntimeTable.LoadSamplePlugin"/>.
    /// v3 — adds <see cref="PluginRuntimeTable.OnAircraftConnected"/>.
    /// v4 — adds <see cref="HostFunctionTable.Logger_Log"/>.
    /// v5 — adds <see cref="PluginRuntimeTable.OnAircraftDisconnected"/>.
    /// v6 — adds Controller_TryGetByCallsign + ControllerConnected/Disconnected events.
    /// v7 — adds Fsd_GetConnection + FsdConnected/Disconnected events.
    /// v8 — adds Flightplan_TryGetByCallsign + FlightplanUpdated/Removed events.
    /// v9 — adds ControllerData_TryGetByCallsign + ControllerData_SetGroundStatus
    ///       + OnControllerDataUpdated event (first mutator-shaped sub-API).
    /// </remarks>
    public const int Version = 9;
}

/// <summary>
/// AOT NeoRadar → JIT PluginRuntime function-pointer surface. Plugin code
/// inside PluginRuntime calls into NeoRadar via these pointers (after going
/// through its own adapter classes which marshal managed types to/from the
/// POD shape these pointers expect).
///
/// All members declared as <c>delegate* unmanaged[Cdecl]&lt;...&gt;</c>:
/// they are raw native function pointers, no managed thunk, no marshalling
/// trampoline. Calls go directly to AOT-NeoRadar's UnmanagedCallersOnly
/// static methods.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct HostFunctionTable
{
    /// <summary>Must equal <see cref="HostedClrAbi.Version"/>. Bootstrap.Start aborts on mismatch.</summary>
    public int Version;
    public int _pad0;  // align the next 64-bit pointer cleanly

    // --- Smoke test ----------------------------------------------------

    /// <summary>Returns <c>value * 2</c>. Used to validate the function-pointer
    /// boundary works at all before exercising real methods.</summary>
    public delegate* unmanaged[Cdecl]<int, int> Ping;

    // --- Aircraft ------------------------------------------------------

    /// <summary>Returns the number of aircraft currently tracked by NeoRadar.</summary>
    public delegate* unmanaged[Cdecl]<int> Aircraft_GetCount;

    /// <summary>
    /// Look up an aircraft by callsign. <paramref name="callsign"/> is a
    /// null-terminated UTF-16 string (Windows) / UTF-8 (macOS — TBD in
    /// macOS port). On a match, fills <paramref name="output"/> and
    /// returns 1. On miss returns 0. On error returns a negative status.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, AircraftSnapshot*, int> Aircraft_TryGetByCallsign;

    // --- Controller ---------------------------------------------------

    /// <summary>
    /// Look up a controller by callsign. Returns 1 on hit, 0 on miss.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, ControllerSnapshot*, int> Controller_TryGetByCallsign;

    // --- Fsd ----------------------------------------------------------

    /// <summary>
    /// Read the live FSD connection state. Returns 1 if connected (fills
    /// the snapshot), 0 if disconnected (output is zeroed).
    /// </summary>
    public delegate* unmanaged[Cdecl]<FsdConnectionSnapshot*, int> Fsd_GetConnection;

    // --- Flightplan ---------------------------------------------------

    /// <summary>
    /// Look up a flightplan by callsign. Returns 1 on hit, 0 on miss.
    /// Route + waypoints stay empty in the SDK record produced from this
    /// snapshot — full Route marshalling is deferred.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, FlightplanSnapshot*, int> Flightplan_TryGetByCallsign;

    // --- ControllerData -----------------------------------------------

    /// <summary>
    /// Look up the controller-data row keyed by aircraft callsign.
    /// Returns 1 on hit, 0 on miss.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, ControllerDataSnapshot*, int> ControllerData_TryGetByCallsign;

    /// <summary>
    /// Mutator — set the ground status (None/Start/Push/Taxi/Dep) for an
    /// aircraft. <paramref name="status"/> matches
    /// <see cref="PluginSdk.ControllerData.GroundStatus"/>. Returns 1 on
    /// success, 0 if the callsign isn't tracked, negative on error.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, int, int> ControllerData_SetGroundStatus;

    // --- Logger -------------------------------------------------------

    /// <summary>
    /// Forward a log line from plugin code into NeoRadar's Serilog pipeline.
    /// <paramref name="level"/> matches <see cref="PluginSdk.Logger.LogLevel"/>;
    /// <paramref name="message"/> is a null-terminated UTF-16 string.
    /// </summary>
    public delegate* unmanaged[Cdecl]<int, char*, void> Logger_Log;

    // Future methods append below in version-stable order. Bumping the
    // major version reorders or removes members.
}

/// <summary>
/// JIT PluginRuntime → AOT NeoRadar function-pointer surface. NeoRadar's
/// PluginManager invocation paths (LoadPlugin, UnloadPlugin, plugin
/// lifecycle hooks) eventually route through this table.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PluginRuntimeTable
{
    /// <summary>Must equal <see cref="HostedClrAbi.Version"/>.</summary>
    public int Version;
    public int _pad0;

    /// <summary>
    /// Phase B.2 smoke-test entry. Takes a null-terminated UTF-16 path to
    /// a plugin DLL, loads it via a collectible AssemblyLoadContext, finds
    /// a BasePlugin-derived type, instantiates, calls Initialize. Returns
    /// 0 on success or a negative status code.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, int> LoadSamplePlugin;

    /// <summary>
    /// AOT NeoRadar's IEventBus AircraftConnected subscription marshals
    /// through here. Parameters: null-terminated UTF-16 callsign, CID
    /// (or -1 if the aircraft has no CID). PluginRuntime constructs the
    /// AircraftConnectedEvent and fans it to every loaded plugin's
    /// OnAircraftConnected handler via EventDispatcher.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, int, void> OnAircraftConnected;

    /// <summary>AircraftDisconnected — callsign only.</summary>
    public delegate* unmanaged[Cdecl]<char*, void> OnAircraftDisconnected;

    /// <summary>
    /// ControllerConnected — the SDK ControllerConnectedEvent needs more
    /// than primitives can carry inline, so the snapshot pointer is passed
    /// instead. PluginRuntime reads it eagerly before returning.
    /// </summary>
    public delegate* unmanaged[Cdecl]<ControllerSnapshot*, void> OnControllerConnected;

    /// <summary>ControllerDisconnected — callsign only.</summary>
    public delegate* unmanaged[Cdecl]<char*, void> OnControllerDisconnected;

    /// <summary>
    /// FsdConnected — callsign of the local connection. Core fires it
    /// signal-only; this side reads server type + facility off the
    /// IFsdConnectionClient at fire time and marshals together.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, int, void> OnFsdConnected;

    /// <summary>FsdDisconnected — signal only, no data to carry.</summary>
    public delegate* unmanaged[Cdecl]<void> OnFsdDisconnected;

    /// <summary>
    /// FlightplanUpdated — snapshot pointer carries the marshalled state.
    /// PluginRuntime reads it eagerly before returning.
    /// </summary>
    public delegate* unmanaged[Cdecl]<FlightplanSnapshot*, void> OnFlightplanUpdated;

    /// <summary>FlightplanRemoved — callsign only.</summary>
    public delegate* unmanaged[Cdecl]<char*, void> OnFlightplanRemoved;

    /// <summary>
    /// ControllerDataUpdated — callsign only. The SDK event carries just
    /// the callsign; the plugin calls api.ControllerData.GetByCallsign to
    /// fetch the full row if it wants the data.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, void> OnControllerDataUpdated;
}

/// <summary>
/// Aircraft data marshalled across the function-pointer boundary. All
/// fields are primitives or fixed-size buffers. Layout pinned by
/// <see cref="HostedClrAbi.Version"/>.
///
/// Strings are inline UTF-16 buffers with fixed capacity. NeoRadar's
/// callsigns are at most 12 characters; 16 covers everything with room
/// for the null terminator.
/// </summary>
/// <summary>
/// ControllerData row marshalled across the function-pointer boundary.
/// Mirrors PluginSdk.ControllerData.ControllerDataModel for every field
/// that survives a POD round-trip — strings inline, numerics direct,
/// enums as byte.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal unsafe struct ControllerDataSnapshot
{
    public const int CallsignCapacity = 24;
    public const int DirectCapacity = 16;
    public const int SquawkCapacity = 8;
    public const int ScratchpadCapacity = 64;
    public const int RawScratchpadCapacity = 128;

    public fixed char Callsign[CallsignCapacity];
    public fixed char AssignedDirect[DirectCapacity];
    public fixed char AssignedSquawk[SquawkCapacity];
    public fixed char Scratchpad[ScratchpadCapacity];
    public fixed char RawScratchpad[RawScratchpadCapacity];
    public fixed char OwnedByCallsign[CallsignCapacity];
    public fixed char FutureOwnerCallsign[CallsignCapacity];

    public int ClearedFlightLevel;
    public int AssignedHeading;
    public int AssignedSpeed;
    public int AssignedMach;
    public int AssignedVerticalRate;
    public int ClearanceQueuePosition;

    public byte ClearanceIssued;
    public byte OwnedByMe;
    public byte AssignedSpeedVariance;  // PluginSdk.ControllerData.AssignedSpeedVariance
    public byte AttentionState;          // PluginSdk.ControllerData.AttentionState
    public byte GroundStatus;            // PluginSdk.ControllerData.GroundStatus
    public byte _pad0;
    public byte _pad1;
    public byte _pad2;
}

/// <summary>
/// Flightplan data marshalled across the function-pointer boundary.
/// Carries the identity + plan strings; Route, waypoints, and time fields
/// stay empty in the SDK Flightplan record produced from this snapshot.
/// Route marshalling needs full waypoint + segment serialisation —
/// deferred until a plugin actually needs it inline.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal unsafe struct FlightplanSnapshot
{
    public const int CallsignCapacity = 24;
    public const int RuleCapacity = 8;
    public const int AcTypeCapacity = 16;
    public const int IcaoCapacity = 8;
    public const int AltitudeCapacity = 16;
    public const int RemarksCapacity = 256;

    public fixed char Callsign[CallsignCapacity];
    public fixed char FlightRule[RuleCapacity];
    public fixed char AcType[AcTypeCapacity];
    public fixed char Origin[IcaoCapacity];
    public fixed char Destination[IcaoCapacity];
    public fixed char Alternate[IcaoCapacity];
    public fixed char PlannedAltitude[AltitudeCapacity];
    public fixed char Remarks[RemarksCapacity];

    public byte IsValid;
    public byte _pad0;
    public byte _pad1;
    public byte _pad2;
}

/// <summary>
/// FSD connection state marshalled across the function-pointer boundary.
/// Plugins fetch this on demand via api.Fsd.GetConnection. Mirrors
/// PluginSdk.Fsd.ConnectionInfo for the fields readable from
/// IFsdConnectionClient — AtisLines / Frequencies arrays land empty in
/// the SDK record (deferred to Phase C array-marshalling follow-up).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal unsafe struct FsdConnectionSnapshot
{
    public const int CallsignCapacity = 24;
    public const int NameCapacity = 64;

    public fixed char Callsign[CallsignCapacity];
    public fixed char Name[NameCapacity];

    public int Cid;
    public byte Rating;     // PluginSdk.Fsd.NetworkRating
    public byte Facility;   // PluginSdk.Fsd.NetworkFacility
    public byte ServerType; // PluginSdk.Fsd.ServerType
    public byte IsATC;
    public byte IsConnected;
    public byte _pad0;
    public byte _pad1;
    public byte _pad2;
}

/// <summary>
/// Controller data marshalled across the function-pointer boundary.
/// Mirrors NeoRadar.Core.Controller.Controller for the fields plugins
/// typically need — identity, position, capabilities. Array fields
/// (Frequencies, AtisLines) are left as empty arrays in the SDK record
/// produced from this snapshot; richer array marshalling is a Phase C
/// follow-up if any plugin actually needs the live frequencies list
/// inline with the event.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal unsafe struct ControllerSnapshot
{
    public const int CallsignCapacity = 24;
    public const int NameCapacity = 64;
    public const int AtisLetterCapacity = 8;
    public const int SectorFileNameCapacity = 64;

    public fixed char Callsign[CallsignCapacity];
    public fixed char Name[NameCapacity];
    public fixed char AtisLetter[AtisLetterCapacity];
    public fixed char SectorFileName[SectorFileNameCapacity];

    public int Cid;            // 0 if unset (NeoRadar.Core stores it as int; SDK Controller.Cid is a string for display)
    public byte Rating;        // PluginSdk.Controller.Rating enum value
    public byte Facility;      // PluginSdk.Controller.FacilityType enum value
    public byte IsATC;
    public byte IsATIS;

    public double Latitude;
    public double Longitude;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal unsafe struct AircraftSnapshot
{
    public const int CallsignCapacity = 16;
    public const int SquawkCapacity = 8;
    public const int NameCapacity = 64;

    // String buffers — fixed sized to avoid heap allocation per call.
    public fixed char Callsign[CallsignCapacity];
    public fixed char Squawk[SquawkCapacity];
    public fixed char Name[NameCapacity];

    // Identity / status
    public int Cid;
    public byte HasCid;
    public byte HasPilotDetails;
    public byte IsTimingOut;
    public byte IsReady;
    public byte TransponderMode;  // matches TransponderMode enum byte value

    // Position — flattened from PluginSdk.Aircraft.Position
    public byte OnGround;
    public byte Stopped;
    public byte _pad0;

    public double Latitude;
    public double Longitude;
    public int Altitude;
    public double Agl;
    public double TrueAltitude;
    public double PressureAltitude;
    public int GroundSpeed;
    public int ReportedHeading;
    public int TrackHeading;
    public int VerticalSpeed;
    public int VerticalTrend;
    public double VelX;
    public double VelY;
    public double VelZ;
    public double VelH;
    public double Pitch;
    public double Bank;
    public int PressureDifference;
}
