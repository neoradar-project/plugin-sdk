using System;
using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>A feature's demos on the Showcase, and the readouts it shows there; registered by AddShowcaseDemos.</summary>
internal interface IShowcaseDemos
{
    IReadOnlyList<ShowcaseDemo> Demos { get; }

    /// <summary>Shows what the feature reports on its Showcase page until each returned handle is disposed.</summary>
    IReadOnlyList<IDisposable> ShowReports();
}
