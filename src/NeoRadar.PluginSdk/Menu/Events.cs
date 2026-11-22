namespace NeoRadar.PluginSdk.Menu;

/// <summary>
/// Raised when a controller clicks one of the plugin's menu entries.
/// </summary>
/// <param name="ItemId">The id <see cref="IMenuApi.RegisterMenuItem"/> returned.</param>
/// <param name="Checked">The new check state of a checkable entry, already toggled by the host, otherwise false.</param>
public sealed record MenuItemClickedEvent(string ItemId, bool Checked);
