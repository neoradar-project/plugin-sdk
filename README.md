# NeoRadar Plugin SDK

`NeoRadar.PluginSdk` is the public contract between a NeoRadar plugin and the client: the
`BasePlugin` entry point, `ICoreApi` with its sub-APIs, and the event and model types.
A plugin is a plain managed `.dll` that references only this assembly.

## Versioning

`PluginSdkVersion` (major, minor, patch) is the contract version and equals this package's
version. A change that breaks a plugin (a member removed, renamed or retyped, or a signature or
payload changed) bumps the minor and resets the patch; an addition bumps the patch; a change to no
public member bumps nothing. The client loads a plugin compiled against the same major and minor
as the copy it ships, when the client's patch is at least the plugin's. That comparison runs
inside the client; nothing in this package takes part in it.

## Referencing the SDK

From the GitHub Packages feed (needs a PAT with `read:packages`):

```xml
<PackageReference Include="NeoRadar.PluginSdk" Version="0.10.0" ExcludeAssets="runtime" />
```

```xml
<!-- nuget.config -->
<add key="github-neoradar" value="https://nuget.pkg.github.com/neoradar-project/index.json" />
```

Or as a submodule with a compile-only project reference:

```xml
<ProjectReference Include="extern\NeoRadar.PluginSdk\src\NeoRadar.PluginSdk\NeoRadar.PluginSdk.csproj">
  <Private>false</Private>
  <ExcludeAssets>runtime</ExcludeAssets>
</ProjectReference>
```

Rules that bite:

- The reference is compile-only. The client loads your plugin into its own load context and
  resolves the SDK from the copy it ships; a second copy next to your DLL splits the `BasePlugin`
  type identity and the plugin silently fails to load.
- Build with `dotnet build`, never `dotnet publish`.
- `Manifest.SdkMajorVersion` must be `PluginSdkVersion.Major`. The version the client checks is
  the one your assembly's SDK reference records, so rebuilding is the only way to change it.
- Event handlers run on client threads and are watched; keep them short and catch your own
  exceptions.

## Expressions

`ICoreApi.Expressions` exports typed values for package conditions. A package author reads one by
its id wherever a package reads a key: to colour a tag, pick a variant, filter a list or exclude an
aircraft from the safety nets. Use it for state a package should test; a tag item is for text a
controller reads.

```csharp
// Initialize: declare once and keep the id, plugin:{Manifest.Name}:EXP_On Stand.
string onStand = coreApi.Expressions.Declare(
    ExpressionDefinition.Boolean("On Stand", defaultValue: false, "The aircraft has a stand allocated"));

// On the event that changed it, per aircraft, with the overload of the declared type.
coreApi.Expressions.Push(onStand, evt.Callsign, true);
```

```json
{ "description": "on stand", "applyWhen": ["==", "plugin:UK Radar Plugin:EXP_On Stand", "1"] }
```

| Type | Declared with | Pushed with | A condition reads |
|---|---|---|---|
| Boolean | `ExpressionDefinition.Boolean(name, bool, description)` | `Push(id, callsign, bool)` | `"1"` or `"0"` |
| Number | `ExpressionDefinition.Number(name, double, description)` | `Push(id, callsign, double)` | the invariant form, such as `"12.5"` |
| Text | `ExpressionDefinition.Text(name, string, description)` | `Push(id, callsign, string)` | the text as pushed |

- An aircraft the plugin has pushed nothing for reads the declared default. A plugin that is not
  loaded reads empty.
- A push of another type than declared, of an id another plugin declared, or of NaN or an infinity
  returns false and stores nothing.
- Push on the change. A push of the value already held does nothing; one that changes it redraws
  that aircraft at once. The client keeps its own copy and never calls the plugin to evaluate a
  condition.
- Declare in `Initialize`. The client checks a package against what the loaded plugins declared
  when a plugin finishes loading.
- `Manifest.Name` and the expression name form the id a package writes, so renaming either breaks
  every package that names it.
- `with { Scope = ExpressionScope.Session }` and `PushSession(id, value)` hold one value for the
  whole client with no callsign. The client stores and lists it, but a condition evaluated for an
  aircraft reads a session-scoped expression as empty, so nothing a package reads today can use it.

