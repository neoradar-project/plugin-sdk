# Designing a window

Docs: https://docs.neoradar.app/plugins/rendering-and-screens (the Windows section). Types:
`src/NeoRadar.PluginSdk/Window/Types.cs` and `IWindowApi.cs`. Model: the Showcase window,
`samples/NeoRadar.SamplePlugin/Showcase/ShowcaseWindowLayout.cs`, `ShowcasePageLayout.cs` and
`ShowcaseWindow.cs`; a smaller form in `DemoWindow/`.

A window is data: a `WindowDefinition` with a tree of `WindowElement`s. You open it once, then
change elements by id. You never see the client's UI toolkit.

## The element set

`Panel`, `Label`, `Button`, `TextInput`, `Checkbox`, `Divider`, `Spacer`, `Group`, `Heading`,
`Paragraph`, `Select`, `Toggle`, `Radio`, `Slider`, `NumberInput`, `ProgressBar`, `Badge`, `Tabs`,
`TabPage`, `ScrollArea`, `Grid`, `Image` and `Table`, nested through `Children`. A `Grid` takes
`Columns` and `Rows` tracks (`"220,1,*"`, `"*,auto"`), and children place themselves with `Row`,
`Col`, `RowSpan` and `ColSpan`.

Give every element you will change an `Id`. Numbers are written and parsed in the invariant
culture (`1.5`, never `1,5`).

## Colour: theme keys first

Colour comes from three places, in this order of preference:

1. **Theme brush keys** on `ElementStyle`: `ThemeBg`, `ThemeFg` and `ThemeBorder` name one of the
   client's own brushes. They follow a theme switch. An unknown key leaves the element with the
   colour it would have had, and the client writes it to `client.log` once per plugin and key.
2. **`Role`** (`Surface`, `SurfaceRaised`, `Muted`, `Accent`, `Success`, `Warning`, `Danger`),
   which also follows the theme.
3. An explicit RGB colour, which does not. Avoid it.

The Showcase uses the same brushes as the client's Package Settings window
(`Showcase/ShowcaseBrushes.cs`):

| Use | Key |
|---|---|
| Nav column and footer | `Brush.TitleBar` |
| Page background | `Brush.Pkg.Base` |
| Card fill | `Brush.Input.Bg` |
| Card outline, subtle rules | `Brush.Border.Subtle` |
| Strong rule between nav and page | `Brush.Border0` |
| Active nav item | `Brush.Toolbar.ItemBg` |
| Text, secondary text, dim text | `Brush.FG1`, `Brush.FG2`, `Brush.Text.Dim` |
| Danger fill and text | `Brush.Danger.Subtle`, `Brush.Danger.Text` |

## Sizes and spacing

- `BorderRadius`, `BorderWidth` and `Spacing` of 0 mean 0: square corners, no border, no gap.
  Leave them unset (`null`) to take the element type's default.
- `WindowDefinition.Padding` is the gap between the frame and the elements; `null` keeps the
  host's 8, and 0 is flush (the Showcase uses 0 so its nav meets the frame).
- `ContentAlign` aligns a button's caption inside the button. `HAlign.Left` makes a nav list.
- `FontWeight.Default` is each type's own weight (SemiBold on a `Heading`).
- `SetElementStyle` re-applies the whole style, so an omitted field falls back to the theme.

## Filling the window

In a window opened with a `Height` above 0, a root `Table`, `ScrollArea`, or `Grid` whose `Rows`
include a star track (`*` or `Nfr`), with no style `Height`, fills the leftover height. So the
pattern for a full-height layout is one root `Grid` with `Rows = "*,auto"`: the star row holds
scroll areas that scroll on their own, the auto row holds the footer.

## The Package Settings recipe

The Showcase reproduces the client's Package Settings window, and a plugin settings window
should look the same:

```text
Grid  Columns "220,1,*"  Rows "*,auto"  ThemeBg Brush.Pkg.Base
  ScrollArea (col 0)  ThemeBg Brush.TitleBar     nav: group headings and full-width buttons
  Panel (col 1)       ThemeBg Brush.Border0      a one-pixel rule
  ScrollArea (col 2)                             the page: a Panel, PaddingH 24, PaddingV 18, Spacing 14, holding cards
  Panel (row 1, ColSpan 3)  ThemeBg Brush.TitleBar   footer: a Divider, then a Grid "*,auto"
```

A nav item is a `Button` with `Height = 30`, `FontSize = 12`, `PaddingH = 10`, `PaddingV = 0`,
`BorderRadius = 0` and `ContentAlign = HAlign.Left`; the active one sets `ThemeBg =
"Brush.Toolbar.ItemBg"`, the others use `ButtonVariant.Ghost` with `ThemeFg = "Brush.FG2"`.

A card:

```csharp
WindowElement card = new()
{
    Type = ElementType.Panel,
    Style = new()
    {
        ThemeBg = "Brush.Input.Bg",
        ThemeBorder = "Brush.Border.Subtle",
        BorderWidth = 1,
        BorderRadius = 0,
        Padding = 12,
        Spacing = 8,
    },
    Children =
    [
        new() { Type = ElementType.Heading, Text = "Stands" },
        new() { Type = ElementType.Label, Id = "stand-count", Text = "No stands allocated" },
    ],
};
```

Mark a button that sends to the network in its card (the Showcase writes "Sends to the network"),
and disable it while the gate is shut.

## Changing a window

- Open once, keep the `WindowHandle`. Check `IsValid`: a plugin may hold 10 windows, and
  `OpenWindow` returns `WindowHandle.Invalid` past that.
- Change only what changed: `UpdateElement` for a text or value, `SetElementStyle` for one nav
  item, `ReplaceChildren` for a page swap, `SetTableRows` for a table, `SetElementEnabled` for a
  gated button. Never rebuild or reopen the window per tick.
- Element calls apply asynchronously. They return `false` only for an unknown or closed window; an
  unknown element is ignored.
- Your own calls never raise a `WindowEvent`. Only the controller does.
- A table never sorts itself: a `sort` event is a request, and you send the rows back in order.

## Events

`OnWindowEvent` runs on the UI thread with `Window`, `ElementId`, `EventType` and `Value`.
`EventType` is `click`, `change`, `submit`, `select`, `activate`, `sort`, `toggle`, `cell` or
`close`. Route by element id, keep the handler short, and hand slow work to the scheduler.
`DefaultButtonId` and `CancelButtonId` make Enter and Escape click those buttons.

## Listed windows and reopening

With `ShowInList: true` the window has a row in the **Insets & Plugin Windows** menu, with its
`Icon` (a Lucide name). When the controller closes it, it is hidden, not destroyed: your calls keep
applying, `close` is still raised, the menu shows it again, and it still counts toward the 10.

The plugin cannot show a hidden window again itself. To offer "reopen" from your own menu entry,
close it and open it afresh, as `ShowcaseFeature.OnMenuItemClicked` does. The client remembers
position and size per plugin and title. Close every window in `Shutdown`.
