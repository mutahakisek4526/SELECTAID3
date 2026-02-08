using System;
using System.Collections.Generic;
using System.IO;
using SelectAid.Models;
using SelectAid.Persistence;

namespace SelectAid.Services;

public sealed class PersistenceService
{
    public AppSettings Settings { get; private set; } = null!;
    public ProfilesDocument Profiles { get; private set; } = null!;
    public KeyboardLayoutsDocument KeyboardLayouts { get; set; } = null!;
    public PhrasesDocument Phrases { get; set; } = null!;
    public UserDictionary UserDict { get; private set; } = null!;
    public HistoryDocument History { get; private set; } = null!;

    public Profile CurrentProfile => Profiles.Profiles.Find(profile => profile.Id == Settings.CurrentProfileId) ?? Profiles.Profiles[0];

    public void Initialize()
    {
        Directory.CreateDirectory(AppPaths.AppDataDirectory);
        Directory.CreateDirectory(AppPaths.BackupsDirectory);

        EnsureDefaults();

        Settings = Migration.EnsureSettings(JsonStore.Load(AppPaths.SettingsFile, new AppSettings()));
        Profiles = Migration.EnsureProfiles(JsonStore.Load(AppPaths.ProfilesFile, new ProfilesDocument
        {
            Profiles = new List<Profile>
            {
                new Profile { Id = "default", Name = "Default", ThemeId = "Friendly" }
            }
        }));

        KeyboardLayouts = Migration.EnsureLayouts(JsonStore.Load(AppPaths.KeyboardLayoutsFile, LoadResource<KeyboardLayoutsDocument>("keyboardLayouts.json")));
        Phrases = Migration.EnsurePhrases(JsonStore.Load(AppPaths.PhrasesFile, LoadResource<PhrasesDocument>("phrases.json")));
        UserDict = Migration.EnsureUserDict(JsonStore.Load(AppPaths.UserDictFile, LoadResource<UserDictionary>("userDict.json")));
        History = Migration.EnsureHistory(JsonStore.Load(AppPaths.HistoryFile, new HistoryDocument()));

        SaveAll();
    }

    public void SaveAll()
    {
        JsonStore.Save(AppPaths.SettingsFile, Settings);
        JsonStore.Save(AppPaths.ProfilesFile, Profiles);
        JsonStore.Save(AppPaths.KeyboardLayoutsFile, KeyboardLayouts);
        JsonStore.Save(AppPaths.PhrasesFile, Phrases);
        JsonStore.Save(AppPaths.UserDictFile, UserDict);
        JsonStore.Save(AppPaths.HistoryFile, History);
    }

    public void SaveSettings()
    {
        JsonStore.Save(AppPaths.SettingsFile, Settings);
    }

    public void SavePhrases()
    {
        JsonStore.Save(AppPaths.PhrasesFile, Phrases);
    }

    public void SaveLayouts()
    {
        JsonStore.Save(AppPaths.KeyboardLayoutsFile, KeyboardLayouts);
    }

    public void SaveHistory()
    {
        JsonStore.Save(AppPaths.HistoryFile, History);
    }

    private static void EnsureDefaults()
    {
        var resourceDir = Path.Combine(AppContext.BaseDirectory, "Resources");
        if (!Directory.Exists(resourceDir))
        {
            return;
        }

        CopyIfMissing(resourceDir, AppPaths.KeyboardLayoutsFile, "keyboardLayouts.json");
        CopyIfMissing(resourceDir, AppPaths.PhrasesFile, "phrases.json");
        CopyIfMissing(resourceDir, AppPaths.UserDictFile, "userDict.json");
    }

    private static void CopyIfMissing(string resourceDir, string dest, string filename)
    {
        if (File.Exists(dest))
        {
            return;
        }

        var source = Path.Combine(resourceDir, filename);
        if (File.Exists(source))
        {
            File.Copy(source, dest, true);
        }
    }

    private static T LoadResource<T>(string filename) where T : class
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", filename);
        return File.Exists(path) ? JsonStore.Load(path, Activator.CreateInstance<T>()) : Activator.CreateInstance<T>();
    }
}
