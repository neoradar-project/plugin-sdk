using NeoRadar.PluginSdk.Controller;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Controller connect, disconnect, ATIS and capability events, on client threads.</summary>
internal interface IControllerEventHandler
{
    void OnControllerConnected(ControllerConnectedEvent evt) { }

    void OnControllerDisconnected(ControllerDisconnectedEvent evt) { }

    void OnAtisLinesUpdate(AtisLinesUpdateEvent evt) { }

    void OnCapabilitiesUpdate(CapabilitiesUpdateEvent evt) { }

    void OnIsControllerATC(IsControllerATCEvent evt) { }
}
