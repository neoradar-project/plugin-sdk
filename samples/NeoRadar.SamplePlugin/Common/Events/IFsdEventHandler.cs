using NeoRadar.PluginSdk.Fsd;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>The user's own FSD connection events, on client threads.</summary>
internal interface IFsdEventHandler
{
    void OnFsdError(FsdErrorEvent evt) { }

    void OnFsdConnectionStateChange(FsdConnectionStateChangeEvent evt) { }

    void OnFsdConnected(FsdConnectedEvent evt) { }

    void OnFsdDisconnected(FsdDisconnectedEvent evt) { }

    void OnFsdConnectionModelUpdated(FsdConnectionModelUpdatedEvent evt) { }
}
