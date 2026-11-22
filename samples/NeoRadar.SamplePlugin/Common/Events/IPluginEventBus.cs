using System;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Feature-to-feature events, one record type per kind, delivered on the publisher's thread.</summary>
/// <remarks>Subscribers run in subscription order, and one that throws never stops the rest.</remarks>
internal interface IPluginEventBus
{
    IDisposable Subscribe<TEvent>(Action<TEvent> handler);

    void Publish<TEvent>(TEvent evt);
}
