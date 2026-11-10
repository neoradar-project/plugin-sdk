using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace NeoRadar.PluginSdk.Tests;

/// <summary>
/// A plugin overrides only the handlers it needs, so every virtual on <see cref="BasePlugin"/>
/// must have a body the host can call unconditionally.
/// </summary>
public sealed class BasePluginDefaultsTests
{
    private sealed class MinimalPlugin : BasePlugin
    {
        public override PluginManifest Manifest => new(
            "minimal",
            "tests",
            PluginSdkVersion.Current,
            PluginSdkVersion.Major);

        public override void Initialize(PluginManifest metadata, ICoreApi coreApi, ClientInformation clientInfo)
        {
        }
    }

    [Fact]
    public void EveryVirtual_HasADefaultBodyThatDoesNotThrow()
    {
        MinimalPlugin plugin = new();
        MethodInfo[] virtuals = typeof(BasePlugin)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.IsVirtual && !m.IsAbstract)
            .ToArray();

        Assert.NotEmpty(virtuals);

        foreach (MethodInfo method in virtuals)
        {
            object?[] args = method.GetParameters()
                .Select(p => CreateDefault(p.ParameterType))
                .ToArray();
            try
            {
                method.Invoke(plugin, args);
            }
            catch (TargetInvocationException ex)
            {
                Assert.Fail($"{method.Name} threw {ex.InnerException?.GetType().Name}: {ex.InnerException?.Message}");
            }
        }
    }

    [Fact]
    public void EveryEventHandler_IsCoveredByTheDefaultsSweep()
    {
        string[] names = typeof(BasePlugin)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.IsVirtual && !m.IsAbstract)
            .Select(m => m.Name)
            .ToArray();

        Assert.Contains(nameof(BasePlugin.OnAircraftHandoffInitiated), names);
        Assert.Contains(nameof(BasePlugin.OnSelectedAircraftChanged), names);
        Assert.Contains(nameof(BasePlugin.OnPrivateMessageReceived), names);
        Assert.Contains(nameof(BasePlugin.OnFrequencyMessageReceived), names);
        Assert.Contains(nameof(BasePlugin.OnBroadcastMessageReceived), names);
        Assert.Contains(nameof(BasePlugin.OnSupervisorMessageReceived), names);
        Assert.Contains(nameof(BasePlugin.OnServerMessageReceived), names);
        Assert.Contains(nameof(BasePlugin.OnAtcMessageReceived), names);
        Assert.Contains(nameof(BasePlugin.OnActiveProfileChanged), names);
        Assert.Contains(nameof(BasePlugin.OnTimer), names);
        Assert.Contains(nameof(BasePlugin.OnMenuItemClicked), names);
    }

    // The sweep builds every payload by reflection, so a new event type has to be constructible
    // from defaults: a null profile name, tick zero, an empty item id.
    [Fact]
    public void ProfileTimerAndMenuPayloads_BuildFromDefaults()
    {
        MinimalPlugin plugin = new();
        Settings.ActiveProfileChangedEvent profile = new(null);
        Timer.TimerEvent tick = new(0L);
        Menu.MenuItemClickedEvent click = new(string.Empty, false);

        plugin.OnActiveProfileChanged(profile);
        plugin.OnTimer(tick);
        plugin.OnMenuItemClicked(click);

        Assert.Null(profile.ProfileName);
        Assert.Equal(0L, tick.Tick);
        Assert.False(click.Checked);
    }

    [Fact]
    public void OnTagShowDropdown_DefaultsToShowing()
        => Assert.True(new MinimalPlugin().OnTagShowDropdown("ACTION_X", "BAW123"));

    /// <summary>Builds a zeroed instance of an event payload: empty strings, empty collections,
    /// null for anything nullable.</summary>
    private static object? CreateDefault(Type type)
    {
        if (Nullable.GetUnderlyingType(type) is not null)
        {
            return null;
        }
        if (type == typeof(string))
        {
            return string.Empty;
        }
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        if (type.IsArray)
        {
            return Array.CreateInstance(type.GetElementType()!, 0);
        }
        if (type.IsGenericType)
        {
            Type definition = type.GetGenericTypeDefinition();
            Type[] arguments = type.GetGenericArguments();
            if (definition == typeof(IReadOnlyList<>)
                || definition == typeof(IList<>)
                || definition == typeof(ICollection<>)
                || definition == typeof(IEnumerable<>)
                || definition == typeof(List<>))
            {
                return Array.CreateInstance(arguments[0], 0);
            }
            if (definition == typeof(IReadOnlyDictionary<,>)
                || definition == typeof(IDictionary<,>)
                || definition == typeof(Dictionary<,>))
            {
                return Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(arguments));
            }
        }

        ConstructorInfo constructor = type.GetConstructors()
            .OrderBy(c => c.GetParameters().Length)
            .FirstOrDefault()
            ?? throw new InvalidOperationException($"No public constructor on {type.FullName}.");
        object?[] args = constructor.GetParameters()
            .Select(p => CreateDefault(p.ParameterType))
            .ToArray();
        return constructor.Invoke(args);
    }
}
