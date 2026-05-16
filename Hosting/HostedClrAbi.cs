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
    /// v10 — adds Airport surface (Is*RunwayActive, SetAirportStatus,
    ///        SetRunwayStatus, RemoveRunwayStatus, SetShowCenterlines)
    ///        + 5 airport events. GetConfigurationByIcao deferred pending
    ///        array marshalling for the runway lists.
    /// v11 — adds Package_GetPath. Caller-provided UTF-16 buffer + capacity;
    ///        AOT writes the path (null-terminated) and returns char count
    ///        written, 0 if no package loaded, negative on error / truncation.
    /// v12 — adds Aircraft_Enumerate. Caller-provided AircraftSnapshot array
    ///        + capacity; AOT fills the first min(snapshot.Length, capacity)
    ///        entries. Returns count on success, -snapshot.Length when the
    ///        buffer is too small (capacity hint for retry).
    /// v13 — adds Controller_Enumerate (same capacity-hint shape as v12).
    /// v14 — adds Flightplan_Enumerate (same capacity-hint shape).
    /// v15 — adds Airport_GetConfigurationsJson. AOT writes UTF-8 JSON via
    ///        Utf8JsonWriter (AOT-safe, reflection-free); JIT parses via
    ///        JsonDocument. Template for future array-rich payloads
    ///        (Controller.Frequencies, Flightplan.Route, etc.) where the
    ///        fixed-buffer POD shape doesn't fit.
    /// v16 — adds the JIT→AOT→JIT callback bridge. First user is Squawk:
    ///        Squawk_RegisterProvider / Unregister / SetActive /
    ///        GetAvailableProvidersJson on the host side; Squawk_GenerateSquawk
    ///        on the plugin-runtime side. Plugin gives the host a managed
    ///        callback; host invokes it later via a function pointer keyed
    ///        by an opaque ulong callback id. Same template will apply to
    ///        Chat / Tag / Rendering / Window.
    /// v17 — adds Chat command registration + execute callback. Definition,
    ///        args, and result marshalled as UTF-8 JSON via Utf8JsonWriter
    ///        (AOT) / JsonDocument (JIT). SendClientMessage primitive params.
    /// v18 — adds Tag registration core: Tag_RegisterTagItem,
    ///        Tag_RegisterTagAction, Tag_UpdateTagValue. Pure registration
    ///        + push, no callback inversion needed (tag-action events flow
    ///        through the existing event delivery path). Dropdown methods
    ///        on the JIT-side adapter return false/null stubs; full JSON
    ///        marshal lands as a follow-up.
    /// v19 — adds 8 events: OnSquawkAssigned, OnFsdError,
    ///        OnFsdConnectionModelUpdated, OnAtcPositionUpdate,
    ///        OnAtisLinesUpdate, OnCapabilitiesUpdate,
    ///        OnFlightplanRouteChanged, OnFlightplanVoiceTypeChanged.
    /// v20 — adds 11 ControllerData granular events:
    ///        OnAircraftBeaconCodeChanged, OnAircraftHandoffCancelled,
    ///        OnAircraftOwnedByChanged, OnAircraftHandoffRejected,
    ///        OnAircraftTerminatedTracking, OnAircraftInitiatedTracking,
    ///        OnAircraftTemporaryAltitudeChanged, OnAircraftCDMStatusChanged,
    ///        OnAircraftScratchpadUpdated, OnAircraftHeadingChanged,
    ///        OnAircraftAssignedSpeedChanged.
    /// v21 — adds OnPositionUpdate (per-batch — AircraftSnapshot[] marshal)
    ///        + OnATISInfoMessageReceived.
    /// v22 — adds OnFlightplanMessageReceived (TextMessageReceived filtered
    ///        by Kind=Flightplan).
    /// v23 — adds full Tag dropdown JSON marshal:
    ///        Tag_SetActionDropdown, Tag_UpdateActionDropdown,
    ///        Tag_RemoveActionDropdown, Tag_GetDropdownForActionJson.
    ///        Schema: nested DropdownComponent tree with Style + Data + Children.
    /// v24 — adds IWindowApi registration: Window_OpenWindow,
    ///        Window_UpdateElement, Window_CloseWindow. Plugin's
    ///        WindowDefinition (Element tree + Style) marshalled as JSON
    ///        into a Core PluginWindowCommand enqueued on
    ///        IPluginWindowQueue. OnWindowEvent callback path layers on
    ///        top with the IPluginEventSink replacement.
    /// v25 — adds OnTagAction + OnTagDropdownAction. Data dict marshalled
    ///        as JSON via the same pattern as Chat ExecuteCommand args.
    /// v26 — first rendering wave: screen registration lifecycle
    ///        (Rendering_RegisterScreen / Unregister / SetEnabled /
    ///        IsScreenEnabled), per-frame callback (Render_OnFrameUpdate),
    ///        and the screen-space IPluginGraphics surface
    ///        (AddScreenLine/Rect/StrokeRect/Point/Text + Remove + Clear +
    ///        SetVisible + SetColor + SetText). World-space draws + mutate
    ///        the rest land in v27+.
    /// v27 — IPluginGraphics complete: world-space adds
    ///        (AddWorldLine/Polyline/Polygon/Point/Text), remaining mutate
    ///        verbs (SetTransform/MinZoom/HitTestVisible/ScreenAnchor/
    ///        ScreenPoints/ScreenSize + BringToFront/SendToBack), pointer
    ///        capture (Capture/Release/HasCapture), coord helpers
    ///        (LatLonToWorld/WorldToLatLon/ScreenToWorld/WorldToScreen),
    ///        viewport accessor.
    /// v28 — per-screen IPluginScreenRenderer pointer + key callbacks
    ///        (Render_OnPointerPressed/Moved/Released/Wheel/Entered/Exited,
    ///        Render_OnKeyDown/Up, Render_OnEnabled/Disabled) +
    ///        broadcast OnRenderObjectClicked + OnViewportChanged events
    ///        via the HostedPluginEventSink. Sync-RPC bool returns
    ///        (Pressed/Wheel/KeyDown/KeyUp) use the existing out-param
    ///        pattern proven on Squawk_GenerateSquawk.
    /// v29 — last 4 BasePlugin events:
    ///        - OnIsControllerATC (Core publishes new IsControllerATCResolved
    ///          event from HandleATCResponse)
    ///        - OnFsdConnectionStateChange (synthesised from
    ///          FsdConnected/FsdDisconnected in the AOT bridge)
    ///        - OnTagShowDropdown (Core TagDropdownCoordinator gains a
    ///          filter list; AOT registers a filter that sync-RPCs into the
    ///          plugin and ANDs the result)
    ///        - OnRenderObjectHovered (Core IPluginEventSink gains
    ///          SendHoverEvent; PluginScreensModule.OnPointerMoved fires it
    ///          on hit-test result)
    /// v30 — Phase D cleanup: NotifyPluginDisabled host fn. PluginRegistry.
    ///        Disable invokes it; AOT-side posts a Feedback toast via
    ///        IFeedbackStore so faulted/slow plugins surface to the user.
    /// v31 — Controller_GetExtrasJson + Fsd_GetExtrasJson. JIT-side fills
    ///        the SDK Controller.Frequencies/AtisLines/AtisLetter and
    ///        ConnectionInfo.AtisLines/Frequencies fields by fetching a
    ///        small UTF-8 JSON blob per record. Same template as v15.
    /// v32 — Flightplan_GetRouteJson. Full Core Route → SDK Route
    ///        marshalled as UTF-8 JSON: RawRoute, Waypoints, Segments,
    ///        ExplicitSegments, OriginalSegments, ParsingErrors, SID/STAR,
    ///        Suggested SID/STAR, runways, HasDirectApplied,
    ///        CurrentDirectWaypoint. JIT-side fills Flightplan.Route
    ///        after the snapshot fetch.
    /// v33 — last mutators / reads:
    ///        - Aircraft_GetDistanceFromOrigin / ToDestination (haversine
    ///          aircraft↔airport in nm; NaN sentinel for unavailable).
    ///        - ControllerData_Enumerate (capacity-hint shape).
    ///        - Airport_DeleteAirport mutator.
    /// v34 — full IPluginManager lifecycle: LoadPlugin (returning id),
    ///        UnloadPlugin, UnloadAllPlugins, GetLoadedPluginsJson. AOT
    ///        HostedClrPluginManager replaces IPluginManager in DI; the
    ///        existing PluginAutoLoader.Run drives discovery from
    ///        UserSettings.AutoLoadPluginPaths (.nrplugin archives extracted
    ///        on AOT side, .dll paths handed through unchanged).
    /// </remarks>
    public const int Version = 34;
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

    /// <summary>
    /// Great-circle distance in nautical miles from the aircraft's current
    /// position to its flight-plan origin airport. Returns <c>double.NaN</c>
    /// if the aircraft / flightplan / origin waypoint is unavailable.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, double> Aircraft_GetDistanceFromOrigin;

    /// <summary>As <see cref="Aircraft_GetDistanceFromOrigin"/> for destination.</summary>
    public delegate* unmanaged[Cdecl]<char*, double> Aircraft_GetDistanceToDestination;

    /// <summary>
    /// Fill <paramref name="output"/> with up to <paramref name="capacity"/>
    /// aircraft snapshots taken from a single lock-free collection snapshot.
    /// Returns:
    ///   ≥ 0  number of snapshots written (snapshot length)
    ///   -1   null output or negative capacity
    ///   -2   collection unbound
    ///   &lt; -2 negated snapshot length when <paramref name="capacity"/> is
    ///         too small — caller retries with a bigger buffer.
    /// </summary>
    public delegate* unmanaged[Cdecl]<AircraftSnapshot*, int, int> Aircraft_Enumerate;

    // --- Controller ---------------------------------------------------

    /// <summary>
    /// Look up a controller by callsign. Returns 1 on hit, 0 on miss.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, ControllerSnapshot*, int> Controller_TryGetByCallsign;

    /// <summary>Enumerate all controllers. Same capacity-hint protocol as <see cref="Aircraft_Enumerate"/>.</summary>
    public delegate* unmanaged[Cdecl]<ControllerSnapshot*, int, int> Controller_Enumerate;

    /// <summary>
    /// Fetch per-controller array fields not in the POD snapshot:
    /// frequencies (int[]) + atisLines (string[]). UTF-8 JSON, capacity-hint
    /// protocol matches <see cref="Airport_GetConfigurationsJson"/>.
    /// Schema: <c>{"freqs":[12345],"atis":["INFO B","WIND 250/10"]}</c>.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, byte*, int, int> Controller_GetExtrasJson;

    // --- Fsd ----------------------------------------------------------

    /// <summary>
    /// Read the live FSD connection state. Returns 1 if connected (fills
    /// the snapshot), 0 if disconnected (output is zeroed).
    /// </summary>
    public delegate* unmanaged[Cdecl]<FsdConnectionSnapshot*, int> Fsd_GetConnection;

    /// <summary>
    /// Fetch ConnectionInfo array fields not in the POD snapshot:
    /// frequencies (int[]) + atisLines (string[]). Same JSON shape as
    /// <see cref="Controller_GetExtrasJson"/>.
    /// </summary>
    public delegate* unmanaged[Cdecl]<byte*, int, int> Fsd_GetExtrasJson;

    // --- Flightplan ---------------------------------------------------

    /// <summary>
    /// Look up a flightplan by callsign. Returns 1 on hit, 0 on miss.
    /// Route + waypoints stay empty in the SDK record produced from this
    /// snapshot — full Route marshalling is deferred.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, FlightplanSnapshot*, int> Flightplan_TryGetByCallsign;

    /// <summary>Enumerate all flightplans. Same capacity-hint protocol as <see cref="Aircraft_Enumerate"/>.</summary>
    public delegate* unmanaged[Cdecl]<FlightplanSnapshot*, int, int> Flightplan_Enumerate;

    /// <summary>
    /// Fetch the parsed Route for a flightplan as UTF-8 JSON. Capacity-hint
    /// protocol matches <see cref="Airport_GetConfigurationsJson"/>. Schema
    /// covers RawRoute, Waypoints, Segments / ExplicitSegments / OriginalSegments,
    /// ParsingErrors, SID/STAR, Suggested SID/STAR, Dep/Arr runways,
    /// HasDirectApplied, CurrentDirectWaypoint, IsAmended.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, byte*, int, int> Flightplan_GetRouteJson;

    // --- Airport ------------------------------------------------------

    /// <summary>Returns 1 if the named runway is an active departure runway, 0 otherwise.</summary>
    public delegate* unmanaged[Cdecl]<char*, char*, int> Airport_IsDepRunwayActive;

    /// <summary>Returns 1 if the named runway is an active arrival runway, 0 otherwise.</summary>
    public delegate* unmanaged[Cdecl]<char*, char*, int> Airport_IsArrRunwayActive;

    /// <summary>
    /// Mutator — set runway status. Type matches PluginSdk.Airport.RunwayType
    /// (Departure=0, Arrival=1). Returns 1 on success, 0 on miss.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, char*, int, int> Airport_SetRunwayStatus;

    /// <summary>Mutator — remove runway status (deactivate).</summary>
    public delegate* unmanaged[Cdecl]<char*, char*, int, int> Airport_RemoveRunwayStatus;

    /// <summary>
    /// Mutator — set airport status. Status matches PluginSdk.Airport.AirportStatus
    /// (Active=0, Observed=1).
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, int, int> Airport_SetAirportStatus;

    /// <summary>Mutator — toggle whether extended runway centerlines render for this airport.</summary>
    public delegate* unmanaged[Cdecl]<char*, int, int> Airport_SetShowCenterlines;

    /// <summary>Mutator — remove an airport entirely. Returns 1 on success, 0 on miss.</summary>
    public delegate* unmanaged[Cdecl]<char*, int> Airport_DeleteAirport;

    /// <summary>
    /// Serialise every <see cref="PluginSdk.Airport.AirportConfig"/> into a
    /// UTF-8 JSON byte stream. Caller provides an output buffer + capacity
    /// (byte count). Returns:
    ///   ≥ 0  bytes written (no null terminator)
    ///   -1   null output or negative capacity
    ///   -2   collection unbound
    ///   &lt; -2 negated required-byte count when capacity is too small
    ///         — caller retries with a bigger buffer.
    /// JSON schema: [{ "icao":"…", "arr":["…"], "dep":["…"], "status":0|1, "centerlines":true|false }].
    /// </summary>
    public delegate* unmanaged[Cdecl]<byte*, int, int> Airport_GetConfigurationsJson;

    // --- Squawk -------------------------------------------------------
    //
    // First sub-API using the JIT→AOT→JIT callback bridge. The plugin owns
    // an ISquawkProvider; the host invokes it via the matching
    // PluginRuntimeTable.Squawk_GenerateSquawk fn-pointer keyed by the
    // ulong callbackId supplied at registration.

    /// <summary>
    /// Register a plugin-owned squawk provider on AOT NeoRadar's
    /// ISquawkManager. Returns 1 on success, 0 if a provider with the same
    /// name is already registered, negative on error.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, int, ulong, int> Squawk_RegisterProvider;

    /// <summary>Unregister by name. Returns 1 on success, 0 on miss.</summary>
    public delegate* unmanaged[Cdecl]<char*, int> Squawk_UnregisterProvider;

    /// <summary>Set the active provider. Returns 1 on success, 0 if not registered.</summary>
    public delegate* unmanaged[Cdecl]<char*, int> Squawk_SetActiveProvider;

    /// <summary>
    /// Serialise the registered provider name list as JSON
    /// (<c>["name1","name2",…]</c>). Same capacity-hint protocol as
    /// <see cref="Airport_GetConfigurationsJson"/>.
    /// </summary>
    public delegate* unmanaged[Cdecl]<byte*, int, int> Squawk_GetAvailableProvidersJson;

    // --- Chat ---------------------------------------------------------
    //
    // Plugin command registration + execute callback. Definition + result
    // are JSON; execute args land as a JSON string array.

    /// <summary>
    /// Register a plugin command. Args: pluginId, pluginName, command name,
    /// command definition JSON (matches PluginSdk.Chat.CommandDefinition),
    /// callback id (for execute callbacks), output buffer for the returned
    /// command id (UTF-16) + capacity. Returns chars written to
    /// <c>outCommandId</c> on success, 0 if the name is reserved/colliding,
    /// negative on error.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, char*, char*, byte*, int, ulong, char*, int, int> Chat_RegisterCommand;

    /// <summary>Unregister by command id. Returns 1 on success, 0 on miss.</summary>
    public delegate* unmanaged[Cdecl]<char*, int> Chat_UnregisterCommand;

    /// <summary>
    /// Push a client-text-message into NeoRadar's chat. Args:
    /// sentFrom, message, hasColour (0/1), R, G, B, isUrgent, useDedicatedChannel.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, char*, int, byte, byte, byte, int, int, int> Chat_SendClientMessage;

    // --- Tag ----------------------------------------------------------
    //
    // Pure registration + push API — no callback inversion. Tag-action and
    // dropdown events flow back through the existing event delivery path,
    // not via fn-pointer callback like Squawk / Chat.

    /// <summary>
    /// Register a tag item. Args: pluginId, item-name, default value, allowed
    /// actions JSON (UTF-8 string array) + length, output tag id (UTF-16) +
    /// capacity. Returns chars written, 0 on collision, negative on error.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, char*, char*, byte*, int, char*, int, int> Tag_RegisterTagItem;

    /// <summary>
    /// Register a tag action. Args: pluginId, action-name, description,
    /// requiresInput (0/1), output action id (UTF-16) + capacity. Returns
    /// chars written, 0 on collision, negative on error.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, char*, char*, int, char*, int, int> Tag_RegisterTagAction;

    /// <summary>
    /// Push a tag value update. Args: tagId, value, callsign, listId,
    /// hasColour (0/1), R, G, B, hasBgColour (0/1), bgR, bgG, bgB,
    /// context-data JSON (UTF-8 object) + length. Returns 1 on success,
    /// 0 if the tag isn't registered.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, char*, char*, char*, int, byte, byte, byte, int, byte, byte, byte, byte*, int, int> Tag_UpdateTagValue;

    /// <summary>Attach a dropdown to an action. Args: pluginId, actionId, dropdownJson + len. Returns 1 on success, 0 on miss.</summary>
    public delegate* unmanaged[Cdecl]<char*, char*, byte*, int, int> Tag_SetActionDropdown;

    /// <summary>Replace an existing dropdown. Same args as SetActionDropdown.</summary>
    public delegate* unmanaged[Cdecl]<char*, char*, byte*, int, int> Tag_UpdateActionDropdown;

    /// <summary>Detach the dropdown. Returns 1 on success, 0 on miss.</summary>
    public delegate* unmanaged[Cdecl]<char*, char*, int> Tag_RemoveActionDropdown;

    /// <summary>
    /// Read back an action's dropdown definition as JSON. Same capacity-hint
    /// protocol as <see cref="Airport_GetConfigurationsJson"/>. Returns 0
    /// when the action has no dropdown.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, byte*, int, int> Tag_GetDropdownForActionJson;

    // --- Window -------------------------------------------------------
    //
    // Plugin's WindowDefinition is serialised as UTF-8 JSON into a Core
    // PluginWindowCommand and enqueued on IPluginWindowQueue. Avalonia's
    // PluginWindowManager picks it up and materialises the actual window.
    //
    // OpenWindow returns the assigned window handle as a ulong (0 = failure).

    public delegate* unmanaged[Cdecl]<char*, byte*, int, ulong> Window_OpenWindow;
    public delegate* unmanaged[Cdecl]<char*, ulong, char*, char*, int> Window_UpdateElement;
    public delegate* unmanaged[Cdecl]<char*, ulong, int> Window_CloseWindow;

    // --- Rendering (v26 screen lifecycle) -----------------------------

    /// <summary>Args: pluginId, definitionJson + len, callbackId. Returns screen handle (ulong; 0 on failure).</summary>
    public delegate* unmanaged[Cdecl]<char*, byte*, int, ulong, ulong> Rendering_RegisterScreen;

    public delegate* unmanaged[Cdecl]<ulong, int> Rendering_UnregisterScreen;
    public delegate* unmanaged[Cdecl]<ulong, int, int> Rendering_SetScreenEnabled;
    public delegate* unmanaged[Cdecl]<ulong, int> Rendering_IsScreenEnabled;

    // --- IPluginGraphics screen-space (v26) ---------------------------
    //
    // Each Add* returns the GraphicsObjectRef.ObjectId (ulong; 0 on failure).
    // The plugin pairs it with the screen handle to reconstruct the SDK ref.
    // RGBA is passed as four floats — RgbaColor is already float-shaped on
    // the SDK side, so no byte/float conversion at the boundary.

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

    // --- v27 world-space adds -----------------------------------------
    //
    // Vector2 arrays flattened as float* (X,Y pairs). For polyline/polygon
    // shapes the outer level is signalled by a separate `int* lengths` +
    // `int outerCount` so the host can rebuild the jagged structure.

    public delegate* unmanaged[Cdecl]<ulong, float*, int, float, float, float, float, float, ulong> Graphics_AddWorldLine;
    public delegate* unmanaged[Cdecl]<ulong, float*, int*, int, float, float, float, float, float, ulong> Graphics_AddWorldPolyline;
    public delegate* unmanaged[Cdecl]<ulong, float*, int*, int, float, float, float, float, ulong> Graphics_AddWorldPolygon;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, float, float, ulong> Graphics_AddWorldPoint;
    public delegate* unmanaged[Cdecl]<ulong, float, float, char*, float, float, float, float, float, ulong> Graphics_AddWorldText;

    // --- v27 mutate ---------------------------------------------------

    public delegate* unmanaged[Cdecl]<ulong, ulong, float, float, float, float, float, float, float, int> Graphics_SetTransform;
    public delegate* unmanaged[Cdecl]<ulong, ulong, float, int> Graphics_SetMinZoom;
    public delegate* unmanaged[Cdecl]<ulong, ulong, int, int> Graphics_SetHitTestVisible;
    public delegate* unmanaged[Cdecl]<ulong, ulong, float, float, int> Graphics_SetScreenAnchor;
    public delegate* unmanaged[Cdecl]<ulong, ulong, float*, int, int> Graphics_SetScreenPoints;
    public delegate* unmanaged[Cdecl]<ulong, ulong, float, float, int> Graphics_SetScreenSize;
    public delegate* unmanaged[Cdecl]<ulong, ulong, int> Graphics_BringToFront;
    public delegate* unmanaged[Cdecl]<ulong, ulong, int> Graphics_SendToBack;

    // --- v27 pointer capture ------------------------------------------

    public delegate* unmanaged[Cdecl]<ulong, void> Graphics_CapturePointer;
    public delegate* unmanaged[Cdecl]<ulong, void> Graphics_ReleasePointer;
    public delegate* unmanaged[Cdecl]<ulong, int> Graphics_HasCapture;

    // --- v27 coord helpers --------------------------------------------

    public delegate* unmanaged[Cdecl]<ulong, double, double, float*, int> Graphics_LatLonToWorld;
    public delegate* unmanaged[Cdecl]<ulong, float, float, double*, int> Graphics_WorldToLatLon;
    public delegate* unmanaged[Cdecl]<ulong, float, float, float*, int> Graphics_ScreenToWorld;
    public delegate* unmanaged[Cdecl]<ulong, float, float, float*, int> Graphics_WorldToScreen;

    /// <summary>
    /// Fill <paramref name="output"/> with the screen's viewport snapshot
    /// (6 doubles: CenterX, CenterY, Zoom, WidthPixels, HeightPixels,
    /// PixelsPerMeter). Returns 1 on success, 0 if the screen has no
    /// viewport bound (screen-only scope).
    /// </summary>
    public delegate* unmanaged[Cdecl]<ulong, double*, int> Graphics_GetViewport;

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

    /// <summary>Enumerate every controller-data row. Same capacity-hint shape.</summary>
    public delegate* unmanaged[Cdecl]<ControllerDataSnapshot*, int, int> ControllerData_Enumerate;

    // --- Logger -------------------------------------------------------

    /// <summary>
    /// Forward a log line from plugin code into NeoRadar's Serilog pipeline.
    /// <paramref name="level"/> matches <see cref="PluginSdk.Logger.LogLevel"/>;
    /// <paramref name="message"/> is a null-terminated UTF-16 string.
    /// </summary>
    public delegate* unmanaged[Cdecl]<int, char*, void> Logger_Log;

    // --- Package ------------------------------------------------------

    /// <summary>
    /// Notify AOT NeoRadar that a plugin has been disabled by the soft-sandbox.
    /// Args: pluginName (UTF-16, null-terminated), reason (UTF-16). AOT-side
    /// posts a Feedback toast through IFeedbackStore.SetFeedback.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, char*, void> NotifyPluginDisabled;

    /// <summary>
    /// Read the active package path into a caller-provided UTF-16 buffer.
    /// <paramref name="capacity"/> is the buffer's element count (must include
    /// space for the null terminator). Return values:
    ///   &gt; 0  number of chars written (excluding null terminator)
    ///   = 0  no package loaded; buffer left untouched
    ///   &lt; 0 error (-1 null args, -2 store unbound, -3 truncation).
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, int, int> Package_GetPath;

    // Future methods append below in version-stable order. Bumping the
    // major version reorders or removes members.
}

