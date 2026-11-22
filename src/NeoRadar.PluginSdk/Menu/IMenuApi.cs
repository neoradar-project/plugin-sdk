namespace NeoRadar.PluginSdk.Menu;

/// <summary>
/// Provides entries in the client's plugin list for commands that are not tied to an aircraft tag.
/// </summary>
public interface IMenuApi
{
    /// <summary>
    /// Adds an entry to the plugin list.
    /// </summary>
    /// <returns>The id <c>plugin:{Manifest.Name}:MENU_{Name}</c>, or an empty string if the name is empty or in use.</returns>
    string RegisterMenuItem(MenuItemDefinition definition);

    /// <summary>
    /// Changes an entry's label or check state, leaving a null argument's field unchanged.
    /// </summary>
    /// <returns>False if the item id is unknown or another plugin's.</returns>
    bool UpdateMenuItem(string itemId, string? label, bool? @checked);

    /// <summary>
    /// Removes an entry from the plugin list.
    /// </summary>
    /// <returns>False if the item id is unknown or another plugin's.</returns>
    bool RemoveMenuItem(string itemId);
}
