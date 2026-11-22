using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Logger;

namespace NeoRadar.SamplePlugin.Common.Events;

internal sealed class PluginEventBus : IPluginEventBus
{
    private readonly ILoggerApi _logger;
    private readonly object _sync = new();
    private readonly Dictionary<Type, List<object>> _subscribers = new();

    public PluginEventBus(ILoggerApi logger) => _logger = logger;

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
    {
        lock (_sync)
        {
            if (!_subscribers.TryGetValue(typeof(TEvent), out List<object>? handlers))
            {
                handlers = new List<object>();
                _subscribers[typeof(TEvent)] = handlers;
            }
            handlers.Add(handler);
        }
        return new Subscription(this, typeof(TEvent), handler);
    }

    public void Publish<TEvent>(TEvent evt)
    {
        object[] snapshot;
        lock (_sync)
        {
            if (!_subscribers.TryGetValue(typeof(TEvent), out List<object>? handlers) || handlers.Count == 0) return;
            snapshot = handlers.ToArray();
        }

        foreach (object subscriber in snapshot)
        {
            Action<TEvent> handler = (Action<TEvent>)subscriber;
            try
            {
                handler(evt);
            }
            catch (Exception ex)
            {
                _logger.Error($"[bus {typeof(TEvent).Name} -> {Describe(handler)}] {ex}");
            }
        }
    }

    private static string Describe<TEvent>(Action<TEvent> handler)
        => handler.Target?.GetType().Name ?? handler.Method.DeclaringType?.Name ?? handler.Method.Name;

    private void Unsubscribe(Type eventType, object handler)
    {
        lock (_sync)
        {
            if (_subscribers.TryGetValue(eventType, out List<object>? handlers))
                handlers.Remove(handler);
        }
    }

    private sealed class Subscription : IDisposable
    {
        private readonly PluginEventBus _bus;
        private readonly Type _eventType;
        private readonly object _handler;
        private bool _disposed;

        public Subscription(PluginEventBus bus, Type eventType, object handler)
        {
            _bus = bus;
            _eventType = eventType;
            _handler = handler;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _bus.Unsubscribe(_eventType, _handler);
        }
    }
}
