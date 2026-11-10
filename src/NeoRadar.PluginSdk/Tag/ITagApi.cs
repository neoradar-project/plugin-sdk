namespace NeoRadar.PluginSdk.Tag;

public interface ITagInterface
{
    string RegisterTagItem(TagItemDefinition definition);

    string RegisterTagAction(TagActionDefinition definition);

    bool UpdateTagValue(string tagId, string value, TagContext context);

    bool SetActionDropdown(string actionId, DropdownDefinition dropdown);

    bool UpdateActionDropdown(string actionId, DropdownDefinition dropdown);

    bool RemoveActionDropdown(string actionId);

    DropdownDefinition? GetDropdownForAction(string actionId);
}

public interface ITagApi
{
    /// <summary>The host's interface; never null, and the same instance on every call.</summary>
    ITagInterface GetInterface();
}
