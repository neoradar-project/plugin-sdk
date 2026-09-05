using System;
using System.Runtime.InteropServices;

namespace NeoRadar.PluginSdk.Hosting;

// Function-pointer ABI between AOT NeoRadar and JIT NeoRadar.PluginRuntime.
// Bump HostedClrAbi.Version on any layout change to HostFunctionTable, PluginRuntimeTable,
// or any POD struct that crosses the boundary. Bootstrap.Start aborts on mismatch.

internal static class HostedClrAbi
{
    // Increment on any field add/remove/reorder in the tables or any boundary-crossing POD struct.
    public const int Version = 39;
}

// AOT NeoRadar → JIT PluginRuntime surface. Members are raw native fn-pointers
// (delegate* unmanaged[Cdecl]) calling AOT UnmanagedCallersOnly statics directly.
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct HostFunctionTable
{
    public int Version;
    public int _pad0;  // align the next 64-bit pointer cleanly

    // Boundary smoke test: returns value * 2.
    public delegate* unmanaged[Cdecl]<int, int> Ping;

    public delegate* unmanaged[Cdecl]<int> Aircraft_GetCount;

    // callsign is null-terminated UTF-16 on every platform. 1 on hit, 0 on miss, negative on error.
    public delegate* unmanaged[Cdecl]<char*, AircraftSnapshot*, int> Aircraft_TryGetByCallsign;

    // Great-circle nm to flightplan origin; NaN when unavailable.
    public delegate* unmanaged[Cdecl]<char*, double> Aircraft_GetDistanceFromOrigin;

    public delegate* unmanaged[Cdecl]<char*, double> Aircraft_GetDistanceToDestination;

    // Capacity-hint protocol: ≥0 count written; -1 null/neg capacity; -2 unbound; <-2 negated required length, caller retries bigger.
    public delegate* unmanaged[Cdecl]<AircraftSnapshot*, int, int> Aircraft_Enumerate;

    public delegate* unmanaged[Cdecl]<char*, ControllerSnapshot*, int> Controller_TryGetByCallsign;

    public delegate* unmanaged[Cdecl]<ControllerSnapshot*, int, int> Controller_Enumerate;

    // Array fields outside the POD snapshot as UTF-8 JSON {"freqs":[…],"atis":[…]}. Capacity-hint protocol.
    public delegate* unmanaged[Cdecl]<char*, byte*, int, int> Controller_GetExtrasJson;

    // 1 if connected (fills snapshot), 0 if disconnected (output zeroed).
    public delegate* unmanaged[Cdecl]<FsdConnectionSnapshot*, int> Fsd_GetConnection;

    // Same JSON shape as Controller_GetExtrasJson.
    public delegate* unmanaged[Cdecl]<byte*, int, int> Fsd_GetExtrasJson;

    // Route + waypoints stay empty in the produced record; fetch via Flightplan_GetRouteJson.
    public delegate* unmanaged[Cdecl]<char*, FlightplanSnapshot*, int> Flightplan_TryGetByCallsign;

    public delegate* unmanaged[Cdecl]<FlightplanSnapshot*, int, int> Flightplan_Enumerate;

    // Parsed Route as UTF-8 JSON; capacity-hint protocol.
    public delegate* unmanaged[Cdecl]<char*, byte*, int, int> Flightplan_GetRouteJson;

    public delegate* unmanaged[Cdecl]<char*, char*, int> Airport_IsDepRunwayActive;

    public delegate* unmanaged[Cdecl]<char*, char*, int> Airport_IsArrRunwayActive;

    // Type matches PluginSdk.Airport.RunwayType (Departure=0, Arrival=1).
    public delegate* unmanaged[Cdecl]<char*, char*, int, int> Airport_SetRunwayStatus;

    public delegate* unmanaged[Cdecl]<char*, char*, int, int> Airport_RemoveRunwayStatus;

    // Status matches PluginSdk.Airport.AirportStatus (Active=0, Observed=1).
    public delegate* unmanaged[Cdecl]<char*, int, int> Airport_SetAirportStatus;

    public delegate* unmanaged[Cdecl]<char*, int, int> Airport_SetShowCenterlines;

    public delegate* unmanaged[Cdecl]<char*, int> Airport_DeleteAirport;

    // AirportConfig[] as UTF-8 JSON; capacity-hint protocol.
    public delegate* unmanaged[Cdecl]<byte*, int, int> Airport_GetConfigurationsJson;

    // 1 on success, 0 if the name is already registered, negative on error.
    public delegate* unmanaged[Cdecl]<char*, int, ulong, int> Squawk_RegisterProvider;

    public delegate* unmanaged[Cdecl]<char*, int> Squawk_UnregisterProvider;

    public delegate* unmanaged[Cdecl]<char*, int> Squawk_SetActiveProvider;

    // Provider names as JSON array; capacity-hint protocol.
    public delegate* unmanaged[Cdecl]<byte*, int, int> Squawk_GetAvailableProvidersJson;

    // Args: pluginId, pluginName, name, definition JSON, callbackId, out command id (UTF-16) + capacity. Returns chars written, 0 on collision.
    public delegate* unmanaged[Cdecl]<char*, char*, char*, byte*, int, ulong, char*, int, int> Chat_RegisterCommand;

    public delegate* unmanaged[Cdecl]<char*, int> Chat_UnregisterCommand;

    // Args: sentFrom, message, hasColour, R, G, B, isUrgent, useDedicatedChannel.
    public delegate* unmanaged[Cdecl]<char*, char*, int, byte, byte, byte, int, int, int> Chat_SendClientMessage;

    // Args: pluginId, name, default value, title, description, valueType, allowed-actions JSON + len, out tag id (UTF-16) + capacity. Returns chars written, 0 on collision.
    public delegate* unmanaged[Cdecl]<char*, char*, char*, char*, char*, int, byte*, int, char*, int, int> Tag_RegisterTagItem;

    // Args: pluginId, name, description, title, requiresInput, out action id (UTF-16) + capacity. Returns chars written, 0 on collision.
    public delegate* unmanaged[Cdecl]<char*, char*, char*, char*, int, char*, int, int> Tag_RegisterTagAction;

    // Args: tagId, value, callsign, listId, hasColour + RGB, hasBgColour + bgRGB, context-data JSON + len.
    public delegate* unmanaged[Cdecl]<char*, char*, char*, char*, int, byte, byte, byte, int, byte, byte, byte, byte*, int, int> Tag_UpdateTagValue;

    // Args: pluginId, actionId, dropdownJson + len.
    public delegate* unmanaged[Cdecl]<char*, char*, byte*, int, int> Tag_SetActionDropdown;

    public delegate* unmanaged[Cdecl]<char*, char*, byte*, int, int> Tag_UpdateActionDropdown;

    public delegate* unmanaged[Cdecl]<char*, char*, int> Tag_RemoveActionDropdown;

    // Capacity-hint protocol; 0 when the action has no dropdown.
    public delegate* unmanaged[Cdecl]<char*, byte*, int, int> Tag_GetDropdownForActionJson;

    // Window_OpenWindow returns the assigned window handle as a ulong (0 = failure).
    public delegate* unmanaged[Cdecl]<char*, byte*, int, ulong> Window_OpenWindow;
    public delegate* unmanaged[Cdecl]<char*, ulong, char*, char*, int> Window_UpdateElement;
    public delegate* unmanaged[Cdecl]<char*, ulong, int> Window_CloseWindow;

    // Args: pluginId, definitionJson + len, callbackId. Returns screen handle (0 on failure).
    public delegate* unmanaged[Cdecl]<char*, byte*, int, ulong, ulong> Rendering_RegisterScreen;

    public delegate* unmanaged[Cdecl]<ulong, int> Rendering_UnregisterScreen;
    public delegate* unmanaged[Cdecl]<ulong, int, int> Rendering_SetScreenEnabled;
    public delegate* unmanaged[Cdecl]<ulong, int> Rendering_IsScreenEnabled;

    // Add* returns ObjectId (ulong; 0 on failure). RGBA passed as four floats; RgbaColor is already float-shaped.
    public delegate* unmanaged[Cdecl]<ulong, float*, int, float, float, float, float, float, ulong> Graphics_AddScreenLine;
    public delegate* unmanaged[Cdecl]<ulong, float, float, float, float, float, float, float, float, ulong> Graphics_AddScreenRect;
    public delegate* unmanaged[Cdecl]<ulong, float, float, float, float, float, float, float, float, float, ulong> Graphics_AddScreenStrokeRect;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, float, float, ulong> Graphics_AddScreenPoint;
    public delegate* unmanaged[Cdecl]<ulong, float, float, char*, float, float, float, float, float, ulong> Graphics_AddScreenText;

    public delegate* unmanaged[Cdecl]<ulong, ulong, int> Graphics_Remove;
    public delegate* unmanaged[Cdecl]<ulong, int> Graphics_Clear;
    public delegate* unmanaged[Cdecl]<ulong, ulong, int, int> Graphics_SetVisible;
    public delegate* unmanaged[Cdecl]<ulong, ulong, float, float, float, float, int> Graphics_SetColor;
    public delegate* unmanaged[Cdecl]<ulong, ulong, char*, int> Graphics_SetText;

    public delegate* unmanaged[Cdecl]<ulong, float*, int> Graphics_GetScreenSize;

    // Vector2 arrays flatten as X,Y float pairs. Polyline/polygon outer level uses int* lengths + int outerCount.
    public delegate* unmanaged[Cdecl]<ulong, float*, int, float, float, float, float, float, ulong> Graphics_AddWorldLine;
    public delegate* unmanaged[Cdecl]<ulong, float*, int*, int, float, float, float, float, float, ulong> Graphics_AddWorldPolyline;
    public delegate* unmanaged[Cdecl]<ulong, float*, int*, int, float, float, float, float, ulong> Graphics_AddWorldPolygon;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, float, float, ulong> Graphics_AddWorldPoint;
    public delegate* unmanaged[Cdecl]<ulong, float, float, char*, float, float, float, float, float, ulong> Graphics_AddWorldText;

    public delegate* unmanaged[Cdecl]<ulong, ulong, float, float, float, float, float, float, float, int> Graphics_SetTransform;
    public delegate* unmanaged[Cdecl]<ulong, ulong, float, int> Graphics_SetMinZoom;
    public delegate* unmanaged[Cdecl]<ulong, ulong, int, int> Graphics_SetHitTestVisible;
    public delegate* unmanaged[Cdecl]<ulong, ulong, float, float, int> Graphics_SetScreenAnchor;
    public delegate* unmanaged[Cdecl]<ulong, ulong, float*, int, int> Graphics_SetScreenPoints;
    public delegate* unmanaged[Cdecl]<ulong, ulong, float, float, int> Graphics_SetScreenSize;
    public delegate* unmanaged[Cdecl]<ulong, ulong, int> Graphics_BringToFront;
    public delegate* unmanaged[Cdecl]<ulong, ulong, int> Graphics_SendToBack;

    public delegate* unmanaged[Cdecl]<ulong, void> Graphics_CapturePointer;
    public delegate* unmanaged[Cdecl]<ulong, void> Graphics_ReleasePointer;
    public delegate* unmanaged[Cdecl]<ulong, int> Graphics_HasCapture;

    public delegate* unmanaged[Cdecl]<ulong, double, double, float*, int> Graphics_LatLonToWorld;
    public delegate* unmanaged[Cdecl]<ulong, float, float, double*, int> Graphics_WorldToLatLon;
    public delegate* unmanaged[Cdecl]<ulong, float, float, float*, int> Graphics_ScreenToWorld;
    public delegate* unmanaged[Cdecl]<ulong, float, float, float*, int> Graphics_WorldToScreen;

    // 6 doubles: CenterX, CenterY, Zoom, WidthPixels, HeightPixels, PixelsPerMeter. 0 if no viewport bound (screen-only scope).
    public delegate* unmanaged[Cdecl]<ulong, double*, int> Graphics_GetViewport;

    public delegate* unmanaged[Cdecl]<char*, ControllerDataSnapshot*, int> ControllerData_TryGetByCallsign;

    // status matches PluginSdk.ControllerData.GroundStatus.
    public delegate* unmanaged[Cdecl]<char*, int, int> ControllerData_SetGroundStatus;

    public delegate* unmanaged[Cdecl]<ControllerDataSnapshot*, int, int> ControllerData_Enumerate;

    // level matches PluginSdk.Logger.LogLevel; message is null-terminated UTF-16.
    public delegate* unmanaged[Cdecl]<int, char*, void> Logger_Log;

    // Soft-sandbox disabled a plugin. Args: pluginId, pluginName, reason (all UTF-16). AOT posts a Feedback toast + stashes a faulted badge.
    public delegate* unmanaged[Cdecl]<char*, char*, char*, void> NotifyPluginDisabled;

    // Fired by PluginRegistry.Remove after Alc.Unload (manual or soft-sandbox). AOT drops its path tracker, cleans the temp dir, fires PluginUnloaded.
    public delegate* unmanaged[Cdecl]<char*, void> NotifyPluginUnloaded;

    // UTF-16 buffer; capacity includes null terminator. >0 chars written, 0 no package, <0 error (-1 null, -2 unbound, -3 truncation).
    public delegate* unmanaged[Cdecl]<char*, int, int> Package_GetPath;

    // Future methods append below in version-stable order; bumping the major version reorders or removes members.
}

