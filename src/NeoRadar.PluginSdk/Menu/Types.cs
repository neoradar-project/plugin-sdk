namespace NeoRadar.PluginSdk.Menu;

/// <summary>
/// Represents an entry a plugin adds to the client's plugin list.
/// </summary>
/// <param name="Name">The identifier within the plugin that forms the item id, unique across its entries.</param>
/// <param name="Label">The text the controller sees, which <see cref="IMenuApi.UpdateMenuItem"/> can change.</param>
/// <param name="Checkable">True to show a check state next to the label that a click toggles.</param>
/// <param name="Checked">The initial check state, ignored when <paramref name="Checkable"/> is false.</param>
public sealed record MenuItemDefinition(
    string Name,
    string Label,
    bool Checkable = false,
    bool Checked = false);
