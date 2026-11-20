using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Window;
using Xunit;

namespace NeoRadar.PluginSdk.Tests;

// Plugin authors write these records and the host serialises them, so a changed default is a contract change.
public sealed class WindowModelTests
{
    [Fact]
    public void WindowDefinition_DefaultsTheFrameToNonResizableWithNoDefaultButtons()
    {
        WindowDefinition definition = new("Title");

        Assert.Equal(300, definition.Width);
        Assert.Equal(200, definition.Height);
        Assert.False(definition.Resizable);
        Assert.Equal(0, definition.MinWidth);
        Assert.Equal(0, definition.MinHeight);
        Assert.Equal(string.Empty, definition.DefaultButtonId);
        Assert.Equal(string.Empty, definition.CancelButtonId);
    }

    // The first six parameters keep their order so plugins built positionally still compile.
    [Fact]
    public void WindowDefinition_KeepsThePositionalOrderOfTheOriginalParameters()
    {
        WindowElement[] elements = { new() { Id = "root" } };
        WindowDefinition definition = new("Title", 640, 480, "icon", true, elements, true, 320, 240, "ok", "cancel");

        Assert.Equal("Title", definition.Title);
        Assert.Equal(640, definition.Width);
        Assert.Equal(480, definition.Height);
        Assert.Equal("icon", definition.Icon);
        Assert.True(definition.ShowInList);
        Assert.Same(elements, definition.Elements);
        Assert.True(definition.Resizable);
        Assert.Equal(320, definition.MinWidth);
        Assert.Equal(240, definition.MinHeight);
        Assert.Equal("ok", definition.DefaultButtonId);
        Assert.Equal("cancel", definition.CancelButtonId);
    }

    [Fact]
    public void WindowElement_OptionsDefaultsToAnEmptyList()
    {
        WindowElement element = new();

        Assert.NotNull(element.Options);
        Assert.Empty(element.Options);
        Assert.False(element.Clickable);
        Assert.Null(element.Table);
    }

    [Fact]
    public void WindowElement_OptionsIsAListOfStrings()
    {
        WindowElement element = new() { Type = ElementType.Select, Options = new[] { "EGLL", "EGKK" }, SelectedIndex = 1 };

        Assert.Equal(new[] { "EGLL", "EGKK" }, element.Options);
        Assert.Equal(1, element.SelectedIndex);
    }

    [Fact]
    public void ElementStyle_RoleDefaultsToDefault()
        => Assert.Equal(ElementRole.Default, new ElementStyle().Role);

    [Fact]
    public void ElementType_TableIs24()
        => Assert.Equal(24, (int)ElementType.Table);

    [Fact]
    public void TableColumn_DefaultsToAStarWidthAndLeftAlignment()
    {
        TableColumn column = new("callsign", "Callsign");

        Assert.Equal("callsign", column.Id);
        Assert.Equal("Callsign", column.Title);
        Assert.Equal("*", column.Width);
        Assert.Equal(HAlign.Left, column.Align);
        Assert.Equal(TableColumnKind.Text, column.Kind);
    }

    [Fact]
    public void TableColumn_TakesTheKindAsItsTrailingParameter()
    {
        TableColumn column = new("assumed", "Assumed", "40", HAlign.Center, TableColumnKind.Check);

        Assert.Equal(TableColumnKind.Check, column.Kind);
        Assert.Equal(HAlign.Center, column.Align);
    }

    [Fact]
    public void TableColumnKind_PinsItsWireValues()
    {
        Assert.Equal(0, (int)TableColumnKind.Text);
        Assert.Equal(1, (int)TableColumnKind.Check);
        Assert.Equal(2, (int)TableColumnKind.Button);
    }

    [Fact]
    public void TableRow_DefaultsToNoStyle()
    {
        TableRow row = new("BAW123", new[] { "BAW123", "EGLL" });

        Assert.Equal("BAW123", row.Id);
        Assert.Equal(new[] { "BAW123", "EGLL" }, row.Cells);
        Assert.Null(row.Style);
    }

    [Fact]
    public void TableDefinition_DefaultsToNoSelectionAndAVisibleHeader()
    {
        TableDefinition table = new(Array.Empty<TableColumn>(), Array.Empty<TableRow>());

        Assert.Equal(-1, table.SelectedIndex);
        Assert.True(table.ShowHeader);
    }

    [Fact]
    public void TableDefinition_HangsOffTheElementAsAWholeTable()
    {
        IReadOnlyList<TableColumn> columns = new[] { new TableColumn("id", "Id", "120", HAlign.Right) };
        IReadOnlyList<TableRow> rows = new[] { new TableRow("r1", new[] { "1" }, new ElementStyle { Role = ElementRole.Warning }) };
        WindowElement element = new()
        {
            Id = "table",
            Type = ElementType.Table,
            Table = new TableDefinition(columns, rows, SelectedIndex: 0, ShowHeader: false),
        };

        Assert.NotNull(element.Table);
        Assert.Same(columns, element.Table!.Columns);
        Assert.Same(rows, element.Table.Rows);
        Assert.Equal(0, element.Table.SelectedIndex);
        Assert.False(element.Table.ShowHeader);
        Assert.Equal(ElementRole.Warning, element.Table.Rows[0].Style!.Role);
    }
}
