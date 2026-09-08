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
<PackageReference Include="NeoRadar.PluginSdk" Version="0.3.0" ExcludeAssets="runtime" />
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

## Building

```bash
dotnet build
dotnet test
```

The plugin authoring guide and the reference sample plugin live with the client repository.
