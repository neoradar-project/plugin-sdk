using System;
using System.Linq;
using System.Reflection;
using NeoRadar.PluginSdk.Expressions;
using Xunit;

namespace NeoRadar.PluginSdk.Tests;

/// <summary>
/// A definition's default is held in the form a package condition compares, and only the three
/// typed factories can build one.
/// </summary>
public sealed class ExpressionDefinitionTests
{
    [Theory]
    [InlineData(true, "1")]
    [InlineData(false, "0")]
    public void Boolean_HoldsItsDefaultAsOneOrZero(bool defaultValue, string expected)
    {
        ExpressionDefinition definition = ExpressionDefinition.Boolean("On Stand", defaultValue, "has a stand");

        Assert.Equal("On Stand", definition.Name);
        Assert.Equal(ExpressionValueType.Boolean, definition.Type);
        Assert.Equal(expected, definition.DefaultValue);
        Assert.Equal("has a stand", definition.Description);
        Assert.Equal(ExpressionScope.Aircraft, definition.Scope);
    }

    [Theory]
    [InlineData(0d, "0")]
    [InlineData(5d, "5")]
    [InlineData(-3.5d, "-3.5")]
    [InlineData(1234567.25d, "1234567.25")]
    public void Number_HoldsItsDefaultInInvariantForm_WhateverTheCulture(double defaultValue, string expected)
    {
        System.Globalization.CultureInfo before = System.Globalization.CultureInfo.CurrentCulture;
        System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
        try
        {
            ExpressionDefinition definition = ExpressionDefinition.Number("Delay", defaultValue, "minutes");

            Assert.Equal(ExpressionValueType.Number, definition.Type);
            Assert.Equal(expected, definition.DefaultValue);
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = before;
        }
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Number_RefusesADefaultThatIsNotFinite(double defaultValue)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ExpressionDefinition.Number("Delay", defaultValue, ""));
    }

    [Fact]
    public void Text_HoldsItsDefaultAsWritten_AndANullAsEmpty()
    {
        Assert.Equal("NONE", ExpressionDefinition.Text("Stand", "NONE", "").DefaultValue);
        Assert.Equal("", ExpressionDefinition.Text("Stand", null!, "").DefaultValue);
        Assert.Equal(ExpressionValueType.Text, ExpressionDefinition.Text("Stand", "", "").Type);
    }

    [Fact]
    public void Scope_IsTheOnlyThingAWithExpressionCanChange()
    {
        ExpressionDefinition session = ExpressionDefinition.Boolean("Low Visibility", false, "") with { Scope = ExpressionScope.Session };

        Assert.Equal(ExpressionScope.Session, session.Scope);
        string[] settable = typeof(ExpressionDefinition).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.SetMethod is { IsPublic: true })
            .Select(p => p.Name)
            .ToArray();
        Assert.Equal(new[] { "Scope" }, settable);
    }

    [Fact]
    public void Definition_HasNoPublicConstructor_SoADefaultIsAlwaysOfTheDeclaredType()
    {
        Assert.Empty(typeof(ExpressionDefinition).GetConstructors(BindingFlags.Public | BindingFlags.Instance));
    }
}
