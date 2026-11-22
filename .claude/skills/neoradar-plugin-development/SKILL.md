---
name: neoradar-plugin-development
description: Build, structure, design, test or debug a NeoRadar plugin against NeoRadar.PluginSdk. Use when someone says "write a plugin", "add a feature to my plugin", "make a tag item / expression / tag action / dropdown / chat command / squawk provider / window / radar overlay / inset", asks how a plugin should be laid out or how to design its window, asks why a plugin will not load, was disabled or shows "Faulted", or asks how to test, publish or version one. Also use when changing the Showcase sample in this repo.
---

# NeoRadar plugin development

A plugin is a .NET 10 class library with one class deriving `BasePlugin`. It references only
`NeoRadar.PluginSdk`, receives one `ICoreApi` with 21 sub-APIs, and hears events as `virtual On...`
methods. Everything it may do is in the SDK; nothing else of the client is reachable.

This skill is what we have learnt building the SDK and its Showcase sample. Each topic has a
reference file; read the one you need before the work it covers.

| Reference | Read it before |
|---|---|
| `reference/docs-and-mcp.md` | anything: how to search the public docs and what each page and Showcase page covers |
| `reference/project-and-loading.md` | creating the project, referencing the SDK, versioning, shipping |
| `reference/plugin-layout.md` | writing a second feature, or any plugin bigger than one class |
| `reference/threads-and-limits.md` | writing a handler, a timer job, a lock or a write to the network |
| `reference/windows.md` | building a window |
| `reference/screens.md` | drawing on the radar or in an inset |
| `reference/package-binding.md` | tag items, expressions, actions, dropdowns, commands, chat, squawk, menus |
| `reference/testing.md` | writing tests or doing a live check |

## 1. Search the docs first

The public docs are the source of truth for behaviour: https://docs.neoradar.app/plugins/overview.
They need a VATSIM account with Client Access. An agent should connect the docs MCP server once:

```bash
claude mcp add --transport http neoradar-docs https://docs.neoradar.app/mcp
```

Then run `/mcp`, choose `neoradar-docs` and sign in. The server has `search_docs`, `read_doc` and
`list_docs`. Search before you guess at a limit, an id format or a thread. Without MCP, download
`/llms-full.txt` or one page as `.md` (details in `reference/docs-and-mcp.md`).

In this repo, `tests/NeoRadar.PluginSdk.Tests/PublicApi/public-api.txt` lists every public member,
and the XML docs on each member (shown by the IDE) state its units, sentinels and threads.

## 2. What a plugin is and how it loads

- A class library targeting `net10.0`, one `BasePlugin` subclass, a `PluginManifest` whose
  `SdkMajorVersion` is `PluginSdkVersion.Major`. `Manifest.Name` is the plugin's identity and part
  of every id it registers; never rename it once a package uses it.
- The SDK reference is compile-only (`Private=false`, `ExcludeAssets=runtime`). A copy of
  `NeoRadar.PluginSdk.dll` beside the plugin makes a second `BasePlugin` type and the load fails.
- Private dependencies ship beside the dll. The sample sets `CopyLocalLockFileAssemblies` so
  `Microsoft.Extensions.DependencyInjection` lands next to it.
- The client reads the dll into memory and loads it from bytes into its own collectible context.
  The file is never locked, and `Assembly.Location` is empty: store files under
  `ClientInformation.DocumentsPath` (the sample uses `<data root>/SamplePlugin`).
- Version gate: the plugin's SDK must have the client's major and minor, and a patch no newer than
  the client's. A minor bump refuses every older build, so rebuild on each one.
- Build with `dotnet build`, never `dotnet publish`.

Detail and the csproj: `reference/project-and-loading.md`.

## 3. How a plugin should be laid out

The Showcase sample (`samples/NeoRadar.SamplePlugin`) is the model. In short:

- One folder per feature at the project root (`RunwayBoard/`, `WhoIsOnline/`, `WarningZone/`).
- `Common/` holds the composition: `Composition` registers the host APIs, `FeatureRegistration`
  has one `AddFeature<T>()` line per feature, `FeatureHost` runs each call into a feature inside a
  try and catch, the typed `PluginEventBus` carries feature-to-feature events, and
  `TimedTaskScheduler` runs periodic work off the host timer with an injected clock.
- The `BasePlugin` class (`SamplePlugin.cs`) builds the host and only forwards events.
- A feature class is wiring only. Logic lives in `*Actions` (changes sent out), `*Processor`
  (inbound events for one concern), `*Formatter` and similar classes.
- Feature events live in the feature's own `Events/` folder, one record per kind.
- Each feature registers its own Showcase demos beside its `AddFeature` line.
- Tests mirror the folders, with hand-rolled fakes of the 21 sub-APIs.

Detail, suffix meanings and a feature skeleton: `reference/plugin-layout.md`.

## 4. Rules of the road

Threads (nothing is marshalled for you):

| Callback | Thread |
|---|---|
| Most `On...` events, chat commands, `OnTagShowDropdown` | client threads (network reader, position batch and others) |
| `OnTimer` | a client thread, each whole UTC second, never re-entered |
| `OnFrameUpdate`, pointer callbacks | the render thread |
| `OnWindowEvent`, key callbacks, `OnSelectedAircraftChanged` | the UI thread |
| `ISquawkProvider.GenerateSquawkAsync` | off the UI thread, never the render thread |

Rules:

1. Touch `IPluginGraphics` only from `OnFrameUpdate` on the render thread. Another thread writes a
   field and calls `RequestRedraw()`, which is safe anywhere.
