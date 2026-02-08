using System.Collections.Generic;

namespace SelectAid.Models;

public sealed class KeyboardLayoutsDocument
{
    public int Version { get; set; } = 1;
    public List<KeyboardLayout> Layouts { get; set; } = new();
}

public sealed class KeyboardLayout
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<KeyboardRow> Rows { get; set; } = new();
}

public sealed class KeyboardRow
{
    public List<KeyDefinition> Keys { get; set; } = new();
}

public sealed class KeyDefinition
{
    public string Label { get; set; } = string.Empty;
    public string OutputText { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Variant { get; set; } = string.Empty;
}
