# CLAUDE.md

`NeoRadar.PluginSdk` is the public contract between NeoRadar plugins and the client. It is
distributed to plugin authors, vendored into the client (`@client`, `extern/NeoRadar.PluginSdk`)
and into `uk-plugin-sharp` as a git submodule, and packed to the `github-neoradar` NuGet feed on
a `v*` tag. Everything `public` here is a customer contract.

## Invariants

- **`PluginSdkVersion` equals the csproj `Version`.** The client compares the SDK version a plugin
  was compiled against (its assembly reference) with the copy it ships and refuses any other
  major.minor. `PluginSdkVersionTests.AssemblyVersion_MatchesTheConstants` pins the two together;
  bump both in the same commit, then tag `v<version>` so `pack.yml` publishes.
- **A breaking change to the public surface bumps `Minor` while `Major` is 0.** Renames, removed
  members, changed signatures, changed event payloads. Additive changes need no bump but do need
  the consumers' submodule pins moved before a plugin can use them.
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

1. Change here, with tests. 2. Bump `PluginSdkVersion` and `<Version>` if the surface broke.
3. Move the `@client` submodule pin, rebuild the client and its `PluginRuntime`, run its suites.
4. Move the `uk-plugin-sharp` pin and rebuild. 5. Tag `v<version>`; the pack workflow publishes.

## Code style

Explicit types, never `var`. Allman braces. `_camelCase` privates, `PascalCase` constants.
`using` outside the namespace, `System` first. Nullable on. Comments say WHAT only, at most two
lines, pointing at a test rather than narrating a mechanism; no em dashes anywhere. Every
behaviour change ships with a test in the same commit. Hand-rolled fakes, no mocking framework.
Never add `Co-Authored-By` or any AI attribution to a commit.