// AOT NeoRadar → JIT PluginRuntime lifecycle + event surface, invoked by HostedClrPluginManager.
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PluginRuntimeTable
{
    public int Version;
    public int _pad0;

    // Direct load entry kept for ABI compat; prefer LoadPlugin which also returns the assigned id.
    public delegate* unmanaged[Cdecl]<char*, int> LoadSamplePlugin;

    // Args: dllPath, out id buffer (UTF-16) + capacity. >0 chars written; 0 id buffer too small; <0 PluginLoader status.
    public delegate* unmanaged[Cdecl]<char*, char*, int, int> LoadPlugin;

    public delegate* unmanaged[Cdecl]<char*, int> UnloadPlugin;

    public delegate* unmanaged[Cdecl]<int> UnloadAllPlugins;

    // (id, path, manifest) tuples as JSON; capacity-hint protocol.
    public delegate* unmanaged[Cdecl]<byte*, int, int> GetLoadedPluginsJson;

    // callsign (UTF-16) + CID (-1 if none). PluginRuntime fans the event to every plugin.
    public delegate* unmanaged[Cdecl]<char*, int, void> OnAircraftConnected;

    public delegate* unmanaged[Cdecl]<char*, void> OnAircraftDisconnected;

    // Snapshot pointer (event needs more than primitives); read eagerly before returning.
    public delegate* unmanaged[Cdecl]<ControllerSnapshot*, void> OnControllerConnected;

    public delegate* unmanaged[Cdecl]<char*, void> OnControllerDisconnected;

    // Core fires signal-only; this side reads server type + facility off IFsdConnectionClient and marshals together.
    public delegate* unmanaged[Cdecl]<char*, int, void> OnFsdConnected;

    public delegate* unmanaged[Cdecl]<void> OnFsdDisconnected;

    // Snapshot pointer read eagerly before returning.
    public delegate* unmanaged[Cdecl]<FlightplanSnapshot*, void> OnFlightplanUpdated;

    public delegate* unmanaged[Cdecl]<char*, void> OnFlightplanRemoved;

    // callsign only; plugin calls ControllerData.GetByCallsign for the row.
    public delegate* unmanaged[Cdecl]<char*, void> OnControllerDataUpdated;

    public delegate* unmanaged[Cdecl]<char*, void> OnAirportAdded;

    public delegate* unmanaged[Cdecl]<char*, void> OnAirportRemoved;

    public delegate* unmanaged[Cdecl]<char*, int, void> OnAirportStatusChanged;

    public delegate* unmanaged[Cdecl]<char*, char*, int, int, void> OnRunwayStatusChanged;

    public delegate* unmanaged[Cdecl]<int, void> OnAirportConfigurationsUpdated;

    // Args: callbackId, callsign, aircraft snapshot, hasFp, flightplan snapshot (valid only when hasFp), out buffer (UTF-16) + capacity.
    // Returns chars written, 0 empty. Capacity-hint protocol; the hint includes the null terminator.
    public delegate* unmanaged[Cdecl]<ulong, char*, AircraftSnapshot*, int, FlightplanSnapshot*, char*, int, int> Squawk_GenerateSquawk;

    // Args: callbackId, commandId, args JSON (UTF-8) + len, out result JSON {"success":bool,"message":"…"} + capacity. Capacity-hint protocol.
    public delegate* unmanaged[Cdecl]<ulong, char*, byte*, int, byte*, int, int> Chat_ExecuteCommand;

    public delegate* unmanaged[Cdecl]<char*, char*, char*, void> OnSquawkAssigned;

    public delegate* unmanaged[Cdecl]<char*, void> OnFsdError;

    public delegate* unmanaged[Cdecl]<char*, void> OnFsdConnectionModelUpdated;

    public delegate* unmanaged[Cdecl]<char*, double, double, void> OnAtcPositionUpdate;

    public delegate* unmanaged[Cdecl]<char*, int, void> OnAtisLinesUpdate;

    public delegate* unmanaged[Cdecl]<char*, void> OnCapabilitiesUpdate;

    public delegate* unmanaged[Cdecl]<char*, char*, void> OnFlightplanRouteChanged;

    public delegate* unmanaged[Cdecl]<char*, byte, byte, void> OnFlightplanVoiceTypeChanged;

    public delegate* unmanaged[Cdecl]<char*, char*, char*, void> OnAircraftBeaconCodeChanged;
    public delegate* unmanaged[Cdecl]<char*, char*, char*, void> OnAircraftHandoffCancelled;
    public delegate* unmanaged[Cdecl]<char*, char*, char*, void> OnAircraftOwnedByChanged;
    public delegate* unmanaged[Cdecl]<char*, char*, void> OnAircraftHandoffRejected;
    public delegate* unmanaged[Cdecl]<char*, void> OnAircraftTerminatedTracking;
    public delegate* unmanaged[Cdecl]<char*, void> OnAircraftInitiatedTracking;
    public delegate* unmanaged[Cdecl]<char*, int, int, void> OnAircraftTemporaryAltitudeChanged;
    public delegate* unmanaged[Cdecl]<char*, int, byte, void> OnAircraftCDMStatusChanged;
    public delegate* unmanaged[Cdecl]<char*, char*, void> OnAircraftScratchpadUpdated;
    public delegate* unmanaged[Cdecl]<char*, int, int, void> OnAircraftHeadingChanged;
    public delegate* unmanaged[Cdecl]<char*, int, int, void> OnAircraftAssignedSpeedChanged;

    public delegate* unmanaged[Cdecl]<AircraftSnapshot*, int, void> OnPositionUpdate;

    public delegate* unmanaged[Cdecl]<char*, char*, char*, void> OnATISInfoMessageReceived;

    public delegate* unmanaged[Cdecl]<char*, int, char*, void> OnFlightplanMessageReceived;

    // Args: pluginId, windowHandle, elementId, eventType, value.
    public delegate* unmanaged[Cdecl]<char*, ulong, char*, char*, char*, void> OnWindowEvent;

    // Args: actionId, tagId, callsign, button, hasUserInput, userInput, dataJson (UTF-8) + len.
    public delegate* unmanaged[Cdecl]<char*, char*, char*, int, int, char*, byte*, int, void> OnTagAction;

    // Args: actionId, componentId, tagId, callsign, hasUserInput, userInput, dataJson (UTF-8) + len.
    public delegate* unmanaged[Cdecl]<char*, char*, char*, char*, int, char*, byte*, int, void> OnTagDropdownAction;

    // Args: callbackId, screenHandle, deltaTime, frameNumber, centerX, centerY, zoom, pixelsPerMeter,
    //   widthPixels/heightPixels (viewport physical px), screenWidthDip/screenHeightDip (DIP, plugin coord space).
    // ScreenSize travels separately from pixel dims: on Retina they differ by the dpi factor.
    public delegate* unmanaged[Cdecl]<ulong, ulong, double, ulong, double, double, double, double, double, double, double, double, void> Render_OnFrameUpdate;

    // Pointer args: callbackId, screenX, screenY, hasWorld, worldX, worldY, button, modifiers, clickCount, hasHit, hitObjectId, hitScreenId.
    // hitScreenId is required: plugins key Dictionary<GraphicsObjectRef,…> by (ScreenId, ObjectId); dropping it silently breaks every lookup.
    // For event-consume callbacks (Pressed/Wheel/KeyDown/KeyUp) the trailing `int* consumed` is set 0/1 by the JIT entry.
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, int, int, int, int, ulong, ulong, int*, void> Render_OnPointerPressed;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, int, int, int, int, ulong, ulong, void> Render_OnPointerMoved;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, int, int, int, int, ulong, ulong, void> Render_OnPointerReleased;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, float, float, int, int, ulong, ulong, int*, void> Render_OnPointerWheel;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, int, int, int, int, ulong, ulong, void> Render_OnPointerEntered;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, int, int, int, int, ulong, ulong, void> Render_OnPointerExited;
    public delegate* unmanaged[Cdecl]<ulong, int, int, char*, int*, void> Render_OnKeyDown;
    public delegate* unmanaged[Cdecl]<ulong, int, int, char*, int*, void> Render_OnKeyUp;
    public delegate* unmanaged[Cdecl]<ulong, void> Render_OnEnabled;
    public delegate* unmanaged[Cdecl]<ulong, void> Render_OnDisabled;

    // pluginId identifies the owning plugin so JIT delivers only to that handler. OnViewportChanged is global.
    public delegate* unmanaged[Cdecl]<char*, ulong, double, double, double, double, int, void> OnRenderObjectClicked;
    public delegate* unmanaged[Cdecl]<double, double, double, double, double, double, void> OnViewportChanged;

    public delegate* unmanaged[Cdecl]<char*, int, void> OnIsControllerATC;

    public delegate* unmanaged[Cdecl]<int, void> OnFsdConnectionStateChange;

    // Sync-RPC. JIT queries every plugin's OnTagShowDropdown; writes 0 to allowOut if any returns false. Unwritten output = 1 (allow).
    public delegate* unmanaged[Cdecl]<char*, char*, int*, void> OnTagShowDropdown;

    // Like OnRenderObjectClicked but no button; pluginId routes to the owning plugin.
    public delegate* unmanaged[Cdecl]<char*, ulong, double, double, double, double, void> OnRenderObjectHovered;
}

// ControllerData row marshalled across the boundary. Layout pinned by HostedClrAbi.Version.
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

// Flightplan POD shape. Route / waypoints / time fields don't fit and arrive via Flightplan_GetRouteJson.
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

// FSD connection POD shape. AtisLines / Frequencies arrive separately via Fsd_GetExtrasJson.
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

// Controller POD shape. Frequencies / AtisLines arrive via Controller_GetExtrasJson.
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

    public fixed char Callsign[CallsignCapacity];
    public fixed char Squawk[SquawkCapacity];
    public fixed char Name[NameCapacity];

    public int Cid;
    public byte HasCid;
    public byte HasPilotDetails;
    public byte IsTimingOut;
    public byte IsReady;
    public byte TransponderMode;  // matches TransponderMode enum byte value

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
