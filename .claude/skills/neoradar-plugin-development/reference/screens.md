# Drawing on the radar and in insets

Docs: https://docs.neoradar.app/plugins/rendering-and-screens. Types:
`src/NeoRadar.PluginSdk/Rendering/` (`Screens.cs`, `IPluginGraphics.cs`, `IPluginScreenRenderer.cs`).
Models in the sample: `WarningZone/` (a decorative world fill), `StatusBadge/` (a screen-space
HUD), `Aman/`, `Tlm/`, `WindProfile/`, `NonSeq/` (inset panels), `MiniMap/` (an inset world view),
`ThemeFollow/` and `Common/ThemeColours.cs` (theme colours for renderers).

## Screens are retained

You register a screen once with an `IPluginScreenRenderer`. You add objects to its scene, keep
each `GraphicsObjectRef`, and afterwards only change them (`SetColor`, `SetText`, `SetVisible`,
`SetTransform`, `SetScreenAnchor` and so on). There is no draw call per frame: the client draws the
scene, and calls your `OnFrameUpdate` on the render thread each frame it draws your view.

## Scope

| `ScreenScope` | Where | World objects |
|---|---|---|
| `MainWorld` | overlay on the main radar in Web Mercator metres; pans and zooms with the map | yes |
| `MainScreen` | overlay on the main radar in pixels (a HUD) | no |
| `PluginInsetWorld` | an inset window with its own camera you can drive | yes |
| `PluginInsetScreen` | an inset window in screen space (a timeline, a list panel) | no |

`AddWorld...` on a screen-space scope returns `GraphicsObjectRef.Invalid` and logs. Screen
coordinates and sizes are device-independent pixels. Keyboard events reach inset scopes only.

## ZOrder

Main-radar screens are ordered by `ZOrder` for input and for screen-space objects. The client's own
layers sit between -20 (sector display) and 90 (feedback overlay): GeoJSON 0, range rings 10,
targets 50, the heading tool 55, tags 60. The default of 100 is above all of them. Choose:

- a warning fill under the targets, around 20 to 30 (the sample's zone uses 30);
- a HUD badge above the tags (the sample's badge uses 80).

World objects ignore `ZOrder` and draw by kind: every world fill, then every world line, then
points, so a world polygon never paints over one of the client's map lines. A hit-testable object
on a screen above an active client tool (the heading tool at 55) takes the press and cancels the
tool; one below it never does.

## Input

- Text is not hittable until `SetHitTestVisible(ref, true)`; lines, polygons, points and
  rectangles are hittable as soon as they are added.
- Decorative objects call `SetHitTestVisible(ref, false)`, so a press inside them reaches the
  targets, the map and the client's tools. A filled zone left hittable swallows every click on the
  aircraft inside it.
- `OnPointerPressed` comes for the left button only, on one of your visible hit-testable objects.
  Return `true` only when you acted; `true` consumes the press and captures the pointer until
  release. `PointerEvent.Hit` names your object; keep a dictionary from ref to your own model.
- Right and middle presses belong to the client.

## Registering

```csharp
public void Initialize()
{
    _handle = _rendering.RegisterScreen(
        new ScreenDefinition(
            Name: "Warning Zone",
            Scope: ScreenScope.MainWorld,
            ZOrder: 30,
            EnabledByDefault: true,
            ShowInList: false),
        _renderer);
    if (!_handle.IsValid) _logger.Warning("Warning Zone: no render surface, so the screen was not registered");
}

public void Shutdown()
{
    if (_handle.IsValid) _rendering.UnregisterScreen(_handle);
    _handle = ScreenHandle.Invalid;
}
```

With `ShowInList` (the default) the screen is a ticked row in the **Insets & Plugin Windows** menu,
with an optional Lucide `Icon`. The controller's on or off choice is remembered across launches;
your own `SetScreenEnabled` lasts for the session. Inset screens wake on position batches;
`BatchFrequency` throttles that (`EveryBatch`, `EveryOtherBatch`, `EveryFourthBatch`, `OnDemand`).

## Disable and enable keep the scene

When a screen is turned off, the client keeps its objects, and turning it on shows them again. Your
refs stay valid. So **do not drop your refs in `OnDisabled` and rebuild**: the old objects are still
in the scene and the rebuild draws a second copy beside them. The sample once did exactly this in
seven renderers; each now has a `TurnedOffAndOn_KeepsItsObjects_AndDrawsNoSecondCopy` test.
`OnDisabled` does not run on unload.

## A frame that costs nothing

`OnFrameUpdate` shares a 16 ms budget with the radar and every other plugin screen. The pattern:

1. Events and jobs on other threads write a `volatile` field or swap a snapshot, and set a dirty flag.
2. `OnFrameUpdate` compares and returns early when nothing changed.
3. Build static chrome once; rebuild only when `info.ScreenSize` changes.
4. Pool objects by key (a callsign) and mutate them; never clear and re-add the scene.
5. Skip a refresh when a signature of what is shown (row count, selection, minute) is unchanged.
6. No allocation, no LINQ, no string building unless the text changed, and no host reads:
   no `Aircraft.GetAll()`, no `Theme.GetPalette()`. Read those on the event or a timer job.
7. Cache `MeasureScreenText`; it is fine for a rebuild, not for every frame.

`StatusBadgeRenderer` is the smallest complete example: it reads the aircraft count on the
caller's thread into a `volatile int`, and the frame only sets the text when the count changed.

## Following the theme

A theme switch raises no event, and `GetPalette` parses on every call. The sample reads it once
a second on the thread pool (`ThemeFollowFeature`), converts it to an immutable `ThemeSnapshot`
with a version number (`ThemeColours`), and each renderer compares the version in its frame and
recolours only on a change (`WarningCircleRenderer`):

```csharp
public void OnFrameUpdate(IPluginGraphics g, FrameInfo info)
{
    ThemeSnapshot theme = _colours.Current;
    if (theme.Version == _themeVersion) return;
    _themeVersion = theme.Version;

    RgbaColor fill = theme.Warning with { A = FillOpacity };
    if (_ring.IsValid)
    {
        g.SetColor(_ring, fill);
        return;
    }

    _ring = g.AddWorldPolygon(new[] { BuildCircle(CircleCenter, CircleRadiusMetres) }, fill);
    g.SetHitTestVisible(_ring, false);
}
```

## Drawing details worth knowing

- `RgbaColor` channels run 0 to 1; `RgbaColor.FromBytes(r, g, b, a)` converts from 0 to 255.
- `AddScreenStrokeRect` width is the full width, snapped to device pixels (1 is one crisp pixel);
  `AddScreenLine` width is from the centre (a hairline is 0.5). Draw boxes with a stroke rect.
- Text alignment is the client's job: `TextAlign.Center` at a symbol's x centres under it;
  `AddScreenTextInRect` centres text in a box.
- `SetTransform` and `SetMinZoom` apply to world objects only.
- On a `PluginInsetWorld`, `SetViewportCenter`, `SetViewportZoom` and `CenterOnLatLon` drive the
  camera (zoom 2 to 18). A filled screen rectangle that is not hit-testable draws below the world
  there, as a background.
- `Graphics(handle)` is available from registration; `MeasureScreenText` returns zero until the
  client has a font ready.
