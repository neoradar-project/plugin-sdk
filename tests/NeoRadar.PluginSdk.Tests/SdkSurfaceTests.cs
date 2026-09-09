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
            "Settings",
            "Menu",
            "Theme",
        };

        string[] actual = typeof(ICoreApi).GetProperties(Declared).Select(p => p.Name).ToArray();

        Assert.Equal(20, expected.Length);
        Assert.Equal(
            expected.OrderBy(n => n, StringComparer.Ordinal),
            actual.OrderBy(n => n, StringComparer.Ordinal));
    }

    [Fact]
    public void SettingsApi_ExposesBothScopesAndTheActiveProfile()
    {
        string[] methods = typeof(Settings.ISettingsApi).GetMethods(Declared)
            .Where(m => !m.IsSpecialName)
            .Select(m => m.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();
        string[] properties = typeof(Settings.ISettingsApi).GetProperties(Declared)
            .Select(p => p.Name)
            .ToArray();

        Assert.Equal(
            new[] { "Get", "GetForProfile", "Keys", "Remove", "RemoveForProfile", "Set", "SetForProfile" },
            methods);
        Assert.Equal(new[] { "ActiveProfile" }, properties);
    }

    [Fact]
    public void MenuApi_ExposesRegisterUpdateAndRemove()
    {
        string[] methods = typeof(Menu.IMenuApi).GetMethods(Declared)
            .Select(m => m.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(new[] { "RegisterMenuItem", "RemoveMenuItem", "UpdateMenuItem" }, methods);
    }

    // The palette is pulled at call time, never pushed, so a plugin cannot hold a stale copy
    // across a theme change.
    [Fact]
    public void ThemeApi_ExposesGetPaletteOnly()
    {
        MethodInfo[] methods = typeof(Theme.IThemeApi).GetMethods(Declared);

        Assert.Equal(new[] { "GetPalette" }, methods.Select(m => m.Name).ToArray());
        Assert.Equal(typeof(Theme.ThemePalette), methods[0].ReturnType);
        Assert.Empty(methods[0].GetParameters());
    }

    // The mutation members let a plugin rebuild a window in place instead of closing and reopening
    // it; each one needs a host command and an ABI entry.
    [Fact]
    public void WindowInterface_DeclaresTheMutationMembers()
    {
        string[] names = typeof(Window.IWindowInterface).GetMethods(Declared)
            .Select(m => m.Name)
            .ToArray();

        Assert.Contains("SetElementVisible", names);
        Assert.Contains("SetElementEnabled", names);
        Assert.Contains("SetElementStyle", names);
        Assert.Contains("ReplaceChildren", names);
        Assert.Contains("AppendChild", names);
        Assert.Contains("RemoveChild", names);
        Assert.Contains("SetSelectedIndex", names);
        Assert.Contains("SetChecked", names);
        Assert.Contains("SetOptions", names);
        Assert.Contains("SetTableRows", names);
        Assert.Contains("SetTableSelection", names);
    }

    [Fact]
    public void WindowInterface_MutationMembersTakeAHandleAndReturnBool()
    {
        string[] mutations =
        {
            "SetElementVisible",
            "SetElementEnabled",
            "SetElementStyle",
            "ReplaceChildren",
            "AppendChild",
            "RemoveChild",
            "SetSelectedIndex",
            "SetChecked",
            "SetOptions",
            "SetTableRows",
            "SetTableSelection",
        };

        foreach (string name in mutations)
        {
            MethodInfo method = typeof(Window.IWindowInterface).GetMethod(name)!;

            Assert.Equal(typeof(bool), method.ReturnType);
            Assert.Equal(typeof(Window.WindowHandle), method.GetParameters()[0].ParameterType);
            Assert.Equal(typeof(string), method.GetParameters()[1].ParameterType);
        }
    }

    [Fact]
    public void BasePlugin_DeclaresTheProfileTimerAndMenuHandlers()
    {
        Assert.Equal(
            typeof(Settings.ActiveProfileChangedEvent),
            typeof(BasePlugin).GetMethod(nameof(BasePlugin.OnActiveProfileChanged))!.GetParameters()[0].ParameterType);
        Assert.Equal(
            typeof(Timer.TimerEvent),
            typeof(BasePlugin).GetMethod(nameof(BasePlugin.OnTimer))!.GetParameters()[0].ParameterType);
        Assert.Equal(
            typeof(Menu.MenuItemClickedEvent),
            typeof(BasePlugin).GetMethod(nameof(BasePlugin.OnMenuItemClicked))!.GetParameters()[0].ParameterType);
    }

    // The values cross the function-pointer ABI as a single byte, so they are a wire contract
    // with the client, not just a managed enum.
    [Fact]
    public void TextAlign_ValuesArePinnedToTheWireBytes()
    {
        Assert.Equal(typeof(byte), Enum.GetUnderlyingType(typeof(Rendering.TextAlign)));
        Assert.Equal(0, (byte)Rendering.TextAlign.Left);
        Assert.Equal(1, (byte)Rendering.TextAlign.Center);
        Assert.Equal(2, (byte)Rendering.TextAlign.Right);
        Assert.Equal(
            new[] { "Left", "Center", "Right" },
            Enum.GetNames<Rendering.TextAlign>());
    }

    // Both text calls gained align as a trailing optional parameter, so plugins compiled against
    // the previous signature still bind.
    [Theory]
    [InlineData(nameof(Rendering.IPluginGraphics.AddScreenText))]
    [InlineData(nameof(Rendering.IPluginGraphics.AddWorldText))]
    public void TextAdders_TakeAnOptionalTrailingAlign(string methodName)
    {
        MethodInfo method = typeof(Rendering.IPluginGraphics).GetMethod(methodName)!;
        ParameterInfo[] parameters = method.GetParameters();
        ParameterInfo align = parameters[^1];
        ParameterInfo fontSize = parameters[^2];

        Assert.Equal("align", align.Name);
        Assert.Equal(typeof(Rendering.TextAlign), align.ParameterType);
        Assert.True(align.HasDefaultValue);
        Assert.Equal(Rendering.TextAlign.Left, (Rendering.TextAlign)align.DefaultValue!);

        Assert.Equal("fontSize", fontSize.Name);
        Assert.True(fontSize.HasDefaultValue);
        Assert.Equal(12f, (float)fontSize.DefaultValue!);
    }

    // Plugins centred text by guessing an em width before this existed; the host measures the
    // real glyphs instead.
    [Fact]
    public void PluginGraphics_MeasuresScreenTextSynchronously()
    {
        MethodInfo method = typeof(Rendering.IPluginGraphics)
            .GetMethod(nameof(Rendering.IPluginGraphics.MeasureScreenText))!;

        Assert.Equal(typeof(System.Numerics.Vector2), method.ReturnType);
        Assert.Equal(
            new[] { typeof(string), typeof(float) },
            method.GetParameters().Select(p => p.ParameterType).ToArray());
        Assert.Equal(new[] { "text", "fontSize" }, method.GetParameters().Select(p => p.Name).ToArray());
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
