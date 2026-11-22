using System.Collections.Generic;
using NeoRadar.PluginSdk.Tag;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeTagInterface : ITagInterface
{
    public List<TagItemDefinition> Items { get; } = new();

    public List<TagActionDefinition> Actions { get; } = new();

    public List<(string TagId, string Value, TagContext Context)> Pushes { get; } = new();

    public Dictionary<string, DropdownDefinition> Dropdowns { get; } = new();

    public string RegisterTagItem(TagItemDefinition definition)
    {
        Items.Add(definition);
        return "plugin:Sample:TAG_" + definition.Name;
    }

    public string RegisterTagAction(TagActionDefinition definition)
    {
        Actions.Add(definition);
        return "plugin:Sample:ACTION_" + definition.Name;
    }

    public bool UpdateTagValue(string tagId, string value, TagContext context)
    {
        Pushes.Add((tagId, value, context));
        return true;
    }

    public bool SetActionDropdown(string actionId, DropdownDefinition dropdown)
    {
        Dropdowns[actionId] = dropdown;
        return true;
    }

    public bool UpdateActionDropdown(string actionId, DropdownDefinition dropdown) => SetActionDropdown(actionId, dropdown);

    public bool RemoveActionDropdown(string actionId) => Dropdowns.Remove(actionId);

    public DropdownDefinition? GetDropdownForAction(string actionId) => Dropdowns.GetValueOrDefault(actionId);
}
