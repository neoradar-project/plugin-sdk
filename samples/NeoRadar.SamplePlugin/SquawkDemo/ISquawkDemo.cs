namespace NeoRadar.SamplePlugin.SquawkDemo;

/// <summary>Registers the demo squawk provider on request only; registering it and making it active each need Demo writes.</summary>
internal interface ISquawkDemo
{
    void Register();

    void Unregister();

    void ListProviders();

    void MakeActive();
}
