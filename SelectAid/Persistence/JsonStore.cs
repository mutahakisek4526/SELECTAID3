using System.IO;
using System.Text.Json;

namespace SelectAid.Persistence;

public static class JsonStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static T Load<T>(string path, T fallback) where T : class
    {
        if (!File.Exists(path))
        {
            return fallback;
        }

        var json = File.ReadAllText(path);
        var loaded = JsonSerializer.Deserialize<T>(json, Options);
        return loaded ?? fallback;
    }

    public static void Save<T>(string path, T data) where T : class
    {
        var json = JsonSerializer.Serialize(data, Options);
        File.WriteAllText(path, json);
    }
}
