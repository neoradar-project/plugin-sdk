namespace NeoRadar.PluginSdk.Menu;

/// <summary>
/// A named entry the plugin adds to the client's plugin list, alongside plugin windows and screens.
/// </summary>
/// <param name="Name">Stable identifier within the plugin. It forms the item id and must be unique
/// across the plugin's entries.</param>
/// <param name="Label">Text the controller sees. Changed later with
/// <see cref="IMenuApi.UpdateMenuItem"/>.</param>
/// <param name="Checkable">True to draw a check state next to the label; a click toggles it.</param>
/// <param name="Checked">Initial check state. Ignored when <paramref name="Checkable"/> is
/// false.</param>
public sealed record MenuItemDefinition(
    string Name,
    string Label,
    bool Checkable = false,
    bool Checked = false);
