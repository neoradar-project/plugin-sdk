using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>A heading of the Showcase's left nav and the guide pages listed under it.</summary>
internal sealed record ShowcaseNavGroup(string Heading, IReadOnlyList<ShowcasePage> Pages);
