using System.Collections.Generic;

namespace SelectAid.Models;

public sealed class UserDictionary
{
    public int Version { get; set; } = 1;
    public List<UserWord> Words { get; set; } = new();
}

public sealed class UserWord
{
    public string Text { get; set; } = string.Empty;
    public int Count { get; set; }
}
