# Docs, MCP and where to find things

## Search the docs first

The docs at https://docs.neoradar.app describe what the client does with a plugin. Read them before
assuming a limit, an id format or a thread. Access needs a VATSIM account with Client Access, the
same grant that lets you use NeoRadar.

### Connect the MCP server

The docs have an MCP server at `https://docs.neoradar.app/mcp` with three tools:

| Tool | Gives |
|---|---|
| `search_docs` | keyword search, the best pages with a snippet |
| `read_doc` | one page in full, as markdown |
| `list_docs` | the table of contents |

Claude Code:

```bash
claude mcp add --transport http neoradar-docs https://docs.neoradar.app/mcp
```

Then run `/mcp`, choose `neoradar-docs` and sign in. A browser opens: sign in with VATSIM if
needed, then press **Allow**. The agent reads the docs as your CID and never sees your password or
tokens. Other clients add the URL as a remote (streamable HTTP) server with OAuth; claude.ai adds
it under Settings, Connectors, Add custom connector.

Good first searches: the API you are about to call (`SetActionDropdown`, `RegisterScreen`), a
symptom (`Faulted`, `no plugin type`), or a limit (`rate limit`, `watchdog`).

### Markdown downloads

While signed in to the site:

| Link | What it is |
|---|---|
| https://docs.neoradar.app/llms-full.txt | every page in one file, in reading order (about 1 MB) |
| https://docs.neoradar.app/llms.txt | an index of every page, linking each page's markdown |
| `<page>.md` | one page's markdown: add `.md` to a page's address |

The setup page itself: https://docs.neoradar.app/getting-started/docs-for-ai-agents

## The plugin pages, by topic

| Topic | Page |
|---|---|
| What a plugin can and cannot do, the 21 sub-APIs | https://docs.neoradar.app/plugins/overview |
| Loading from bytes, package offers and approval, the version gate, watchdogs, unloading | https://docs.neoradar.app/plugins/how-plugins-load |
| A first plugin: project, SDK reference, build, install, debug | https://docs.neoradar.app/plugins/getting-started |
| Building and loading the Showcase, its pages, its package fragment | https://docs.neoradar.app/plugins/the-showcase-sample |
| Tag items, exported expressions, tag actions, dropdowns, ids | https://docs.neoradar.app/plugins/tag-items-and-expressions |
| Chat commands, parameters, reserved names | https://docs.neoradar.app/plugins/commands |
| Squawk providers | https://docs.neoradar.app/plugins/squawk-provider |
| Posting to chat, channels, incoming message events | https://docs.neoradar.app/plugins/chat |
| Screens, ZOrder, drawing, windows, window events | https://docs.neoradar.app/plugins/rendering-and-screens |
| Lifecycle, threads, the timer, menus, shutting down | https://docs.neoradar.app/plugins/events-and-lifecycle |
| Log files, diagnosing a load failure or a disable | https://docs.neoradar.app/plugins/logging-and-diagnostics |
| `.dll` and `.nrplugin`, shipping with a package, versioning | https://docs.neoradar.app/plugins/publishing-a-plugin |
| Every member, rate limits, events | https://docs.neoradar.app/plugins/sdk-reference |

Related pages outside the plugin section:

- Where the data root and logs are: https://docs.neoradar.app/getting-started/where-files-live
- The Plugin Management window: https://docs.neoradar.app/using-the-client/plugin-management-window
- Packages, which bind plugin ids: https://docs.neoradar.app/packages/overview, the manifest's
  `plugins` array at https://docs.neoradar.app/packages/manifest, lists at
  https://docs.neoradar.app/packages/lists

## In the SDK repo

| What | Where |
|---|---|
| Every public member, one per line, with the SDK version on the first line | `tests/NeoRadar.PluginSdk.Tests/PublicApi/public-api.txt` |
| The API itself, one folder per sub-API, XML docs on each member | `src/NeoRadar.PluginSdk/` |
| The events and their threads | `src/NeoRadar.PluginSdk/BasePlugin.cs` |
| Window elements and styles | `src/NeoRadar.PluginSdk/Window/Types.cs` |
| Screens and drawing | `src/NeoRadar.PluginSdk/Rendering/` |
| The working example of everything | `samples/NeoRadar.SamplePlugin/` |
| Its tests and fakes | `tests/NeoRadar.SamplePlugin.Tests/` |

The XML docs ship with the package, so the IDE shows each member's summary, units and sentinels.
Grep `public-api.txt` to check a member name before writing it.

## The Showcase, page by page

Load `samples/NeoRadar.SamplePlugin/bin/Release/net10.0/NeoRadar.SamplePlugin.dll` with **Add
Plugin**. Each page explains one area and runs a live demo of it.

| Group | Page | Shows | Source |
|---|---|---|---|
| Start | Start | what the host passed to `Initialize` | `Showcase/DescribeHost.cs` |
| | Boundary | the cost of one host call | `Showcase/HostDemos.cs` |
| | Threads | which thread a click and a pool job run on | `Showcase/HostDemos.cs` |
| | Feature host | the registered features | `Common/` |
| Traffic | Flight plans | the selected aircraft's parsed route | `RouteInspector/` |
| | Controller data | controller-data writes and the per-aircraft limit | `TrackWrites/` |
| | Airports | configured airports, runways, navaid lookup | `RunwayBoard/` |
| | Squawk | a squawk provider | `SquawkDemo/` |
| Screens | Tags and menus | tag items, actions, dropdowns, a menu entry | `TagDemo/`, `Showcase/ShowcaseFeature.cs` |
| | Expressions | values package conditions read | `QuietHours/`, `TagDemo/` |
| | Screens and windows | overlays, insets, a form window | `StatusBadge/`, `Aman/`, `DemoWindow/`, `WarningZone/`, `MiniMap/` |
| | Sound and focus | toasts, a `.wav`, following the selection | `Alerts/` |
| | Render performance | drawing within the frame budget | `Aman/`, `Tlm/`, `WindProfile/`, `NonSeq/` |
| Network | Network and chat | who is online, the plugin's own chat channels | `WhoIsOnline/` |
| | Commands | dot-commands with parameter rules | `Commands/` |
| | Rate limits | the burst and the refill | `WhoIsOnline/`, `Alerts/` |
| Settings and events | Settings | the plugin and profile scopes | `SettingsScopes/` |
| | Event log | every `BasePlugin` event counted | `EventLog/` |

`ThemeFollow/` has no page; it reads the palette once a second for every renderer.
