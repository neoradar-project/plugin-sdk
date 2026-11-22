namespace NeoRadar.SamplePlugin.Common;

/// <summary>The Showcase's "Demo writes" tick, which every demo that sends to the network checks first.</summary>
/// <remarks>Closed at load and never remembered.</remarks>
internal sealed class DemoWritesGate
{
    // Set on the UI thread, read on client threads.
    private volatile bool _open;

    public bool IsOpen => _open;

    public void Set(bool open) => _open = open;
}
