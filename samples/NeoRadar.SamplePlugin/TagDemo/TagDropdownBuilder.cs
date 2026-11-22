using NeoRadar.PluginSdk.Tag;

namespace NeoRadar.SamplePlugin.TagDemo;

/// <summary>Builds the dropdown for one aircraft, ticking the toggle and radio from what was picked before.</summary>
internal static class TagDropdownBuilder
{
    public const string Hello = "hello";
    public const string Watch = "watch";
    public const string WatchWithNote = "watch-note";
    public const string Low = "low";
    public const string High = "high";

    public static DropdownDefinition Build(string callsign, TagChoice choice) => new(
        Title: callsign,
        Width: 180,
        MaxHeight: 260,
        Components:
        [
            new DropdownComponent { Id = Hello, Type = DropdownComponentType.Button, Text = "Say hello in the note" },
            new DropdownComponent { Type = DropdownComponentType.Divider },
            new DropdownComponent { Id = Watch, Type = DropdownComponentType.Toggle, Text = "Watch", Checked = choice.Watched },
            new DropdownComponent
            {
                Id = WatchWithNote,
                Type = DropdownComponentType.Toggle,
                Text = "Watch, with a note",
                Checked = choice.Watched,
                RequiresInput = true,
            },
            new DropdownComponent { Type = DropdownComponentType.Label, Text = "Priority" },
            new DropdownComponent { Id = Low, Type = DropdownComponentType.Radio, Text = "Low", Checked = choice.Priority == Low },
            new DropdownComponent { Id = High, Type = DropdownComponentType.Radio, Text = "High", Checked = choice.Priority == High },
        ]);
}
