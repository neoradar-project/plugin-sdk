using NeoRadar.PluginSdk.Focus;
using NeoRadar.PluginSdk.Logger;
using NeoRadar.PluginSdk.Tag;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.TagDemo.Events;

namespace NeoRadar.SamplePlugin.TagDemo;

/// <summary>Two tag items, an action that takes typed input and an action with a dropdown, for a package to bind.</summary>
internal sealed class TagDemoFeature : IFeature, ITagEventHandler, ITagDemo
{
    public const string NoteItemName = "Note";
    public const string StatusItemName = "Status";
    public const string NoteActionName = "Note";
    public const string MenuActionName = "Menu";
    public const string StatusDefault = "DEFAULT";

    private readonly ITagInterface _tags;
    private readonly IFocusApi _focus;
    private readonly ILoggerApi _logger;
    private readonly IPluginEventBus _bus;
    private readonly TagChoiceStore _choices = new();
    private readonly RecentLines _activity = new(8);
    private string _noteId = string.Empty;
    private string _statusId = string.Empty;
    private string _noteActionId = string.Empty;
    private string _menuActionId = string.Empty;

    public TagDemoFeature(HostSurfaces surfaces, IFocusApi focus, ILoggerApi logger, IPluginEventBus bus)
    {
        _tags = surfaces.Tag;
        _focus = focus;
        _logger = logger;
        _bus = bus;
    }

    public void Initialize()
    {
        _noteId = _tags.RegisterTagItem(new TagItemDefinition(NoteItemName, "-") { Title = "NOTE", Description = "The note typed or picked for the aircraft" });
        _statusId = _tags.RegisterTagItem(new TagItemDefinition(StatusItemName, StatusDefault) { Title = "STATUS", Description = "SET once pushed; an empty push shows the default" });
        _noteActionId = _tags.RegisterTagAction(new TagActionDefinition(NoteActionName, RequiresInput: true, "Asks for text and shows it as the note") { Title = "Type a note" });
        _menuActionId = _tags.RegisterTagAction(new TagActionDefinition(MenuActionName, RequiresInput: false, "Opens the sample dropdown") { Title = "Sample menu" });
        if (_noteId.Length == 0 || _statusId.Length == 0 || _noteActionId.Length == 0 || _menuActionId.Length == 0)
            _logger.Warning("TagDemo: a tag item or action was refused");
    }

    public void Shutdown()
    {
    }

    public void PushStatus() => PushOnSelected("SET");

    public void PushEmptyStatus() => PushOnSelected(string.Empty);

    // Client thread; UserInput holds the submitted text for an action that requires input.
    public void OnTagAction(TagActionEvent evt)
    {
        if (evt.ActionId != _noteActionId || evt.UserInput is null) return;
        SetNote(evt.Callsign, evt.UserInput, $"typed on {Where(evt.TagId)}");
    }

    // Client thread; must answer quickly, so it only builds the dropdown from state it already holds.
    public bool OnTagShowDropdown(TagShowDropdownEvent evt)
    {
        if (evt.ActionId != _menuActionId) return true;
        DropdownDefinition dropdown = TagDropdownBuilder.Build(evt.Callsign, _choices.For(evt.Callsign));
        if (_tags.GetDropdownForAction(_menuActionId) is null) _tags.SetActionDropdown(_menuActionId, dropdown);
        else _tags.UpdateActionDropdown(_menuActionId, dropdown);
        return true;
    }

    // Client thread.
    public void OnTagDropdownAction(DropdownActionEvent evt)
    {
        if (evt.ActionId != _menuActionId) return;
        switch (evt.ComponentId)
        {
            case TagDropdownBuilder.Hello:
                SetNote(evt.Callsign, "HELLO", "from the dropdown");
                break;
            case TagDropdownBuilder.Watch:
                Record(evt.Callsign, $"watch {_choices.Change(evt.Callsign, c => c with { Watched = !c.Watched }).Watched}");
                break;
            case TagDropdownBuilder.WatchWithNote:
                _choices.Change(evt.Callsign, c => c with { Watched = !c.Watched });
                SetNote(evt.Callsign, evt.UserInput ?? string.Empty, "typed in the dropdown");
                break;
            case TagDropdownBuilder.Low or TagDropdownBuilder.High:
                Record(evt.Callsign, $"priority {_choices.Change(evt.Callsign, c => c with { Priority = evt.ComponentId }).Priority}");
                break;
        }
    }

    private void PushOnSelected(string value)
    {
        string? callsign = _focus.GetSelectedAircraft();
        if (callsign is null)
        {
            Record("-", "select an aircraft first");
            return;
        }
        bool pushed = _tags.UpdateTagValue(_statusId, value, new TagContext(callsign));
        Record(callsign, $"{StatusItemName} = '{value}' {(pushed ? "pushed" : "refused")}{(value.Length == 0 ? $", so it shows {StatusDefault}" : string.Empty)}");
    }

    private void SetNote(string callsign, string text, string how)
    {
        bool pushed = _tags.UpdateTagValue(_noteId, text, new TagContext(callsign, Colour: (255, 220, 0)));
        Record(callsign, $"note '{text}' {how}: {(pushed ? "pushed" : "refused")}");
    }

    private void Record(string callsign, string line)
    {
        _activity.Add($"{callsign} {line}");
        _bus.Publish(new TagActivityRecorded(_activity.Snapshot()));
    }

    private static string Where(string tagId) => tagId.Length == 0 ? "the target symbol" : tagId;
}
