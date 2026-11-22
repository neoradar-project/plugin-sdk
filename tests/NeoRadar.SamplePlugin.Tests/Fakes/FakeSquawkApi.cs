using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Squawk;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeSquawkApi : ISquawkApi
{
    public List<ISquawkProvider> Providers { get; } = new();

    public string? Active { get; private set; }

    public bool RegisterProvider(ISquawkProvider provider)
    {
        if (Providers.Exists(p => string.Equals(p.ProviderName, provider.ProviderName, StringComparison.OrdinalIgnoreCase))) return false;
        Providers.Add(provider);
        return true;
    }

    public IDisposable? RegisterProviderWithToken(ISquawkProvider provider)
        => RegisterProvider(provider) ? new Registration(this, provider) : null;

    public bool SetActiveProvider(string providerName)
    {
        if (!Providers.Exists(p => string.Equals(p.ProviderName, providerName, StringComparison.OrdinalIgnoreCase))) return false;
        Active = providerName;
        return true;
    }

    public IReadOnlyList<string> GetAvailableProviders() => Providers.ConvertAll(p => p.ProviderName);

    private sealed class Registration(FakeSquawkApi owner, ISquawkProvider provider) : IDisposable
    {
        public void Dispose() => owner.Providers.Remove(provider);
    }
}
