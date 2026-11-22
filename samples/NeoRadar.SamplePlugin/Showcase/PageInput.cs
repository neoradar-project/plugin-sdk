namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>A text box on a Showcase page whose text, on Enter, runs the demo with that id.</summary>
internal sealed record PageInput(string Placeholder, string DemoId);
