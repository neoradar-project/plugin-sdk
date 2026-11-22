using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>One page of the guide: a one-line summary, its sections, and the docs page to read next.</summary>
/// <param name="ReadNext">The page of the plugin guide (Plugins section of the NeoRadar docs) that says more.</param>
internal sealed record ShowcasePage(
    string Id,
    string Title,
    string Summary,
    IReadOnlyList<ShowcaseSection> Sections,
    string ReadNext)
{
    public IReadOnlyList<DemoButton> Buttons { get; } = ButtonsOf(Sections);

    public PageInput? Input { get; } = InputOf(Sections);

    public bool HasReadout { get; init; } = true;

    public bool HasCountsTable { get; init; }

    private static List<DemoButton> ButtonsOf(IReadOnlyList<ShowcaseSection> sections)
    {
        List<DemoButton> buttons = new();
        foreach (ShowcaseSection section in sections) buttons.AddRange(section.Buttons);
        return buttons;
    }

    private static PageInput? InputOf(IReadOnlyList<ShowcaseSection> sections)
    {
        foreach (ShowcaseSection section in sections)
            if (section.Input is PageInput input) return input;
        return null;
    }
}
