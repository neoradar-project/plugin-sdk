using System.Collections.Generic;
using System.Linq;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>The guide's pages in nav order: what each says, what each demo runs, and what to read next.</summary>
internal static class ShowcaseGuide
{
    public const string Start = "start";
    public const string Boundary = "boundary";
    public const string Threads = "threads";
    public const string FeatureHost = "features";
    public const string FlightPlans = "flightplans";
    public const string ControllerData = "controllerdata";
    public const string Network = "network";
    public const string Commands = "commands";
    public const string Airports = "airports";
    public const string Tags = "tags";
    public const string Expressions = "expressions";
    public const string Settings = "settings";
    public const string Alerts = "alerts";
    public const string Squawk = "squawk";
    public const string Screens = "screens";
    public const string Performance = "performance";
    public const string RateLimits = "ratelimits";
    public const string Events = "events";

    public static IReadOnlyList<ShowcaseNavGroup> Groups { get; } =
    [
        new("Start",
        [
            new(Start, "Start", "The sample plugin's front page, and how the client loaded it.",
                [
                    new("This window",
                        [
                            "This window is the sample plugin's front page: each page in the list on the left explains one part of a plugin and runs a live demo of it.",
                            "Nothing here sends anything to the network until you tick Demo writes at the foot of the window. It is never remembered.",
                        ],
                        []),
                    new("How the plugin loaded",
                        ["The client read this plugin's .dll into memory and loaded it from those bytes, so the file is never locked. It loaded only because the plugin was built against an SDK with the client's major and minor version and a patch no newer than the client's."],
                        [new("ShowPluginInfo", "Show what the host told the plugin")]),
                ],
                "How plugins load"),
            new(Boundary, "Boundary", "What one call across the plugin boundary costs.",
                [
                    new("Calls across the boundary",
                        [
                            "The plugin runs in a .NET runtime the client hosts for plugins, apart from the client's own code. Every ICoreApi call and every event crosses between the two through a table of function pointers.",
                            "So a call is not free: prefer one GetAll to many GetByCallsign calls, and keep what you read.",
                        ],
                        [new("TimeOneHostCall", "Time one Aircraft.GetAll call")]),
                ],
                "SDK reference"),
            new(Threads, "Threads", "Which thread each event and callback arrives on.",
                [
                    new("Where events arrive",
                        [
                            "Events arrive on client threads (the network reader, the position-batch timer and others). OnFrameUpdate and pointer callbacks run on the render thread; OnWindowEvent, key callbacks and OnSelectedAircraftChanged on the UI thread.",
                            "Nothing is marshalled for you. A handler that takes 250 ms or more is a slow strike, so slow work goes to a thread of your own; this plugin's Scheduling runs it on the thread pool.",
                        ],
                        [new("ShowThreads", "Show this click's thread and a pool job's")]),
                    new("State the render thread reads",
                        ["State the render thread reads is either guarded by one lock or handed over as one value: the status badge's aircraft count is a volatile field set on a client thread."],
                        []),
                ],
                "Events and lifecycle"),
            new(FeatureHost, "Feature host", "How the sample plugin is split into features.",
                [
                    new("One folder per feature",
                        [
                            "SamplePlugin.cs only forwards each BasePlugin event to FeatureHost. Each feature is a folder, added by one line in FeatureRegistration, and hears only the events of the areas it opts into.",
                            "Features never reach into each other: they talk through a feature's own interface or a typed event on the plugin's bus.",
                        ],
                        [new("ListFeatures", "List the registered features")]),
                    new("Faults",
                        ["FeatureHost runs every call inside a try and catch: an exception that reached the client would be a fault, and three in 30 seconds disable the whole plugin."],
                        []),
                ],
                "Events and lifecycle"),
        ]),
        new("Traffic",
        [
            new(FlightPlans, "Flight plans", "The selected aircraft's filed route, as the server parsed it.",
                [
                    new("Route inspector",
                        [
                            "The route inspector explains the selected aircraft's filed route: each leg with its point's real kind and navaid frequency, the direct-to, parsing errors, the voice type and the remarks whole.",
                            "The NeoRadar server parses routes; without it the legs and errors stay empty. While a direct-to is applied the legs start at the direct point.",
                        ],
                        [new("InspectRoute", "Inspect the selected aircraft")]),
                ],
                "SDK reference"),
            new(ControllerData, "Controller data", "Writing the selected aircraft's controller data, as the controller would.",
                [
                    new("Scratchpad and cleared level",
                        ["These write the selected aircraft's controller data, as the controller would. Each write is rate limited, first to one a second for the same aircraft, so the scratchpad burst sends one and the host refuses the other six."],
                        [
                            new("SetScratchpad", "Set scratchpad SAMPLE", "a scratchpad update to FSD"),
                            new("SetClearedLevel", "Clear to FL120", "a cleared level to FSD"),
                            new("BurstScratchpad", "Send it 7 scratchpads at once", "one scratchpad update to FSD; the host refuses the other six for the same aircraft"),
                        ]),
                    new("Tracking and handoffs",
                        ["Accept and reject must name the controller who made the offer; naming SAMPLE_CTR, who made none, is refused and sends nothing. A Scratchpad tag action does the same with typed text."],
                        [
                            new("Assume", "Assume", "a tracking request to FSD"),
                            new("Release", "Release", "a release to FSD"),
                            new("AcceptHandoff", "Accept a handoff from SAMPLE_CTR", "nothing, as no offer is pending"),
                            new("RejectHandoff", "Reject a handoff from SAMPLE_CTR", "nothing, as no offer is pending"),
                            new("CancelHandoff", "Cancel a handoff to SAMPLE_CTR", "a handoff cancel to FSD, if one is pending"),
                            new("SetNextController", "Next controller SAMPLE_CTR", "a coordination to FSD"),
                        ]),
                ],
                "SDK reference"),
            new(Airports, "Airports", "Your configured airports, their runways, and the navdata behind them.",
                [
                    new("Airports and runways",
                        ["The airports you have configured, by their current ICAO, with the runways the navdata knows. An elevation the navdata does not know is NaN and reads unknown."],
                        [new("RefreshRunways", "Refresh")]),
                    new("Runway and airport changes",
                        ["The centreline switch changes only this client's display. Runway and airport changes act on the first airport listed."],
                        [
                            new("ToggleCentrelines", "Toggle its centrelines"),
                            new("ActivateRunway", "Activate a departure runway", "a runway change the server hears"),
                            new("DeactivateRunway", "Deactivate it", "a runway change the server hears"),
                            new("BatchRunways", "Toggle an arrival runway in one batch", "a runway change the server hears"),
                            new("ToggleObserved", "Toggle active and observed", "an airport status change the server hears"),
                            new("DeleteAirport", "Remove the airport", "an airport removal the server hears"),
                        ]),
                    new("Navaid lookup",
                        ["Type a navaid's ident and press Enter to read it from the navdata."],
                        [])
                    {
                        Input = new PageInput("Navaid ident, then Enter", "LookUpNavaid"),
                    },
                ],
                "SDK reference"),
            new(Squawk, "Squawk", "A squawk provider the client asks for codes.",
                [
                    new("Squawk provider",
                        [
                            "A squawk provider is asked for a code when the client assigns one. Unless the controller chose a provider, the client asks the highest priority first, and this one's priority 1 ranks above the client's own at 0.",
                            "So once registered, the next code the client assigns comes from it, from 4601 to 4677, and goes to FSD: Register needs Demo writes. Its token is disposed on Unregister and at shutdown.",
                        ],
                        [
                            new("RegisterSquawk", "Register", "the codes the client next assigns, from 4601 to 4677, as this provider ranks above the client's own"),
                            new("UnregisterSquawk", "Unregister"),
                            new("ListSquawk", "List providers"),
                            new("ActivateSquawk", "Make it the active provider", "codes from 4601 to 4677 when squawks are assigned"),
                        ]),
                ],
                "Squawk providers"),
        ]),
        new("Screens",
        [
            new(Tags, "Tags and menus", "Tag items, tag actions and a menu entry of the plugin's own.",
                [
                    new("Tag items and actions",
                        [
                            "The plugin registers tag items NOTE and STATUS, an input action Note and a dropdown action Menu. A package binds them by id; the Package folder beside this plugin holds an example.",
                            "An empty pushed value shows the item's default. The dropdown's toggle and radio are ticked from the plugin's own state, so it sets the dropdown again each time it opens.",
                        ],
                        [
                            new("PushStatus", "Push SET to STATUS"),
                            new("PushEmptyStatus", "Push an empty STATUS"),
                        ]),
                    new("Menu entry",
                        ["The plugin also adds an entry, Reopen the Showcase, to the Insets & Plugin Windows menu; a click arrives as OnMenuItemClicked."],
                        []),
                ],
                "Tag items and expressions"),
            new(Expressions, "Expressions", "Values a package's conditions read from the plugin.",
                [
                    new("Session and aircraft expressions",
                        [
                            "Quiet Hours is a session expression: one value for the whole client, which a map item's showWhen can read. Flagged is an aircraft expression a tag colour or list filter can read.",
                            "The client keeps the last value pushed and never calls the plugin to evaluate a condition.",
                        ],
                        [
                            new("QuietHoursOn", "Quiet hours on"),
                            new("QuietHoursOff", "Quiet hours off"),
                            new("FlagSelected", "Flag or unflag the selected aircraft"),
                        ]),
                ],
                "Tag items and expressions"),
            new(Screens, "Screens and windows", "What the plugin draws, and the windows it opens.",
                [
                    new("Screens and windows",
                        [
                            "A screen is drawn: a main-radar overlay such as the status badge, or an inset such as AMAN. A window is a form described as elements, such as this one.",
                            "A screen turned off keeps its objects, so turning it on shows them again with nothing rebuilt.",
                            "A hit-testable object on a screen above a client tool takes the press, as the status badge does above the heading tool. The warning zone is decorative: its fill is set SetHitTestVisible(false), so a press inside it reaches the targets, the map and the tools; a screen ZOrder below the tool's does the same for a whole screen.",
                        ],
                        [
                            new("ToggleStatusBadge", "Status badge off or on"),
                            new("ToggleAman", "AMAN inset off or on"),
                            new("OpenForm", "Open the form window"),
                        ]),
                    new("Theme colours",
                        ["The status badge, warning zone and Mini Map take their colours from the client's theme. A theme switch raises no event, so the palette is read once a second and a frame recolours only when it changed."],
                        []),
                ],
                "Rendering and screens"),
            new(Alerts, "Sound and focus", "Toasts, sounds and the controller's selection.",
                [
                    new("Toasts and sounds",
                        ["Toasts are for what the controller may miss; one shows at a time. A .wav plays everywhere; an .mp3 plays on macOS and Linux and is refused on Windows."],
                        [
                            new("ToastInfo", "Info toast"),
                            new("ToastWarning", "Warning toast"),
                            new("ToastError", "Error toast"),
                            new("PlayWav", "Play the .wav"),
                            new("PlayMp3", "Play an .mp3"),
                        ]),
                    new("Focus",
                        ["The plugin plays its sound when a handoff is offered to you, and can follow the selected aircraft on the Mini Map. Selecting an aircraft moves your visible selection."],
                        [
                            new("FollowOn", "Follow the selection"),
                            new("FollowOff", "Stop following"),
                            new("SelectNext", "Select the next aircraft"),
                        ]),
                ],
                "SDK reference"),
            new(Performance, "Render performance", "Drawing within the render thread's budget.",
                [
                    new("The frame budget",
                        ["OnFrameUpdate runs on the one render thread with a 16 ms budget. Add objects once, keep their refs, and change them only when the data changed."],
                        []),
                    new("The AMAN pattern",
                        ["The AMAN renderer shows the pattern: a dirty flag, static chrome apart from content, objects pooled by callsign, a signature to skip unchanged frames, and no allocation per frame."],
                        []),
                ],
                "Rendering and screens")
            {
                HasReadout = false,
            },
        ]),
        new("Network",
        [
            new(Network, "Network and chat", "Who is online, and chat channels of the plugin's own.",
                [
                    new("Who is online",
                        ["Who is online, your own FSD connection, and what the network just did, refreshed every five seconds by a scheduled job."],
                        [new("RefreshStations", "Refresh")]),
                    new("Chat channels",
                        ["A plugin posts only to this client's chat, never to the network. It may keep up to 8 channels of its own; a built-in name such as SYSTEM is refused."],
                        [
                            new("PostOnSample", "Post on [Sample]"),
                            new("PostOnSampleTwo", "Post on [Sample Two]"),
                            new("PostOnSystem", "Post on [SYSTEM], refused"),
                        ]),
                ],
                "Chat"),
            new(Commands, "Commands", "Chat commands the plugin declares.",
                [
                    new("Chat commands",
                        [
                            "Type these in the chat input. .sampleSwitch takes on or off in any case and receives it as declared, so .sampleSwitch ON reaches the plugin as on, and .sampleSwitch maybe never reaches it.",
                            ".sampleCallsign takes a value the host matches against [A-Z]{3}[0-9]+[A-Z]? before the plugin sees it.",
                        ],
                        []),
                ],
                "Writing a command"),
            new(RateLimits, "Rate limits", "How the host throttles calls that act for the controller.",
                [
                    new("Throttled calls",
                        [
                            "Calls that act for the controller are rate limited per operation: a burst of five, then about one a second as it refills. A call that names an aircraft is first limited to one a second for that aircraft. A throttled call returns false and logs a warning; it never disables the plugin.",
                            "The chat burst posts 7 lines at once to this client's [Sample] channel, a call that names no aircraft: from a full burst five are posted and two refused, and a second later one more can be. A throttled toast is dropped without telling the plugin; the scratchpad burst on Controller data shows the per-aircraft limit.",
                        ],
                        [
                            new("ChatBurst", "Post 7 chat lines at once"),
                            new("ToastBurst", "Show 7 toasts at once"),
                        ]),
                ],
                "SDK reference"),
        ]),
        new("Settings and events",
        [
            new(Settings, "Settings", "The plugin's settings, in its two scopes.",
                [
                    new("Plugin and profile scopes",
                        [
                            "Settings are strings. The plugin scope follows the controller everywhere; the profile scope is kept per profile and is read again on OnActiveProfileChanged.",
                            "A key never set reads null, not empty. A write made before the saved settings have loaded waits briefly, and is refused only if they could not be read.",
                        ],
                        [
                            new("WriteGlobalSetting", "Write the plugin-scope value"),
                            new("WriteProfileSetting", "Write the profile value"),
                            new("ReadUnsetSetting", "Read a key never set"),
                            new("ShowSettings", "Show both scopes"),
                        ]),
                ],
                "SDK reference"),
            new(Events, "Event log", "Every BasePlugin event, counted as it arrives.",
                [
                    new("Event counts",
                        ["Every one of BasePlugin's 53 events, counted as it arrives, with the thread it came on. Position batches, controller data updates and the timer are counted but not listed."],
                        [new("LogEachLevel", "Write one log line at each level")]),
                ],
                "Logging and diagnostics")
            {
                HasCountsTable = true,
            },
        ]),
    ];

    public static IReadOnlyList<ShowcasePage> Pages { get; } = Groups.SelectMany(g => g.Pages).ToArray();

    /// <summary>Finds a page by id, or null for an id no page has.</summary>
    public static ShowcasePage? PageOrNull(string id)
    {
        foreach (ShowcasePage page in Pages)
            if (page.Id == id) return page;
        return null;
    }
}
