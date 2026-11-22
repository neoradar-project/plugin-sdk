# How a plugin should be laid out

A plugin with one feature can be one class. Past that, copy the Showcase sample's shape
(`samples/NeoRadar.SamplePlugin`). Read its `Common/` folder and two features in full before you
start: `RunwayBoard/` (a feature with actions and a formatter) and `WhoIsOnline/` (a feature with
several processors and a scheduled job).

## The folders

```text
MyPlugin/
  MyPlugin.cs              the BasePlugin class: builds the host, forwards every event
  AssemblyInfo.cs          InternalsVisibleTo for the test project
  Common/                  composition and shared parts, no feature logic
    Composition.cs         registers ICoreApi, each sub-API, HostSurfaces, the bus, the scheduler
    FeatureRegistration.cs one AddFeature<T>() line per feature
    FeatureHost.cs         runs every call into a feature inside a try and catch
    IFeature.cs            Initialize and Shutdown
    HostSurfaces.cs        the rendering, window and tag interfaces, resolved once
    Events/                one handler interface per SDK area, and the typed event bus
    Scheduling/            the timed task scheduler
  Stands/                  one folder per feature
    StandsFeature.cs       wiring only
    StandActions.cs        changes sent out
    FlightplanProcessor.cs inbound events for one concern
    StandFormatter.cs      pure text building
    IStands.cs             what other parts (a window, demos) may call
    Events/StandAllocated.cs
  Package/                 an example package fragment that binds the plugin's ids
```

Tests mirror it: `tests/MyPlugin.Tests/Stands/StandActionsTests.cs`, `tests/MyPlugin.Tests/Fakes/`.

## The parts of `Common/`

| Part | Sample file | Job |
|---|---|---|
| Composition | `Common/Composition.cs` | Registers `ICoreApi`, `ClientInformation` and each of the sub-APIs as singletons, then the shared parts, then the features. A feature asks only for the sub-APIs it uses. |
| Host surfaces | `Common/HostSurfaces.cs` | `Rendering.GetInterface()`, `Window.GetInterface()` and `Tag.GetInterface()` called once. |
| Feature registration | `Common/FeatureRegistration.cs` | `AddFeature<T>()` registers a feature once (twice throws), as `IFeature`, and under every per-area handler interface it implements and every interface in its own namespace. The list order is the `Initialize` order; `Shutdown` runs in reverse. |
| Feature host | `Common/FeatureHost.cs` | Takes every `IFeature` and every handler list from the container. Each call runs in a try and catch that logs `[Feature.Method]` and carries on, so one feature's bug is never a fault against the whole plugin. |
| Handler interfaces | `Common/Events/I*EventHandler.cs`, `ITimerHandler.cs` | One per SDK area, every method with an empty default body, so a feature implements only the events it needs. |
| Event bus | `Common/Events/PluginEventBus.cs` | `Subscribe<TEvent>` returns an `IDisposable`; `Publish<TEvent>` snapshots the subscribers under a lock, then calls each outside it on the publisher's thread, logging any that throws. One record type per kind of event. |
| Scheduler | `Common/Scheduling/TimedTaskScheduler.cs` | `Every(name, period, work)` and `RunOnce(name, work)`. The host's `OnTimer` calls `Tick()`; due work runs on the thread pool, never overlaps itself, and never starts after `Stop()`. Takes a `TimeProvider`, so tests drive the clock. |
| Gate | `Common/DemoWritesGate.cs` | A `volatile bool`, closed at load, that every network-visible action checks. |

The plugin class does nothing else:

```csharp
public override void Initialize(PluginManifest metadata, ICoreApi coreApi, ClientInformation clientInfo)
{
    try
    {
        ServiceCollection services = new();
        services.AddMyPlugin(coreApi, clientInfo);
        _provider = services.BuildServiceProvider();
        _features = _provider.GetRequiredService<FeatureHost>();
        _features.Initialize();
    }
    catch (Exception ex)
    {
        coreApi.Logger.Error($"Could not compose the plugin: {ex}");
    }
}

public override void Shutdown()
{
    _features?.Shutdown();
    _features = null;
    _provider?.Dispose();
    _provider = null;
}

public override void OnFlightplanUpdated(FlightplanUpdatedEvent evt) => _features?.OnFlightplanUpdated(evt);
```

`OnTagShowDropdown` returns a value, so forward it as `_features?.OnTagShowDropdown(evt) ?? true`.

## A feature

The feature class implements `IFeature`, the handler interfaces it needs and its own `I<Feature>`
interface, and forwards each to the class that does the work:

```csharp
internal sealed class StandsFeature : IFeature, IStands, IFlightplanEventHandler
{
    private readonly ITimedTaskScheduler _scheduler;
    private readonly StandActions _actions;
    private readonly FlightplanProcessor _flightplans;
    private IDisposable? _refreshJob;

    public StandsFeature(HostSurfaces surfaces, IControllerDataApi controllerData, DemoWritesGate gate,
        ITimedTaskScheduler scheduler, IPluginEventBus bus)
    {
        StandTable table = new();
        _scheduler = scheduler;
        _actions = new StandActions(controllerData, gate, bus, table);
        _flightplans = new FlightplanProcessor(surfaces.Tag, table);
    }

    public void Initialize() => _refreshJob = _scheduler.Every("stands-refresh", TimeSpan.FromSeconds(5), _actions.Refresh);

    public void Shutdown()
    {
        _refreshJob?.Dispose();
        _refreshJob = null;
    }

    public void AllocateSelected() => _actions.AllocateSelected();

    // Client threads.
    public void OnFlightplanUpdated(FlightplanUpdatedEvent evt) => _flightplans.HandleFlightplanUpdated(evt);
}
```

`WhoIsOnlineFeature` has exactly this shape with five processors; `RunwayBoardFeature` with one
actions class.

Rules that keep it readable:

- A feature class holds no logic, only fields, `Initialize`, `Shutdown` and one-line forwards.
- Name the thread above a group of handlers in a one-line comment (`// Client threads.`).
- A feature's events go in its own `Events/` folder, one `sealed record` per kind, named in the
  past tense (`RunwayBoardRefreshed`, `StationsListed`).
- A feature reaches another feature through that feature's interface or a bus event, never its class.
- A feature registers its own Showcase demos (or your plugin's equivalent) on the same line as
  `AddFeature`: `.AddFeature<RunwayBoardFeature>().AddShowcaseDemos<RunwayBoardDemos>()`. The
  demos class lists the buttons and subscribes the window to the feature's events; the window
  knows no feature.

## What each suffix means

Each suffix has one meaning. A class whose contents do not match its suffix, or that holds two
roles, is split.

| Suffix | Means only |
|---|---|
| `Processor` | Handles inbound events for one concern (`ControllerActivityProcessor`, `ChatActivityProcessor`). Several concerns are several processors. |
| `Actions` | Changes the controller makes, sent out, with their guards (`RunwayBoardActions`, `WhoIsOnlineActions`). |
| `Manager` | One piece of state under one lock, and the operations on it that reach outside. |
| `Store` | State that readers query, one lock, no IO (`EventCountStore`). |
| `Service` | Talks to something outside the process (an HTTP API, a file). |
| `Helper` | Static pure functions about one thing. Prefer putting each on the type it is about. |
| `Handler` | Handles one kind of input: one click action, one command, one form submission. |
| `Formatter`, `Layout`, `Renderer` | Builds text, builds a window definition, draws a screen. |

A processor that also holds a lock over shared state is a store in disguise: keep the state and
its lock in a store, and let the processor call the store's methods.

Data is a noun, an action is verb first, an event is past tense.
