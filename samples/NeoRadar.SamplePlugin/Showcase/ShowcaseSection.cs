using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>One card of a guide page: a short heading, what it explains, and the demos that show it.</summary>
internal sealed record ShowcaseSection(string Heading, IReadOnlyList<string> Paragraphs, IReadOnlyList<DemoButton> Buttons)
{
    public PageInput? Input { get; init; }

    public bool SendsToNetwork
    {
        get
        {
            foreach (DemoButton button in Buttons)
                if (button.SendsToNetwork) return true;
            return false;
        }
    }
}
