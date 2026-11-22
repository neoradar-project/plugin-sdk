namespace NeoRadar.SamplePlugin.TagDemo;

/// <summary>What the controller picked in one aircraft's dropdown.</summary>
internal sealed record TagChoice(bool Watched, string Priority)
{
    public static TagChoice None { get; } = new(false, TagDropdownBuilder.Low);
}
