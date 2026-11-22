using System.Collections.Generic;
using NeoRadar.PluginSdk.Expressions;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeExpressionApi : IExpressionApi
{
    public List<ExpressionDefinition> Declared { get; } = new();

    public List<string> Pushes { get; } = new();

    public string Declare(ExpressionDefinition definition)
    {
        if (Declared.Exists(d => d.Name == definition.Name)) return string.Empty;
        Declared.Add(definition);
        return "plugin:Sample:EXP_" + definition.Name;
    }

    public bool Push(string expressionId, string callsign, bool value) => Record($"{expressionId} {callsign} {value}");

    public bool Push(string expressionId, string callsign, double value) => Record($"{expressionId} {callsign} {value}");

    public bool Push(string expressionId, string callsign, string value) => Record($"{expressionId} {callsign} {value}");

    public bool PushSession(string expressionId, bool value) => Record($"{expressionId} session {value}");

    public bool PushSession(string expressionId, double value) => Record($"{expressionId} session {value}");

    public bool PushSession(string expressionId, string value) => Record($"{expressionId} session {value}");

    private bool Record(string push)
    {
        Pushes.Add(push);
        return true;
    }
}
