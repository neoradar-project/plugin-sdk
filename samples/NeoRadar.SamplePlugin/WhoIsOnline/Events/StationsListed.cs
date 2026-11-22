using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.WhoIsOnline.Events;

/// <summary>The who-is-online demo listed the FSD connection, the controllers online and recent network activity.</summary>
internal sealed record StationsListed(IReadOnlyList<string> Lines);
