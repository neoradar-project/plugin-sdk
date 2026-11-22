using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.WhoIsOnline.Events;

/// <summary>The who-is-online demo posted a burst of chat lines; the lines say how many the host took.</summary>
internal sealed record ChatBurstPosted(IReadOnlyList<string> Lines);
