# What a package binds: tags, expressions, actions, commands and more

Docs: https://docs.neoradar.app/plugins/tag-items-and-expressions,
https://docs.neoradar.app/plugins/commands, https://docs.neoradar.app/plugins/chat,
https://docs.neoradar.app/plugins/squawk-provider,
https://docs.neoradar.app/plugins/events-and-lifecycle (menus). Models: `TagDemo/`,
`QuietHours/`, `Commands/`, `WhoIsOnline/`, `SquawkDemo/`, `Alerts/`, `SettingsScopes/`, and the
`Package/` fragment.

A controller-facing item in a data tag or list is package data. A plugin supplies the values and
the clicks; the package decides where they appear. Design the two together.

## Ids are a contract

```text
plugin:{Manifest.Name}:TAG_{item name}
plugin:{Manifest.Name}:EXP_{expression name}
plugin:{Manifest.Name}:ACTION_{action name}
plugin:{Manifest.Name}:MENU_{menu item name}
```

You register the bare name; the client adds the prefix and returns the full id. An empty string
back means the name was empty, already used, or made an id of 128 characters or more. Renaming the
plugin or an item turns every package reference into empty, silently. Plugin Management shows the
registered ids on the plugin's row.

## Choosing

| You want | Use |
|---|---|
| Text a controller reads in a tag or list | tag item (`ITagInterface.RegisterTagItem`, `UpdateTagValue`) |
| A click or dropdown on it | tag action (`RegisterTagAction`, `OnTagAction`, `SetActionDropdown`) |
| A flag, number or code package conditions test | expression (`ICoreApi.Expressions.Declare`, `Push`) |
| Both text and state | a tag item for the text, an expression for the state |
| A command typed in chat | `ICoreApi.Chat.RegisterCommand` |
| An entry not tied to an aircraft | `ICoreApi.Menu.RegisterMenuItem` |

## Tag items

- Register in `Initialize`; push per aircraft on the event that changed the data, never per frame
  or per batch. Pushing the value already held is cheap, but still push on change.
- Nothing pushed, or an empty push, reads the `DefaultValue` on labels, in lists and in conditions.
- `TagContext` carries the callsign and optional text and background colours. A condition sees
  only the value string, never the colour.
- A tag item is also a key in the package expression language, so it can colour, filter or exclude.

## Expressions

- `ExpressionDefinition.Boolean`, `.Number` or `.Text`, declared in `Initialize` (the client checks
  packages against what loaded plugins declared once the load finishes).
- `Push(id, callsign, value)` with the overload of the declared type. A wrong type, another
  plugin's id, NaN or an infinity returns `false` and stores nothing.
- Nothing pushed reads the declared default. A plugin that is not loaded reads empty, so a Boolean
  default of `false` (`"0"`) lets a condition tell "no" from "not loaded".
- `Scope = ExpressionScope.Session` with `PushSession(id, value)` holds one value for the whole
  client, for a map item's `showWhen`. The sample's Quiet Hours flag does this.
- `.dev plugin-expressions [callsign]` in the chat input reports every loaded plugin's expressions.

## Tag actions and dropdowns

- `TagActionEvent` gives `Callsign` (the track clicked) and `PlanCallsign` (the flight plan it
  stands for). Look up flight-plan data by `PlanCallsign`.
- `RequiresInput: true` opens an input box in the label or list cell. A target-symbol click cannot
  ask for input and is refused, and an input action cannot also show a dropdown.
- Build the dropdown in `OnTagShowDropdown` with `SetActionDropdown`; return `false` to stop it
  opening. Components: `Button`, `Divider`, `Label`, `Toggle`, `Radio`, `Group`, `ScrollArea`,
  `InputArea`. A press reports `ComponentId` to `OnTagDropdownAction` and closes the dropdown
  without switching a toggle, so set it again to show the new state.
- `OnTagShowDropdown` counts toward the watchdogs; keep it cheap. The sample builds its dropdown
  in `TagDropdownBuilder` from a store.

## The package fragment

`samples/NeoRadar.SamplePlugin/Package/` is copied beside the built dll and shows a package using
the Sample plugin's ids:

| File | Binds |
|---|---|
| `systems/sample-label-items.json` | `plugin:Sample:TAG_Note` with left and right clicks on `ACTION_Note` and `ACTION_Menu` |
| `systems/sample-lists.yaml` | a list filtered by `["==", "plugin:Sample:EXP_Flagged", "1"]`, with plugin tag items as columns |
| `systems/sample-target-symbol.json` | a symbol rule applied by an expression, with a click the client refuses because the action needs input |
| `sample-map-layer-entry.json`, `datasets/sample-quiet-hours.geojson` | a map layer shown by the session expression `EXP_Quiet Hours` |

Ship a fragment like this with your plugin so package authors copy working ids, and say in your
release notes which plugin version the package expects. Package syntax:
https://docs.neoradar.app/packages/overview.

## Commands

- `RegisterCommand(name, definition, provider)`; the controller types `.name`. Names compare
  without case against the built-ins and every other plugin. Built-in names are reserved:
  `audio`, `center`, `chat`, `clear`, `cpdlc`, `dev`, `find`, `inf`, `msg`, `rings`, `vis`,
  `wallop`, `wx`. Prefix yours with your plugin or vACC name.
- Declare `AllowedValues` and `Pattern` on parameters; the client validates as the controller types,
  and a bad value never reaches `Execute`. An empty or invalid pattern refuses the registration.
- `Execute` runs on a client thread and is watched. Return a `CommandResult` at once; for slow work
  return "Working..." and report later with `SendClientMessage`.
- The sample has one class per command implementing `ISampleCommand` (`Commands/`).

## Chat

- `SendClientMessage` is local to this client; nothing goes to the network.
- With no `Channel` the line goes to the System channel. With one, it goes to a channel of your
  own, `[name]`. Up to 8 names per plugin, 32 characters each. Refused: empty, edge spaces, a colon
  or control character, a client channel name in any case (`System`, `Server`, `ATC`, `Sup`,
  `Supervisor`, `Broadcast`, `Frequency`, `AFV`), or a frequency (`118.500`).
- Use a toast (`Feedback.Show`) for something the controller may miss; chat for something they
  must be able to read again.

## Squawk providers

- `RegisterProviderWithToken` returns an `IDisposable` (or `null` when refused). Dispose it in
  `Shutdown`.
- Return four octal digits or `null` to decline; never throw. Honour the cancellation token.
- Higher `Priority` is asked first. A package's policy can disable a provider, replace its priority
  for ranking, and reserve codes.
- Registering is network-visible: a provider that ranks first starts supplying the codes the
  client assigns, with no further call. Gate it like any other write.

## Menu entries, settings, sound and toasts

- `Menu.RegisterMenuItem(new MenuItemDefinition(Name, Label, Checkable, Checked))` adds a row to
  the plugin section of the **Insets & Plugin Windows** menu; `OnMenuItemClicked` reports it.
- `Settings.Get`/`Set` follow the controller everywhere; `GetForProfile`/`SetForProfile` are per
  profile. Values are strings. Re-read profile settings in `OnActiveProfileChanged`.
- `Sound.Play(path)` plays a `.wav` you own (an `.mp3` only on macOS and Linux) and returns
  `false` for a missing file, an unplayable format or a throttle. The sample writes its `.wav`
  under the data root the first time it is played (`Alerts/SoundFileWriter.cs`).
- `Feedback.Show(text, kind, durationMs)` shows one toast at a time; a throttled one is dropped.
