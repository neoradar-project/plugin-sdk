using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.PluginSdk.Logger;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.PluginSdk.Window;

namespace NeoRadar.SamplePlugin.Common;

internal sealed class PluginContext
{
    public ICoreApi Core { get; }
    public ILoggerApi Logger => Core.Logger;
    public IRenderingInterface Rendering { get; }
    public IWindowInterface Window { get; }
    public IChatApi Chat => Core.Chat;

    public PluginContext(ICoreApi core)
    {
        Core = core;
        Rendering = core.Rendering.GetInterface();
        Window = core.Window.GetInterface();
    }
}
