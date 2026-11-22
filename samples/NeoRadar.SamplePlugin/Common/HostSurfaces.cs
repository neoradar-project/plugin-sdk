using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.PluginSdk.Tag;
using NeoRadar.PluginSdk.Window;

namespace NeoRadar.SamplePlugin.Common;

/// <summary>The rendering, window and tag interfaces, each resolved once from the host.</summary>
internal sealed class HostSurfaces
{
    public HostSurfaces(IRenderingInterface rendering, IWindowInterface window, ITagInterface tag)
    {
        Rendering = rendering;
        Window = window;
        Tag = tag;
    }

    public IRenderingInterface Rendering { get; }

    public IWindowInterface Window { get; }

    public ITagInterface Tag { get; }

    public static HostSurfaces FromHost(ICoreApi core)
        => new(core.Rendering.GetInterface(), core.Window.GetInterface(), core.Tag.GetInterface());
}
