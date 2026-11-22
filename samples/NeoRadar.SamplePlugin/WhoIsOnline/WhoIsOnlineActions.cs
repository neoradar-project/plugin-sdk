using System.Collections.Generic;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.PluginSdk.Controller;
using NeoRadar.PluginSdk.Fsd;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.WhoIsOnline.Events;

namespace NeoRadar.SamplePlugin.WhoIsOnline;

/// <summary>Lists who is online with the recent network activity, and posts on the plugin's own chat channels.</summary>
internal sealed class WhoIsOnlineActions : IWhoIsOnline
{
    public const int BurstSize = 7;
    public const string BurstChannel = "Sample";

    private readonly IControllerApi _controllers;
    private readonly IFsdApi _fsd;
    private readonly IChatApi _chat;
    private readonly IPluginEventBus _bus;
    private readonly NetworkActivity _activity;

    public WhoIsOnlineActions(IControllerApi controllers, IFsdApi fsd, IChatApi chat, IPluginEventBus bus, NetworkActivity activity)
    {
        _controllers = controllers;
        _fsd = fsd;
        _chat = chat;
        _bus = bus;
        _activity = activity;
    }

    public void Refresh()
    {
        List<string> lines = StationSummaryFormatter.Describe(_fsd.GetConnection(), _controllers.GetAll());
        lines.Add("Recent:");
        lines.AddRange(_activity.Snapshot());
        _bus.Publish(new StationsListed(lines));
    }

    public void PostOnChannel(string channel)
    {
        bool posted = _chat.SendClientMessage(new ClientTextMessageEvent("Sample", $"Hello on [{channel}]", Channel: channel));
        _activity.Note($"Post on [{channel}]: {(posted ? "posted" : "refused (a reserved name, a ninth channel, or the rate limit)")}");
        Refresh();
    }

    public void PostBurst()
    {
        List<string> results = new();
        int posted = 0;
        for (int i = 1; i <= BurstSize; i++)
        {
            bool sent = _chat.SendClientMessage(new ClientTextMessageEvent("Sample", $"Burst line {i} of {BurstSize}", Channel: BurstChannel));
            if (sent) posted++;
            results.Add($"Line {i}: {(sent ? "posted" : "refused")}");
        }

        results.Insert(0, $"Posted {posted} of {BurstSize} lines at once; the host refused {BurstSize - posted}");
        _bus.Publish(new ChatBurstPosted(results));
    }
}