2. In `OnFrameUpdate`: no allocation, no host call per aircraft, no `GetAll`, no `GetPalette`.
   Return early when nothing changed.
3. A handler taking 250 ms or more is a slow strike; 5 in 60 seconds disable the plugin (none count
   in the first 60 seconds after a load). 3 escaped exceptions in 30 seconds disable it too. Slow
   work goes to the thread pool through a scheduler.
4. Under a lock, copy what you need; publish events, call the host and dispose tokens after leaving it.
5. A timed job never starts after `Shutdown`, and `Shutdown` waits a bounded time for jobs in flight.
6. In `Shutdown`: dispose every subscription, job and squawk token, unregister every screen, close
   every window, clear statics. Anything left running keeps the plugin from unloading.
7. Rate limits, per operation: a burst of 5, then about one a second. `ControllerData` writes are
   also limited to one a second per aircraft, `Focus.SelectAircraft` to one a second per callsign,
   and chat to a burst of 5. A throttled call returns `false` (a toast is dropped silently).

Detail: `reference/threads-and-limits.md`.

## 5. Network etiquette

Nothing reaches FSD or the server unless the controller acted. The sample's pattern is the
"Demo writes" gate: a checkbox in the window footer, unticked at every load and never remembered;
every `*Actions` method that sends checks it first and says so when it refuses, and the window
disables those buttons while it is shut.

Count as network-visible: every `ControllerData` write, airport and runway writes (the server
hears them), `SetActiveProvider`, and registering a squawk provider, because a provider ranked
above the client's own is asked first by priority alone. Reads, chat posts to your own channel,
toasts, sounds and drawing are local.

## 6. Designing UI

Two surfaces, chosen by what you show:

| You want | Use | Read |
|---|---|---|
| A form, settings panel, table, dialog | `ICoreApi.Window` (declarative elements) | `reference/windows.md` |
| Lines, polygons, text over the radar, or a custom inset panel | `ICoreApi.Rendering` (retained screens) | `reference/screens.md` |
| Text in a data tag or list, a click, a dropdown, a condition value | tag items, actions, expressions | `reference/package-binding.md` |

The short version:

- Windows: colour with theme brush keys (`ThemeBg`, `ThemeFg`, `ThemeBorder`) so a theme switch
  follows. The Showcase reproduces the client's Package Settings window: a 220 px nav on
  `Brush.TitleBar`, a page on `Brush.Pkg.Base`, cards on `Brush.Input.Bg` outlined in
  `Brush.Border.Subtle`, and a footer. Open once, then change only the elements that changed.
- Screens: pick the scope, choose a `ZOrder` against the host layers (default 100), make decorative
  objects non-hit-testable, return `false` from `OnPointerPressed` unless you act, keep the scene
  across disable and enable, and follow the theme from a snapshot.

## 7. Testing

- Unit tests drive `*Actions`, `*Processor` and renderers against hand-rolled fakes of the
  sub-APIs, as `tests/NeoRadar.SamplePlugin.Tests/Fakes/` does. No mocking framework.
- A wiring test composes the real service graph and fails when a registration line is deleted
  (`Common/FeatureRegistrationTests.cs`).
- A renderer test draws, turns the screen off and on, draws again and asserts the object count is
  unchanged (`Fakes/ScreenReenable.cs`).
- Live: **Add Plugin** in Plugin Management, then read `logs/client.log` and the plugin's own
  `logs/plugins/<Manifest.Name>.log` in the data root.

Detail: `reference/testing.md`.

## 8. Pitfalls we hit

| Pitfall | What to do |
|---|---|
| A renderer that drops its refs in `OnDisabled` and rebuilds draws everything twice after re-enable | The host keeps the scene while a screen is off; do not override `OnDisabled` to rebuild |
| `IPluginGraphics` called from a client thread | Write a field, call `RequestRedraw()`, draw in `OnFrameUpdate` |
| `Aircraft.GetAll()` or `Theme.GetPalette()` every frame | Read on the event or a timer job; hand the renderer a snapshot |
| A filled world zone swallows presses, so the controller cannot click targets under it | `SetHitTestVisible(ref, false)` on decorative objects |
| A button that writes to the network with no gate | Check a never-remembered gate in the `*Actions` method |
| A burst demo that names one aircraft shows one success, not five | The per-aircraft limit hits first; demonstrate a burst with a call that names no aircraft (a chat post) |
| `Sound.Play` with an `.mp3` returns `false` on Windows | Ship `.wav` |
| A chat channel named `System`, `ATC`, `AFV` or like `118.500` is refused | Use your own name, at most 8 per plugin, 32 characters each |
| A hidden `ShowInList` window cannot be shown again by the plugin | Close it and open it afresh (the Showcase's reopen menu entry) |
| A plugin built on another SDK minor, or a newer patch, than the client ships is refused | Rebuild against the SDK the client ships |
| `RegisterTagItem`, `Declare` or `RegisterCommand` returns an empty string | Log it; the name is empty, taken, reserved or too long |
| A command named `audio`, `dev`, `msg` or another built-in | Prefix your commands with your plugin or vACC name |

## 9. Changing this repo's sample

The Showcase is held to the same rules it teaches. Keep `FeatureRegistration` and
`FeatureRegistrationTests` in step, keep each feature's demos registered next to it, gate every
network-visible demo, and add a test with each behaviour change. Public SDK changes follow the
rollout in this repo's `CLAUDE.md`.
