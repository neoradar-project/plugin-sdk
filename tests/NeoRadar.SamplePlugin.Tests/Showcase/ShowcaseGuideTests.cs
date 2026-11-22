using System;
using System.Collections.Generic;
using System.Linq;
using NeoRadar.SamplePlugin.Showcase;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Showcase;

public sealed class ShowcaseGuideTests
{
    // The pages of the Plugins section of the NeoRadar docs, by title.
    private static readonly string[] s_docsPages =
    [
        "Plugins overview", "How plugins load", "SDK reference", "Tag items and expressions", "Writing a command",
        "Squawk providers", "Chat", "Rendering and screens", "Events and lifecycle", "Logging and diagnostics",
        "Getting started", "Publishing a plugin",
    ];

    [Fact]
    public void EveryPage_HasAUniqueId_ATitle_AOneLineSummary_TextInEachSection_AndADocsPageToReadNext()
    {
        Assert.Equal(ShowcaseGuide.Pages.Count, ShowcaseGuide.Pages.Select(p => p.Id).Distinct().Count());
        foreach (ShowcasePage page in ShowcaseGuide.Pages)
        {
            Assert.False(string.IsNullOrWhiteSpace(page.Title), page.Id);
            Assert.False(string.IsNullOrWhiteSpace(page.Summary), page.Id);
            Assert.DoesNotContain('\n', page.Summary);
            Assert.NotEmpty(page.Sections);
            Assert.All(page.Sections, s =>
            {
                Assert.False(string.IsNullOrWhiteSpace(s.Heading), page.Id);
                Assert.NotEmpty(s.Paragraphs);
            });
            Assert.Contains(page.ReadNext, s_docsPages);
            Assert.DoesNotContain('-', page.Id);
        }
    }

    [Fact]
    public void EveryPage_IsInExactlyOneNavGroup_AndPagesFollowTheNavOrder()
    {
        Assert.All(ShowcaseGuide.Groups, g =>
        {
            Assert.False(string.IsNullOrWhiteSpace(g.Heading));
            Assert.NotEmpty(g.Pages);
        });
        Assert.Equal(ShowcaseGuide.Groups.SelectMany(g => g.Pages), ShowcaseGuide.Pages);
        Assert.Equal(ShowcaseGuide.Start, ShowcaseGuide.Pages[0].Id);
    }

    [Fact]
    public void EveryDemoId_IsAWordWithNoDash_SoItsButtonIdReadsBack()
    {
        IEnumerable<string> ids = ShowcaseGuide.Pages.SelectMany(p => p.Buttons.Select(b => b.DemoId));
        ids = ids.Concat(ShowcaseGuide.Pages.Where(p => p.Input is not null).Select(p => p.Input!.DemoId));

        Assert.All(ids, id => Assert.Matches("^[A-Za-z0-9]+$", id));
    }

    [Fact]
    public void ANetworkButton_SaysWhatItSends_AndOnlyTheControllerDataAirportAndSquawkPagesHaveThem()
    {
        DemoButton[] network = ShowcaseGuide.Pages.SelectMany(p => p.Buttons).Where(b => b.SendsToNetwork).ToArray();

        Assert.All(network, b => Assert.False(string.IsNullOrWhiteSpace(b.Sends)));
        Assert.Equal(
            [ShowcaseGuide.ControllerData, ShowcaseGuide.Airports, ShowcaseGuide.Squawk],
            ShowcaseGuide.Pages.Where(p => p.Buttons.Any(b => b.SendsToNetwork)).Select(p => p.Id));
    }

    [Fact]
    public void TheSquawkRegisterButton_NeedsDemoWrites_AsTheProviderItAddsOutranksTheClientsOwn()
    {
        DemoButton register = ShowcaseGuide.Pages.SelectMany(p => p.Buttons).Single(b => b.DemoId == "RegisterSquawk");

        Assert.True(register.SendsToNetwork);
    }

    [Fact]
    public void NoText_UsesAnEmDash()
    {
        IEnumerable<string> text = ShowcaseGuide.Groups.Select(g => g.Heading)
            .Concat(ShowcaseGuide.Pages.SelectMany(p => p.Sections
                .SelectMany(s => s.Paragraphs.Append(s.Heading))
                .Append(p.Title)
                .Append(p.Summary)
                .Concat(p.Buttons.Select(b => b.Label + b.Sends))));

        Assert.DoesNotContain(text, t => t.Contains((char)0x2014));
    }
}
