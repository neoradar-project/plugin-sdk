namespace NeoRadar.PluginSdk.Window;

public interface IWindowInterface
{
    WindowHandle OpenWindow(WindowDefinition definition);

    bool UpdateElement(WindowHandle window, string elementId, string text);

    bool CloseWindow(WindowHandle window);
}

public interface IWindowApi
{
    IWindowInterface? GetInterface();
}
