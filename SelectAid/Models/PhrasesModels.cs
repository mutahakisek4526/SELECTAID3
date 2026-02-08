using System.Collections.Generic;

namespace SelectAid.Models;

public sealed class PhrasesDocument
{
    public int Version { get; set; } = 1;
    public List<PhraseScene> Scenes { get; set; } = new();
}

public sealed class PhraseScene
{
    public string Name { get; set; } = string.Empty;
    public List<PhraseCategory> Categories { get; set; } = new();
}

public sealed class PhraseCategory
{
    public string Name { get; set; } = string.Empty;
    public List<PhraseItem> Items { get; set; } = new();
}

public sealed class PhraseItem
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