/// <summary>
/// AOT NeoRadar → JIT PluginRuntime function-pointer surface. The desktop's
/// HostedClrPluginManager invokes LoadPlugin / UnloadPlugin / etc. through
/// this table.
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
    /// Real plugin load entry. Args: dllPath (UTF-16), output id buffer
    /// (UTF-16) + capacity. Returns:
    ///   &gt; 0  chars written to outId (plugin loaded)
    ///   = 0  id buffer too small (load attempted, caller retries)
    ///   &lt; 0 PluginLoader status code (file missing / bad assembly / etc.)
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, char*, int, int> LoadPlugin;

    /// <summary>Unload a plugin by id. Returns 1 on success, 0 on miss.</summary>
    public delegate* unmanaged[Cdecl]<char*, int> UnloadPlugin;

    /// <summary>Unload every loaded plugin. Returns the count unloaded.</summary>
    public delegate* unmanaged[Cdecl]<int> UnloadAllPlugins;

    /// <summary>
    /// Serialise every loaded plugin's (id, path, manifest) tuple as JSON.
    /// Schema: <c>[{"id":"…","path":"…","name":"…","version":"…","author":"…","sdkMajor":N,"description":"…"}]</c>.
    /// Capacity-hint protocol matches the rest of the JSON-blob calls.
    /// </summary>
    public delegate* unmanaged[Cdecl]<byte*, int, int> GetLoadedPluginsJson;

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

    /// <summary>AirportAdded — ICAO only.</summary>
    public delegate* unmanaged[Cdecl]<char*, void> OnAirportAdded;

    /// <summary>AirportRemoved — ICAO only.</summary>
    public delegate* unmanaged[Cdecl]<char*, void> OnAirportRemoved;

    /// <summary>AirportStatusChanged — ICAO + status byte.</summary>
    public delegate* unmanaged[Cdecl]<char*, int, void> OnAirportStatusChanged;

    /// <summary>RunwayStatusChanged — ICAO + runway + active flag + isArrival flag.</summary>
    public delegate* unmanaged[Cdecl]<char*, char*, int, int, void> OnRunwayStatusChanged;

    /// <summary>AirportConfigurationsUpdated — total count.</summary>
    public delegate* unmanaged[Cdecl]<int, void> OnAirportConfigurationsUpdated;

    /// <summary>
    /// AOT-side <c>HostedPluginSquawkProvider.GenerateSquawk</c> marshals
    /// through this slot. Parameters: callback id (set at register time),
    /// callsign (UTF-16, null-terminated), aircraft snapshot pointer, hasFp
    /// flag, flightplan snapshot pointer (only valid when hasFp != 0),
    /// output buffer (UTF-16) + capacity. Returns the count of chars
    /// written, 0 if the provider returned empty, negative on error.
    /// </summary>
    public delegate* unmanaged[Cdecl]<ulong, char*, AircraftSnapshot*, int, FlightplanSnapshot*, char*, int, int> Squawk_GenerateSquawk;

    /// <summary>
    /// AOT-side <c>HostedPluginCommandProvider.Execute</c> marshals through
    /// this slot. Args: callback id, command id (UTF-16, null-terminated),
    /// args JSON (UTF-8 string array) + byte count, output result JSON
    /// (UTF-8 <c>{"success":bool,"message":"…"}</c>) + capacity. Returns
    /// bytes written; -(required + 100) on insufficient capacity; -1 / -2
    /// for null args / unknown callback.
    /// </summary>
    public delegate* unmanaged[Cdecl]<ulong, char*, byte*, int, byte*, int, int> Chat_ExecuteCommand;

    // --- v19 events ----------------------------------------------------

    /// <summary>SquawkAssigned — callsign + squawk + providerName.</summary>
    public delegate* unmanaged[Cdecl]<char*, char*, char*, void> OnSquawkAssigned;

    /// <summary>FsdError — message.</summary>
    public delegate* unmanaged[Cdecl]<char*, void> OnFsdError;

    /// <summary>FsdConnectionModelUpdated — callsign.</summary>
    public delegate* unmanaged[Cdecl]<char*, void> OnFsdConnectionModelUpdated;

    /// <summary>AtcPositionUpdate — callsign + lat/lon.</summary>
    public delegate* unmanaged[Cdecl]<char*, double, double, void> OnAtcPositionUpdate;

    /// <summary>AtisLinesUpdate — callsign + line count.</summary>
    public delegate* unmanaged[Cdecl]<char*, int, void> OnAtisLinesUpdate;

    /// <summary>CapabilitiesUpdate — callsign.</summary>
    public delegate* unmanaged[Cdecl]<char*, void> OnCapabilitiesUpdate;

    /// <summary>FlightplanRouteChanged — callsign + raw route string.</summary>
    public delegate* unmanaged[Cdecl]<char*, char*, void> OnFlightplanRouteChanged;

    /// <summary>FlightplanVoiceTypeChanged — callsign + oldVoice + newVoice (enum bytes).</summary>
    public delegate* unmanaged[Cdecl]<char*, byte, byte, void> OnFlightplanVoiceTypeChanged;

    // --- v20 ControllerData granular events ---------------------------

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

    // --- v21 batch + text-message events ------------------------------

    /// <summary>Per-batch position update — array pointer + count.</summary>
    public delegate* unmanaged[Cdecl]<AircraftSnapshot*, int, void> OnPositionUpdate;

    /// <summary>ATIS-info message received — sender callsign, icao, atisLetter.</summary>
    public delegate* unmanaged[Cdecl]<char*, char*, char*, void> OnATISInfoMessageReceived;

    /// <summary>Flightplan-message received — callsign, requestType (int), request body.</summary>
    public delegate* unmanaged[Cdecl]<char*, int, char*, void> OnFlightplanMessageReceived;

    /// <summary>
    /// Window event — pluginId, windowHandle, elementId, eventType, value.
    /// AOT side routes window UI events through this slot via the
    /// IPluginEventSink replacement.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, ulong, char*, char*, char*, void> OnWindowEvent;

    /// <summary>
    /// Tag action invoked — actionId, tagId, callsign, button, hasUserInput,
    /// userInput, dataJson (UTF-8), dataJsonLen.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, char*, char*, int, int, char*, byte*, int, void> OnTagAction;

    /// <summary>
    /// Tag dropdown action invoked — actionId, componentId, tagId, callsign,
    /// hasUserInput, userInput, dataJson (UTF-8), dataJsonLen.
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, char*, char*, char*, int, char*, byte*, int, void> OnTagDropdownAction;

    /// <summary>
    /// Per-frame callback for a registered plugin screen. Args:
    /// callbackId, screenHandle, deltaTimeSeconds, frameNumber,
    /// viewportCenterX, viewportCenterY, zoom, pixelsPerMeter,
    /// widthPixels, heightPixels.
    /// </summary>
    public delegate* unmanaged[Cdecl]<ulong, ulong, double, ulong, double, double, double, double, double, double, void> Render_OnFrameUpdate;

    // --- v28 per-screen pointer / key / enable callbacks ----------------
    //
    // Pointer args: screenX, screenY, hasWorld, worldX, worldY, button (int),
    // modifiers (int), clickCount, hasHit, hitObjectId.
    // For event-consume callbacks (Pressed/Wheel/KeyDown/KeyUp) the last
    // parameter is `int* consumed` — the JIT entry writes 0/1 back to it.

    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, int, int, int, int, ulong, int*, void> Render_OnPointerPressed;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, int, int, int, int, ulong, void> Render_OnPointerMoved;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, int, int, int, int, ulong, void> Render_OnPointerReleased;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, float, float, int, int, ulong, int*, void> Render_OnPointerWheel;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, int, int, int, int, ulong, void> Render_OnPointerEntered;
    public delegate* unmanaged[Cdecl]<ulong, float, float, int, float, float, int, int, int, int, ulong, void> Render_OnPointerExited;
    public delegate* unmanaged[Cdecl]<ulong, int, int, char*, int*, void> Render_OnKeyDown;
    public delegate* unmanaged[Cdecl]<ulong, int, int, char*, int*, void> Render_OnKeyUp;
    public delegate* unmanaged[Cdecl]<ulong, void> Render_OnEnabled;
    public delegate* unmanaged[Cdecl]<ulong, void> Render_OnDisabled;

    // Broadcast render events — routed through HostedPluginEventSink.
    public delegate* unmanaged[Cdecl]<ulong, double, double, double, double, int, void> OnRenderObjectClicked;
    public delegate* unmanaged[Cdecl]<double, double, double, double, double, double, void> OnViewportChanged;

    // --- v29 final 4 events --------------------------------------------

    /// <summary>OnIsControllerATC — callsign + isATC.</summary>
    public delegate* unmanaged[Cdecl]<char*, int, void> OnIsControllerATC;

    /// <summary>OnFsdConnectionStateChange — state (PluginSdk.Fsd.ConnectionState int).</summary>
    public delegate* unmanaged[Cdecl]<int, void> OnFsdConnectionStateChange;

    /// <summary>
    /// OnTagShowDropdown sync-RPC. AOT calls when Core's TagDropdownCoordinator
    /// is about to show a dropdown; JIT side queries every loaded plugin's
    /// OnTagShowDropdown override; writes 0 to <paramref name="allowOut"/>
    /// if any returns false. Caller treats unwritten output as 1 (allow).
    /// </summary>
    public delegate* unmanaged[Cdecl]<char*, char*, int*, void> OnTagShowDropdown;

    /// <summary>
    /// OnRenderObjectHovered — analogous to OnRenderObjectClicked.
    /// Args: handle, worldX, worldY, screenX, screenY (no button).
    /// </summary>
    public delegate* unmanaged[Cdecl]<ulong, double, double, double, double, void> OnRenderObjectHovered;
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
