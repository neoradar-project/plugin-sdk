# Testing a plugin

Model: `tests/NeoRadar.SamplePlugin.Tests/`. Docs for the live check:
https://docs.neoradar.app/plugins/logging-and-diagnostics.

## The test project

- xUnit, referencing the plugin project and the SDK project (a normal reference here; the test
  host has no client to supply the SDK).
- The plugin keeps its types `internal` and grants `InternalsVisibleTo` to the test assembly.
- Folders mirror the plugin: `RunwayBoard/RunwayBoardActionsTests.cs` tests
  `RunwayBoard/RunwayBoardActions.cs`.
- Hand-rolled fakes, no mocking framework. Every behaviour change ships with a test.

## Fakes of the host

`Fakes/` has one fake per sub-API (`FakeAircraftApi`, `FakeChatApi`, `FakeWindowInterface` and
so on) and a `FakeCoreApi` exposing each as a typed property (`core.AirportFake`,
`core.ChatFake`). A fake records what was called and lets the test set what it returns:
`FakeChatApi.Sent` lists posted messages and `AllowedMessages` simulates the rate limit;
`FakeAirportApi.Calls` lists writes as strings. Copy them; they implement the SDK interfaces, so
the compiler tells you when the SDK changes.

Also there: `FakeTimeProvider` and `FakeTimedTaskScheduler` (drive time and jobs by hand),
`InlineTaskScheduler` and `PausingTaskScheduler` (run pool work inline or hold it),
`RecordingGraphics` (an `IPluginGraphics` that records the live objects and which are not hit-testable),
and `ScreenReenable`.

## What to test

| Kind | Example in the sample |
|---|---|
| An action's guard: nothing is sent while the gate is shut | `RunwayBoardActionsTests.AnAirportWrite_BeforeDemoWrites_CallsNothing` |
| An action's call when allowed | `RunwayBoardActionsTests.AnAirportWrite_WithDemoWrites_ActsOnTheFirstAirport` |
| A formatter's text, including the NaN and empty cases | `RunwayBoardFormatterTests` |
| `Shutdown` disposes what `Initialize` scheduled | `WhoIsOnlineFeatureTests.Shutdown_StopsTheRefreshJob_ItsInitializeScheduled` |
| A renderer survives off and on without a second copy | each `*RendererTests.TurnedOffAndOn_KeepsItsObjects_AndDrawsNoSecondCopy` |
| A decorative object lets presses through | `WarningCircleRendererTests.TheZone_IsDecorative_SoAPressOnItReachesTheMapAndItsTools` |
| One feature's exception does not stop the others | `FeatureHostTests.AThrowingFeature_DoesNotStopTheOthers_FromInitialising_OrHearingAnEvent` |
| No timed job runs after `Shutdown` | `FeatureHostTests.Shutdown_StopsTheScheduler_SoNoTimedJobRunsAfterIt`, `TimedTaskSchedulerTests` |
| Every id the package fragment names is registered | `PackageFragmentTests.EveryPluginIdTheFragmentNames_IsOneThePluginRegisters` |

A bug fix starts with a test named for the bug, shown failing, then the fix.

## Wiring tests

Composition is code too. `Common/FeatureRegistrationTests.cs` builds the real service graph with
`FakeCoreApi` and checks that:

- each named feature resolves exactly once as an `IFeature`;
- no feature is registered that the test does not name;
- the registered order is the expected `Initialize` order;
- registering a feature twice throws at composition;
- a handler interface resolves to the same instance as the feature.

The test that matters is one that fails when you delete a registration line. Check it: delete an
`AddFeature<T>()` line, run the tests, see the named failure, put the line back. A wiring test that
still passes with the line gone tests nothing.

## The live check

1. Build (`dotnet build -c Release`).
2. In NeoRadar open **Plugin Settings** from the settings menu, press **Add Plugin** and pick the dll.
3. Read `logs/client.log` in the data root for lines naming your plugin: a load refusal, an id
   refused, a theme key the theme does not define, a watchdog trip.
4. Read your own `logs/plugins/<Manifest.Name>.log`: your start-up line and nothing unexpected.
5. Expand the plugin's row in Plugin Management to see its screens and the exact tag and action ids.
6. Run it for several minutes on live traffic with no watchdog warning.
7. Reload it and remove it. A plugin still reachable after unload is named in `client.log`; find
   what you left running.

`.dev plugin-expressions` lists what the plugin exported to packages, and `.dev gen-squawk` asks
the squawk providers for the selected aircraft.
