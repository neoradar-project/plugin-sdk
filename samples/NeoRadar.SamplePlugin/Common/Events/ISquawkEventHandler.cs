using NeoRadar.PluginSdk.Squawk;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Squawk assignment events, on client threads.</summary>
internal interface ISquawkEventHandler
{
    void OnSquawkAssigned(SquawkAssignedEvent evt) { }
}
