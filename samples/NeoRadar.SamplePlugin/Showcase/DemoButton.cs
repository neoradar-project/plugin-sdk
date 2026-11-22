namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>A Showcase button: the demo it runs, its label, and what it sends to the network, if anything.</summary>
/// <param name="DemoId">The id a feature's <see cref="IShowcaseDemos"/> registers the demo under.</param>
/// <param name="Sends">What reaches FSD or the server, said on the button's tooltip; null for a demo that stays on this client.</param>
internal sealed record DemoButton(string DemoId, string Label, string? Sends = null)
{
    public bool SendsToNetwork => Sends is not null;
}
