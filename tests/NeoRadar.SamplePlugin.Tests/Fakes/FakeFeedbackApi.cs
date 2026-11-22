using System.Collections.Generic;
using NeoRadar.PluginSdk.Feedback;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeFeedbackApi : IFeedbackApi
{
    public List<(string Text, FeedbackKind Kind)> Shown { get; } = new();

    public void Show(string text, FeedbackKind kind = FeedbackKind.Info, int durationMs = 2000) => Shown.Add((text, kind));
}
