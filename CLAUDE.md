# CLAUDE.md

<!-- shared:constellation -->
## The NeoRadar constellation

Six repos, developed together. Cross-repo references use `@id` tokens, never paths: `@client` (C# radar
client), `@server` (Go compute tier), `@hub` (JS management dashboard), `@cli` (JS package and dataset
build pipeline), `@schemas` (shared JSON Schemas), `@pluginsdk` (C# plugin SDK, vendored into `@client` at
`extern/NeoRadar.PluginSdk`). Data: `@packages` (built packages), `@sectorsrc` (CLI source sector files),
`@data` (runtime data root). An id resolves through the `NEORADAR_<ID>_DIR` env var, then
`neoradar.repos.local.json` at the repo root (gitignored, per-machine), then an upward scan using the
`defaultSubpaths` in `neoradar.repos.json`. If it does not resolve, say so and work from this repo alone;
never guess a path. A sibling repo is normally absent in CI, and that is not an error.

These pairs move in LOCKSTEP; a one-sided edit is a break, not a cleanup:
- `@server/internal/contract/` and `@client/src/NeoRadar.Core/ServerLink/Contract/`
- `@server/internal/contract/testdata/*.json` and `@client/tests/NeoRadar.Core.Tests/ServerLink/Vectors/*.json`
- `@server/internal/contract/vocabulary.json` and `@client/tests/NeoRadar.Core.Tests/ServerLink/vocabulary.json`
- `@server/internal/trajectory/testdata/copx-matcher-cases.json` and
  `@client/tests/NeoRadar.Core.Tests/FutureOwnership/Copx/copx-matcher-cases.json`

Alpha: no backwards-compatibility or fallback layers unless asked, especially in ServerLink. Every process
or data-format change states how it rolls out to the existing sector files. Thousands of controllers use
this, so failure paths are handled and logged, never swallowed. Commits are signed (never `--no-verify`,
never bypass signing), made only when asked and never pushed unless asked; no commit or PR carries
`Co-Authored-By` or any other AI attribution. This block is identical in all six repos.
<!-- /shared:constellation -->

`NeoRadar.PluginSdk` is the public contract between NeoRadar plugins and the client. It is
distributed to plugin authors, vendored into the client (`@client`, `extern/NeoRadar.PluginSdk`)
and into `uk-plugin-sharp` as a git submodule, and packed to the `github-neoradar` NuGet feed on
a `v*` tag. Everything `public` here is a customer contract.

## Invariants

- **`PluginSdkVersion` equals the csproj `Version`.** The client compares the SDK version a plugin
  was compiled against (its assembly reference) with the copy it ships: it loads the plugin when
  the major.minor match and its own patch is at least the plugin's.
  `PluginSdkVersionTests.AssemblyVersion_MatchesTheConstants` pins the two together; bump both in
  the same commit, then tag `v<version>` so `pack.yml` publishes.
- **This assembly carries the constants, never the rule.** The compatibility decision is
  `SdkVersionGate` in `@client` (`src/NeoRadar.PluginRuntime/`), evaluated against the client's own
  copy. No predicates or refusal messages here: a plugin author cannot influence the check, and a
  policy change must not need an SDK release.
- **The version says what changed.** A change that breaks a plugin (a member removed, renamed or
  retyped, a signature or payload changed) bumps `Minor` and sets `Patch` to 0. An addition (a new
  event, API method, type or optional member) bumps `Patch`. A change to no public member (docs,
  tests, internals) bumps nothing.
- **The public-API baseline enforces that rule.** `tests/NeoRadar.PluginSdk.Tests/PublicApi/public-api.txt`
  records the version and every public member; `PublicApiBaselineTests` fails when the surface
  differs from it, when an addition ships without the next patch or a break without the next minor,
  and when the version moves with no public change. After the bump, rerun with
  `NEORADAR_UPDATE_SDK_API=1` to rewrite it; the run refuses to for an unchanged version.
- **AOT compatible.** `NeoRadar.exe` is AOT and links this assembly (`IsAotCompatible`). No
  reflection-driven serialisation, no dynamic code.
- **No hosting internals.** The AOT-to-JIT function-pointer ABI between NeoRadar and
  `NeoRadar.PluginRuntime` lives in the client at `src/NeoRadar.PluginHosting/`, never here. This
  assembly has no friend assemblies and no unsafe code.
- **Models are `sealed record`s with `init` properties; events are `virtual` methods on
  `BasePlugin`.** Keep that shape: it is what plugin authors have written against.
- **Public members carry XML docs** (`GenerateDocumentationFile`); `CS1591` is suppressed only so
  a missing doc does not fail the build, not as permission to omit them.

## Rollout of a change

1. Change here, with tests. 2. Bump `PluginSdkVersion` and `<Version>`: `Minor` for a break,
`Patch` for an addition, nothing when no public member changed; rewrite the API baseline
(`NEORADAR_UPDATE_SDK_API=1`). 3. Move the `@client` submodule pin, rebuild the client and its
`PluginRuntime`, run its suites.
4. Move the `uk-plugin-sharp` pin and rebuild. 5. Tag `v<version>`; the pack workflow publishes.

## Build and test

`dotnet build -c Release`, then `dotnet test -c Release --no-build` (tests in `tests/NeoRadar.PluginSdk.Tests`);
CI runs the same.

<!-- shared:writing-rules -->
## Writing rules

These hold for code, comments, docs, UI copy and commit messages in every repo; the hook in
`.claude/hooks/style-check.sh` flags the first four on each edit.
- UK English.
- No em dashes anywhere. Use a period, semicolon, colon or middot.
- C#: explicit types, never `var`.
- A comment says only what the code cannot, in at most two lines. It never restates the code, narrates
  history or explains a mechanism in prose; if it needs more room, the name is wrong.
- A comment never cites a roadmap, phase, wave, track, card, backlog, plan or decision id (`P8`, `Wave 3`,
  `C1-D7`, `B110`, `G-TOOL-04`). It states what the code does in terms that stay true once the plan is
  done; where a pointer helps, it names a test or a method.
- Names carry the meaning: a loop or action is verb first, data is a noun, an event is past tense.
- A comment MAY state a thread, a lock or gate, a unit, a sentinel or an ordering, and must be true: re-read the
  code before writing or keeping one.
This block is identical in all six repos.
<!-- /shared:writing-rules -->

## Code style

Allman braces. `_camelCase` privates, `PascalCase` constants. `using` outside the namespace, `System` first.
Nullable on. Every behaviour change ships with a test in the same commit. Hand-rolled fakes, no mocking
framework.
