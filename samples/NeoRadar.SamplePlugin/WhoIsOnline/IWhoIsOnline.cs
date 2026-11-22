namespace NeoRadar.SamplePlugin.WhoIsOnline;

/// <summary>What other features may ask of the who-is-online demo.</summary>
internal interface IWhoIsOnline
{
    void Refresh();

    /// <summary>Posts one line to this client's chat on the plugin's own channel of that name; nothing reaches the network.</summary>
    void PostOnChannel(string channel);

    /// <summary>Posts a burst of lines at once on the plugin's own channel, to show how many the host's rate limit takes.</summary>
    void PostBurst();
}
