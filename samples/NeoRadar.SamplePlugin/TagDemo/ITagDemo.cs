namespace NeoRadar.SamplePlugin.TagDemo;

/// <summary>Pushes the tag demo's values for the selected aircraft; nothing reaches the network.</summary>
internal interface ITagDemo
{
    void PushStatus();

    void PushEmptyStatus();
}
