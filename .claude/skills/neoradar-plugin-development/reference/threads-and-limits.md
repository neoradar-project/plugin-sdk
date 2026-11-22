# Threads, watchdogs and rate limits

Docs: https://docs.neoradar.app/plugins/events-and-lifecycle,
https://docs.neoradar.app/plugins/how-plugins-load (watchdogs),
https://docs.neoradar.app/plugins/sdk-reference (rate limits).

## Which thread

Plugin code runs on the client's own threads, and nothing is marshalled for you.

| Callback | Thread |
|---|---|
| Aircraft, flight plan, controller, controller-data, airport, FSD, chat, squawk, tag and menu events | client threads: the network reader, the position-batch timer and others |
| `OnPositionUpdate` | a client thread, once per position batch (every 500 ms), carrying every aircraft |
| `OnTimer` | a client thread, each whole UTC second; never re-entered, a beat is skipped while the last runs |
| Chat command `Execute`, `OnTagShowDropdown` | client threads |
| `OnFrameUpdate`, `OnPointerPressed`, `OnPointerMoved`, `OnPointerReleased`, `OnPointerWheel` | the render thread |
| `OnWindowEvent`, `OnKeyDown`, `OnKeyUp`, `OnSelectedAircraftChanged` | the UI thread |
| `ISquawkProvider.GenerateSquawkAsync` | off the UI thread and never on the render thread; 5 seconds, then cancelled |

Calls into `ICoreApi` are allowed from any thread, your own included. `IPluginGraphics` is the
exception: use it only inside `OnFrameUpdate`. `IPluginGraphics.RequestRedraw()` is safe from any
thread and only asks for a frame. After every plugin's `OnTimer` returns, the client asks for a
frame on each view showing a plugin screen, so a clock-driven screen needs no `RequestRedraw`.

## The watchdogs

| Watchdog | Trips when |
|---|---|
| Faults | an exception escapes an event handler, chat command, `OnTagShowDropdown` or `OnFrameUpdate`: 3 within 30 seconds |
| Slow handler | a handler, chat command or `OnTagShowDropdown` takes 250 ms or longer: 5 within 60 seconds; nothing counts in the first 60 seconds after a load |
| Slow frame | `OnFrameUpdate` runs past the 16 ms frame budget: 5 outstanding strikes, each fast frame earning one back |

A tripped plugin is switched off and fully unloaded. The controller sees a warning toast,
"Plugin 'name' disabled", with the reason; Plugin Management shows a **Faulted** badge; the
reason is in `client.log`. A throw from a pointer or key callback is logged and does not count.

A debugger pause counts as wall-clock time, so a long pause on a breakpoint can disable the plugin
once the first 60 seconds are over. Log rather than break where you can.

What keeps a plugin inside the limits:

- Keep handlers short. File parsing, HTTP and heavy computing go to the thread pool. The sample's
  `TimedTaskScheduler` runs every periodic or deferred job on the pool from the host tick, so no
  feature starts a thread of its own.
- Catch your own exceptions. The sample's `FeatureHost` wraps every call into a feature.
- In `OnPositionUpdate`, compare with the last batch and act only on change. Do not make a host
  call per aircraft per batch, and do not log per aircraft.
- In `OnFrameUpdate`, see `screens.md`: no allocation, no host reads, return early when clean.

## Shared state and locks

Events arrive on several threads at once, so state a renderer, a window and a handler share needs care:

- A single value read by the render thread: a `volatile` field (`StatusBadgeRenderer._aircraftCount`).
- A group of values that must change together: an immutable snapshot record swapped whole through
  a `volatile` reference (`Common/ThemeColours.cs` and `ThemeSnapshot`), with a version number the
  renderer compares.
- State under a lock: copy what you need inside the lock, then publish, call the host, log or
  dispose after leaving it. `PluginEventBus.Publish` snapshots its subscribers under the lock and
  calls them outside it; `SquawkDemoFeature.Unregister` takes the token under the lock and
  disposes it after.
- A comment above a lock says what it guards and which threads meet there.

## Timed work and shutdown

The sample's scheduler shows the rules:

- A job is due again one period after its work returns, so it never overlaps itself.
- Jobs start under the scheduler's lock and `Stop()` sets a stopped flag under the same lock, so a
  job either starts before `Stop` (and `Stop` waits for it, up to 2 seconds, warning if it is still
  running) or never starts.
- `FeatureHost.Shutdown` shuts features down in reverse order, then stops the scheduler.

`Shutdown` must leave nothing of yours running or referenced:

1. Dispose every bus subscription and scheduled job handle.
2. Dispose every squawk provider token.
3. `UnregisterScreen` each screen handle and `CloseWindow` each window.
4. Stop and join your own threads; cancel your own cancellation tokens.
5. Clear static fields and caches; remove event subscriptions made outside `BasePlugin`.

The client still answers your calls until `Shutdown` returns, then drops your tag items, screens,
commands, menu entries and providers, unloads the context, and names it in `client.log` if it is
still reachable after three collections.

## Rate limits

Every call that acts for the controller or on the client is rate limited per plugin, counted
separately per operation: a burst of 5, then about one more each second as it refills. An
over-rate call fails and logs a warning; it never disables the plugin.

| Call | Extra limit | When throttled |
|---|---|---|
| Every `ControllerData` write | one a second per aircraft, whatever is left of the burst | returns `false` |
| `Focus.SelectAircraft` | one a second per callsign | returns `false` |
| `Airport` writes | | returns `false`; `BatchUpdateRunways` counts once and returns an empty result |
| `Chat.SendClientMessage` | | returns `false` |
| `Sound.Play` | | returns `false` |
| `Feedback.Show` | | the toast is dropped silently; debounce it yourself |

So a scratchpad set twice on one aircraft in a second fails the second time, while five aircraft
in a second all succeed. To show the burst and refill in a demo, use a call that names no aircraft:
the Showcase posts 7 lines to its own chat channel and 5 succeed. Its scratchpad burst on one
aircraft sends 1 of 7, which demonstrates the per-aircraft limit instead.

Use `BatchUpdateRunways` for bulk runway changes. Never drive any of these from `OnPositionUpdate`
per aircraft.
