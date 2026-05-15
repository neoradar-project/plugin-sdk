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
    /// </remarks>
    public const int Version = 6;
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
