namespace NeoRadar.SamplePlugin.EventLog;

/// <summary>What other features may ask of the event log.</summary>
internal interface IEventLog
{
    void WriteEachLogLevel();
}
