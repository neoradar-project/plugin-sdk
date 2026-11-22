# The project, loading and versioning

Docs: https://docs.neoradar.app/plugins/getting-started,
https://docs.neoradar.app/plugins/how-plugins-load, https://docs.neoradar.app/plugins/publishing-a-plugin.

## The project file

A plain class library on `net10.0`. The SDK reference must be compile-only. The sample's project
(`samples/NeoRadar.SamplePlugin/NeoRadar.SamplePlugin.csproj`) is the one to copy:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <!-- Ships private dependencies beside the plugin, where its load context finds them. -->
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
  </PropertyGroup>
  <ItemGroup>
    <!-- Submodule form. The package form is:
         <PackageReference Include="NeoRadar.PluginSdk" Version="0.1.0" ExcludeAssets="runtime" /> -->
    <ProjectReference Include="extern\NeoRadar.PluginSdk\src\NeoRadar.PluginSdk\NeoRadar.PluginSdk.csproj">
      <Private>false</Private>
      <ExcludeAssets>runtime</ExcludeAssets>
    </ProjectReference>
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.5" />
  </ItemGroup>
</Project>
```

- The package comes from the `github-neoradar` feed,
  `https://nuget.pkg.github.com/neoradar-project/index.json`, which needs a GitHub token with
  `read:packages`. The submodule form takes `neoradar-project/plugin-sdk`.
- Compile-only matters: the client supplies its own SDK. A `NeoRadar.PluginSdk.dll` beside your
  plugin defines a second `BasePlugin` type and the client reports "no plugin type".
- Private dependencies (here dependency injection) are copied next to the plugin dll. Ship them with it.
- The sample adds `InternalsVisibleTo` for its test project in `AssemblyInfo.cs`, so its features
  can stay `internal`.
- Build with `dotnet build` (Release to ship). Never `dotnet publish`: the plugin uses the client's
  runtime.

## The plugin class

One `BasePlugin` subclass per assembly. The client finds it by reflection.

```csharp
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Flightplan;
using NeoRadar.PluginSdk.Tag;

namespace MyPlugin;

public sealed class DestinationPlugin : BasePlugin
{
    private ITagInterface _tag = null!;
    private string _destinationTagId = string.Empty;

    public override PluginManifest Manifest { get; } = new(
        Name: "My Plugin",
        Author: "Your Name",
        Version: "1.0.0",
        SdkMajorVersion: PluginSdkVersion.Major,
        Description: "Shows the filed destination as a tag item");

    public override void Initialize(PluginManifest metadata, ICoreApi coreApi, ClientInformation clientInfo)
    {
        _tag = coreApi.Tag.GetInterface();
        _destinationTagId = _tag.RegisterTagItem(new TagItemDefinition(Name: "Destination", DefaultValue: "----"));
        if (_destinationTagId.Length == 0) coreApi.Logger.Warning("The Destination tag item was refused");
    }

    public override void OnFlightplanUpdated(FlightplanUpdatedEvent evt)
    {
        string destination = evt.Destination.Length > 0 ? evt.Destination : "----";
        _tag.UpdateTagValue(_destinationTagId, destination, new TagContext(evt.Callsign));
    }
}
```

The manifest rules the client enforces:

- `Name`: letters, digits, spaces, `-`, `_` and `.` only, unique among loaded plugins in any
  letter case. It is part of every id (`plugin:My Plugin:TAG_Destination`). Never rename it once a
  package uses it.
- `Version`: not empty; letters, digits, spaces, `.`, `-` and `+` only. A supervisor's INF query
  is answered with it.
- `SdkMajorVersion`: always `PluginSdkVersion.Major`, never a typed number.
- `Website` is optional and shown as a link in Plugin Management.

`Initialize` and `Shutdown` must not throw. Catch, log and carry on. Every SDK call returns null,
empty, an empty id or `false` instead of throwing, so check return values.

## How the client loads it

- **From bytes.** The client reads the dll into memory and loads it into an isolated, collectible
  load context. The file is never locked; rebuild and press reload in Plugin Management.
- **`Assembly.Location` is empty.** Never derive a path from it. Keep files under
  `ClientInformation.DocumentsPath`, the data root, in a folder of your own (the sample's `Alerts`
  writes its `.wav` to `<data root>/SamplePlugin`).
- **Two ways in.** **Add Plugin** in Plugin Management, for the session; or a package's
  `manifest.json` `plugins` array, which loads only what the controller approved (or everything,
  while "Trust package plugins" is on). There is no folder scan.
- **Inside a package a plugin must be a `.nrplugin`.** A bare `.dll` is refused there unless the
  trust setting is on. A `.dll` pinned from outside every package still loads.

## The version gate

The client checks the SDK version recorded in your assembly's reference against the copy it ships:

| Plugin's SDK | Result |
|---|---|
| different major or minor | refused |
| same major and minor, newer patch | refused |
| same major and minor, same or older patch | loaded |

A breaking change raises the SDK minor; an addition raises the patch. So a client update that
moves the minor refuses every plugin built before it, with a `client.log` line naming the versions.
Rebuild against the SDK the client ships; rebuilding is the only way to change the recorded version.
`public-api.txt` starts with `sdk-version`, and `PluginSdkVersion.Current` gives it at run time.

## Shipping

- A `.dll` (plus its private dependencies) for a controller to add by hand. Say which NeoRadar
  release it targets.
- A `.nrplugin`, one JSON file carrying one build per platform, is what a package pins. The CLI
  builds one with `neoradar-cli create-plugin-archives <buildDir> -o <outputDir>`; test it on each
  platform you claim.
- Raise `Manifest.Version` on every release; never rename `Manifest.Name` or registered item,
  expression or action names.

## Logs

| File, in the data root | Holds |
|---|---|
| `logs/plugins/<Manifest.Name>.log` | everything you log, `Verbose` included |
| `logs/client.log` | your lines at Debug and above, prefixed `[<Manifest.Name>]`, plus load failures and watchdog trips |

Set `NEORADAR_PLUGIN_LOG_DIR` before starting the client to write plugin logs elsewhere while
developing. The data root is `Documents/NeoRadar` on Windows and macOS.
