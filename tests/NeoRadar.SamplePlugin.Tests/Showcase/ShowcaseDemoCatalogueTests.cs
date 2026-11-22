using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NeoRadar.PluginSdk;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Showcase;
using NeoRadar.SamplePlugin.Tests.Common;
using NeoRadar.SamplePlugin.Tests.Fakes;
using NeoRadar.SamplePlugin.TrackWrites;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Showcase;

public sealed class ShowcaseDemoCatalogueTests
{
    [Fact]
    public void EveryButtonAndInput_NamesARegisteredDemo()
    {
        using ServiceProvider provider = FeatureRegistrationTests.Compose();

        IReadOnlyCollection<string> registered = provider.GetRequiredService<ShowcaseDemoCatalogue>().Ids;

        Assert.All(Offered(), id => Assert.Contains(id, registered));
    }

    [Fact]
    public void EveryRegisteredDemo_IsOfferedByAButtonOrAnInput()
    {
        using ServiceProvider provider = FeatureRegistrationTests.Compose();
        HashSet<string> offered = Offered().ToHashSet();

        Assert.All(provider.GetRequiredService<ShowcaseDemoCatalogue>().Ids, id => Assert.Contains(id, offered));
    }

    [Fact]
    public void TwoDemosWithOneId_FailAtComposition_NamingTheId()
    {
        ServiceCollection services = new();
        services.AddSamplePlugin(new FakeCoreApi(), new ClientInformation("Test", "0.0.0", @"C:\NeoRadar"));
        services.AddSingleton<IShowcaseDemos>(new ProbeDemos("Assume"));
        using ServiceProvider provider = services.BuildServiceProvider();

        InvalidOperationException refused = Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<FeatureHost>());

        Assert.Contains("Assume", refused.Message);
    }

    [Fact]
    public void AddingDemosTwice_FailsAtComposition()
    {
        ServiceCollection services = new();
        services.AddShowcaseDemos<TrackWritesDemos>();

        InvalidOperationException refused = Assert.Throws<InvalidOperationException>(() => services.AddShowcaseDemos<TrackWritesDemos>());

        Assert.Contains(nameof(TrackWritesDemos), refused.Message);
    }

    private static IEnumerable<string> Offered()
        => ShowcaseGuide.Pages.SelectMany(p => p.Buttons.Select(b => b.DemoId))
            .Concat(ShowcaseGuide.Pages.Where(p => p.Input is not null).Select(p => p.Input!.DemoId));

    private sealed class ProbeDemos(string id) : IShowcaseDemos
    {
        public IReadOnlyList<ShowcaseDemo> Demos => [new(id, () => { })];

        public IReadOnlyList<IDisposable> ShowReports() => [];
    }
}
