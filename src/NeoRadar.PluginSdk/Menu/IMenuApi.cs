namespace NeoRadar.PluginSdk.Menu;

/// <summary>
/// Named entries the plugin contributes to the client's plugin list, where they sit alongside the
/// plugin's windows and screens. An entry is the cheapest way to give a controller a command that
/// is not tied to an aircraft tag.
/// </summary>
public interface IMenuApi
{
    /// <summary>
    /// Add an entry to the plugin list. Returns the item id, which is
    /// <c>plugin:{Manifest.Name}:MENU_{Name}</c> and is stable across loads, or an empty string
    /// when the entry could not be registered because the name is empty or already in use.
    /// </summary>
    string RegisterMenuItem(MenuItemDefinition definition);

    /// <summary>
    /// Change an entry's label or check state. A null argument leaves that field alone. Returns
    /// false for an unknown item id.
    /// </summary>
    bool UpdateMenuItem(string itemId, string? label, bool? @checked);

    /// <summary>
    /// Remove an entry from the plugin list. Returns false for an unknown item id.
    /// </summary>
    bool RemoveMenuItem(string itemId);
}
