using System.Collections.Generic;
using NeoRadar.PluginSdk.Menu;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeMenuApi : IMenuApi
{
    public Dictionary<string, MenuItemDefinition> Items { get; } = new();

    public string RegisterMenuItem(MenuItemDefinition definition)
    {
        string id = "plugin:Sample:MENU_" + definition.Name;
        if (Items.ContainsKey(id)) return string.Empty;
        Items[id] = definition;
        return id;
    }

    public bool UpdateMenuItem(string itemId, string? label, bool? @checked) => Items.ContainsKey(itemId);

    public bool RemoveMenuItem(string itemId) => Items.Remove(itemId);
}