## Windows

`ICoreApi.Window.GetInterface()` opens a window from a `WindowDefinition` and mutates it in place
afterwards. The definition carries the frame as well as the element tree: `Resizable` lets the
controller resize the window and the host remembers the size they leave it at, keyed like its
position, with `MinWidth` and `MinHeight` bounding the drag; `DefaultButtonId` and `CancelButtonId`
make Enter and Escape raise `click` on those buttons while they are enabled.

Every interaction arrives at `OnWindowEvent` as a `WindowEvent`. The vocabulary is closed:

| `EventType` | Raised by | `Value` |
|-------------|-----------|---------|
| `click` | buttons, and any element with `Clickable` set | the button name, or `left`, `right`, `middle` |
| `change` | text and number inputs, selects, checkboxes, toggles, radios, sliders | the new content |
| `select` | a table, when the user changes the selection | the selected row id, empty when cleared |
| `activate` | a table row double-clicked, or Enter pressed on it | the row id |
| `sort` | a table header cell clicked | the column id |
| `toggle` | a table `Check` cell clicked | `rowId\|columnId\|true` or `rowId\|columnId\|false`, the new state |
| `cell` | a table `Button` cell clicked | `rowId\|columnId` |
| `close` | the window closed; `ElementId` is empty | empty |

`toggle` and `cell` pack their ids with `|`, so `|` is reserved in row and column ids. Neither also
raises `select`: clicking a `Check` or `Button` cell leaves the row selection alone.

`Clickable` turns a `Panel`, `Group`, `Grid`, `Label`, `Paragraph`, `Badge` or `Image` into a click
target; buttons raise `click` whether or not it is set.

`ElementType.Table` renders the `WindowElement.Table` definition: `TableColumn` per column with a
Grid-style width (`120`, `auto`, `*`, `2*`), `TableRow` per row with cells aligned to the columns by
index and an optional row `ElementStyle`. `TableColumn.Kind` decides what the column's cells render
as: `Text` shows the cell text as is, `Check` a check box whose cell text is `true` or `false`, and
`Button` a button whose cell text is its caption. The table scrolls and virtualises inside its own bounds,
so bound it with `Style.Height` or `Style.MaxHeight`. The host never sorts: a `sort` event is a
request, and the plugin re-sends the rows in the order it wants. `SetTableRows`, `SetTableSelection`
and `SetOptions` replace rows, selection and Select or Radio choices in place without rebuilding the
control and without raising an event.

`ElementStyle.Role` is the way to follow the client theme without hard-coding RGB: `Surface`,
`SurfaceRaised` and `Muted` colour containers and text from the theme, `Accent`, `Success`,
`Warning` and `Danger` colour text and give containers the theme's subtle background. An explicit
`BackgroundColor` or `TextColor` still wins.

## Rendering

`ICoreApi.Rendering.GetInterface()` registers screens; `Graphics(handle)` returns the retained-mode
`IPluginGraphics` for one of them. Objects are added once and mutated afterwards; there is no
per-frame draw call.

`AddScreenText` and `AddWorldText` take a `TextAlign`. The anchor is the text's left edge, its
horizontal centre or its right edge, at the baseline, so a label centred under a symbol is
`TextAlign.Center` at the symbol's x rather than a guess at half the string's width. The host
measures the real glyphs, and the alignment survives a later `SetText`, so a caption that changes
length stays put.

`MeasureScreenText(text, fontSize)` returns the width and height that text will occupy, for layouts
that have to size themselves: button widths, column fitting, a panel sized to its longest row. It is
synchronous and cheap enough to call while rebuilding a scene, but not once per frame; cache the
result next to the text it measured. It returns `Vector2.Zero` before the host has a font atlas
bound, which is the same condition under which `Graphics(handle)` returns `null`.

## Theme

`ICoreApi.Theme.GetPalette()` returns the active `ThemePalette`: the map background a plugin draws
over, the surface, border and text colours, and the accent, success, warning and danger colours.
Read it at the point of use and re-read it in `OnActiveProfileChanged`, because a profile change can
change the theme.

## Building

```bash
dotnet build
dotnet test
```

The plugin authoring guide and the reference sample plugin live with the client repository.
