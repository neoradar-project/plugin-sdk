using System;
using System.Runtime.InteropServices;

namespace NeoRadar.PluginSdk.Hosting;

// Function-pointer ABI between AOT NeoRadar and the JIT NeoRadar.PluginRuntime
// (per doc 29). Both runtimes reference NeoRadar.PluginSdk so they agree on
// these layouts at compile time.
//
// VERSION BUMPS: any time a struct member is added/removed/reordered, bump
// AbiVersion. The AOT and JIT sides both check the version at boot — a
// mismatch aborts with a clear log instead of corrupting memory.

public static class HostedClrAbi
{
    /// <summary>
    /// Increment on any layout change to <see cref="HostFunctionTable"/>,
    /// <see cref="PluginRuntimeTable"/>, or any POD struct that crosses the
    /// boundary. Both sides assert this matches at Bootstrap.Start.
    /// </summary>
    public const int Version = 1;
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
public unsafe struct HostFunctionTable
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

    // Future methods append below in version-stable order. Bumping the
    // major version reorders or removes members.
}

/// <summary>
/// JIT PluginRuntime → AOT NeoRadar function-pointer surface. NeoRadar's
/// PluginManager invocation paths (LoadPlugin, UnloadPlugin, plugin
/// lifecycle hooks) eventually route through this table.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct PluginRuntimeTable
{
    /// <summary>Must equal <see cref="HostedClrAbi.Version"/>.</summary>
    public int Version;
    public int _pad0;

    // Phase B.1 has no methods here yet — Bootstrap returns a populated
    // Version field and reserved padding. Phase B.2 onwards adds
    // LoadPlugin, UnloadPlugin, DispatchEvent, OnFrameUpdate, etc.
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
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct AircraftSnapshot
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
