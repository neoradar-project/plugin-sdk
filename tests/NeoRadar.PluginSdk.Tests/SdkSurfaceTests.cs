using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace NeoRadar.PluginSdk.Tests;

/// <summary>
/// Shape tests for the interfaces the host implements. A member added or removed here is a
/// contract change that has to reach the client adapters and the SDK version in the same commit.
/// </summary>
public sealed class SdkSurfaceTests
{
    private const BindingFlags Declared = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    // The provider is async-only: a synchronous GenerateSquawk would block the host's allocation
    // path and earn a watchdog strike.
    [Fact]
    public void SquawkProvider_ExposesTheAsyncMethodAndNothingElse()
    {
        MethodInfo[] methods = typeof(Squawk.ISquawkProvider).GetMethods(Declared);
        string[] plainMethods = methods
            .Where(m => !m.IsSpecialName)
            .Select(m => m.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();
        string[] properties = typeof(Squawk.ISquawkProvider).GetProperties(Declared)
            .Select(p => p.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(new[] { "GenerateSquawkAsync" }, plainMethods);
        Assert.Equal(new[] { "Priority", "ProviderName" }, properties);
    }

    [Fact]
    public void SquawkProvider_GenerateSquawkAsync_TakesACancellationToken()
    {
        MethodInfo method = typeof(Squawk.ISquawkProvider).GetMethod(nameof(Squawk.ISquawkProvider.GenerateSquawkAsync))!;
        Type[] parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();

        Assert.Equal(typeof(System.Threading.Tasks.Task<string>), method.ReturnType);
        Assert.Equal(
            new[]
            {
                typeof(string),
                typeof(Aircraft.Aircraft),
                typeof(Flightplan.Flightplan),
                typeof(System.Threading.CancellationToken),
            },
            parameters);
    }

    [Fact]
    public void CoreApi_ExposesEverySubApi()
    {
        string[] expected =
        {
            "Package",
            "Aircraft",
            "Flightplan",
            "Controller",
            "ControllerData",
            "Airport",
            "Squawk",
            "Tag",
            "Logger",
            "Fsd",
            "Chat",
            "Rendering",
            "Window",
            "Sound",
            "Focus",
            "Navdata",
            "Feedback",
        };

        string[] actual = typeof(ICoreApi).GetProperties(Declared).Select(p => p.Name).ToArray();

        Assert.Equal(17, expected.Length);
        Assert.Equal(
            expected.OrderBy(n => n, StringComparer.Ordinal),
            actual.OrderBy(n => n, StringComparer.Ordinal));
    }

    // The retained-mode IPluginGraphics is the only drawing path; the legacy queue-based Draw*
    // members silently did nothing and are gone.
    [Fact]
    public void RenderingInterface_DeclaresNoLegacyDrawMembers()
    {
        IEnumerable<string> members = typeof(Rendering.IRenderingInterface)
            .GetMembers(Declared)
            .Select(m => m.Name)
            .Where(n => n.StartsWith("Draw", StringComparison.Ordinal));

        Assert.Empty(members);
    }
}
