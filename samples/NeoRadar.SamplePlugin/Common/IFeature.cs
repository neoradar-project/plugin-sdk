namespace NeoRadar.SamplePlugin.Common;

/// <summary>One self-contained feature, initialised in registration order and shut down in reverse.</summary>
/// <remarks>Host events arrive through the handler interfaces in Common/Events that the feature opts into.</remarks>
internal interface IFeature
{
    void Initialize();

    void Shutdown();
}
