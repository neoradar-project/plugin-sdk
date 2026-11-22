using System.Collections.Generic;
using System.Numerics;
using NeoRadar.PluginSdk.Rendering;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeRenderingInterface : IRenderingInterface
{
    private ulong _next;

    public Dictionary<ScreenHandle, (ScreenDefinition Definition, IPluginScreenRenderer Renderer)> Screens { get; } = new();

    public Dictionary<ScreenHandle, bool> Enabled { get; } = new();

    public List<(ScreenHandle Handle, double Lat, double Lon)> Centred { get; } = new();

    public ScreenHandle RegisterScreen(ScreenDefinition definition, IPluginScreenRenderer renderer)
    {
        ScreenHandle handle = new(++_next);
        Screens[handle] = (definition, renderer);
        Enabled[handle] = definition.EnabledByDefault;
        return handle;
    }

    public bool UnregisterScreen(ScreenHandle handle)
    {
        Enabled.Remove(handle);
        return Screens.Remove(handle);
    }

    public bool SetScreenEnabled(ScreenHandle handle, bool enabled)
    {
        if (!Screens.TryGetValue(handle, out (ScreenDefinition Definition, IPluginScreenRenderer Renderer) screen)) return false;
        bool was = Enabled[handle];
        Enabled[handle] = enabled;
        if (was != enabled)
        {
            if (enabled) screen.Renderer.OnEnabled();
            else screen.Renderer.OnDisabled();
        }
        return true;
    }

    public bool IsScreenEnabled(ScreenHandle handle) => Enabled.GetValueOrDefault(handle);

    public IPluginGraphics? Graphics(ScreenHandle handle) => null;

    public ScreenHandle Named(string name)
    {
        foreach (KeyValuePair<ScreenHandle, (ScreenDefinition Definition, IPluginScreenRenderer Renderer)> entry in Screens)
            if (entry.Value.Definition.Name == name) return entry.Key;
        return ScreenHandle.Invalid;
    }

    public bool CenterOnLatLon(ScreenHandle handle, double lat, double lon, double? zoom = null)
    {
        Centred.Add((handle, lat, lon));
        return Screens.ContainsKey(handle);
    }

    public bool SetViewportCenter(ScreenHandle handle, Vector2 worldCenter) => Screens.ContainsKey(handle);
}
