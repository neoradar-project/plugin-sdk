using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Logger;
using NeoRadar.PluginSdk.Window;

namespace NeoRadar.SamplePlugin.Common;

internal sealed class FeatureHost
{
    private readonly List<IFeature> _features;
    private readonly ILoggerApi _logger;
    private bool _initialized;

    public FeatureHost(ILoggerApi logger, IEnumerable<IFeature> features)
    {
        _logger = logger;
        _features = new List<IFeature>(features);
    }

    public IReadOnlyList<IFeature> Features => _features;

    public void Initialize(PluginContext context)
    {
        if (_initialized) return;
        _initialized = true;

        for (int i = 0; i < _features.Count; i++)
            Run(() => _features[i].Initialize(context), _features[i], "Initialize");
    }

    public void Shutdown()
    {
        if (!_initialized) return;
        _initialized = false;

        for (int i = _features.Count - 1; i >= 0; i--)
            Run(() => _features[i].Shutdown(), _features[i], "Shutdown");
    }

    public void Dispatch(Action<IFeature> handler, string source)
    {
        for (int i = 0; i < _features.Count; i++)
            Run(() => handler(_features[i]), _features[i], source);
    }

    private void Run(Action work, IFeature feature, string label)
    {
        try { work(); }
        catch (Exception ex)
        {
            _logger.Error($"[{feature.GetType().Name}.{label}] {ex.GetType().Name}: {ex.Message}");
        }
    }
}
