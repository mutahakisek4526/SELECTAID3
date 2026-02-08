using System;
using System.IO;

namespace SelectAid.Services;

public static class AppPaths
{
    public static string AppDataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SelectAid");
    public static string SettingsFile => Path.Combine(AppDataDirectory, "settings.json");
    public static string ProfilesFile => Path.Combine(AppDataDirectory, "profiles.json");
    public static string KeyboardLayoutsFile => Path.Combine(AppDataDirectory, "keyboardLayouts.json");
    public static string PhrasesFile => Path.Combine(AppDataDirectory, "phrases.json");
    public static string UserDictFile => Path.Combine(AppDataDirectory, "userDict.json");
    public static string HistoryFile => Path.Combine(AppDataDirectory, "history.json");
    public static string LogFile => Path.Combine(AppDataDirectory, "log.txt");
    public static string BackupsDirectory => Path.Combine(AppDataDirectory, "backups");
    public static string MetricsFile => Path.Combine(AppDataDirectory, "metrics.csv");
}
