# NeoRadar Plugin SDK

`NeoRadar.PluginSdk` is the public contract between a NeoRadar plugin and the client: the
`BasePlugin` entry point, `ICoreApi` with its sub-APIs, and the event and model types.
A plugin is a plain managed `.dll` that references only this assembly.

## Versioning

`PluginSdkVersion` (major, minor, patch) is the contract version and equals this package's
version. The client ships one copy of the SDK and refuses to load a plugin compiled against a
different major or minor, so a plugin is always rebuilt against the SDK release its target client
ships. That comparison runs inside the client against the copy it ships; nothing in this package
takes part in it. While the major is 0 a breaking change bumps the minor.

## Referencing the SDK

From the GitHub Packages feed (needs a PAT with `read:packages`):

```xml
<PackageReference Include="NeoRadar.PluginSdk" Version="0.5.0" ExcludeAssets="runtime" />
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
- `Manifest.SdkMajorVersion` must be `PluginSdkVersion.Major` and `Manifest.SdkBuildId` must be
  `PluginSdkVersion.BuildId`. Both are compile-time constants, so the plugin carries the exact SDK
  build it was compiled against, and the client refuses a plugin compiled against any other build
  of the same version.
- Event handlers run on client threads and are watched; keep them short and catch your own
  exceptions.

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
| `close` | the window closed; `ElementId` is empty | empty |

`Clickable` turns a `Panel`, `Group`, `Grid`, `Label`, `Paragraph`, `Badge` or `Image` into a click
target; buttons raise `click` whether or not it is set.

`ElementType.Table` renders the `WindowElement.Table` definition: `TableColumn` per column with a
Grid-style width (`120`, `auto`, `*`, `2*`), `TableRow` per row with cells aligned to the columns by
index and an optional row `ElementStyle`. The table scrolls and virtualises inside its own bounds,
so bound it with `Style.Height` or `Style.MaxHeight`. The host never sorts: a `sort` event is a
request, and the plugin re-sends the rows in the order it wants. `SetTableRows`, `SetTableSelection`
and `SetOptions` replace rows, selection and Select or Radio choices in place without rebuilding the
control and without raising an event.

`ElementStyle.Role` is the way to follow the client theme without hard-coding RGB: `Surface`,
`SurfaceRaised` and `Muted` colour containers and text from the theme, `Accent`, `Success`,
`Warning` and `Danger` colour text and give containers the theme's subtle background. An explicit
`BackgroundColor` or `TextColor` still wins.

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
