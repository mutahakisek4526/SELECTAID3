using SelectAid.Models;

namespace SelectAid.Persistence;

public static class Migration
{
    public static AppSettings EnsureSettings(AppSettings settings)
    {
        settings.Version = 1;
        return settings;
    }

    public static ProfilesDocument EnsureProfiles(ProfilesDocument profiles)
    {
        profiles.Version = 1;
        return profiles;
    }

    public static KeyboardLayoutsDocument EnsureLayouts(KeyboardLayoutsDocument layouts)
    {
        layouts.Version = 1;
        return layouts;
    }

    public static PhrasesDocument EnsurePhrases(PhrasesDocument phrases)
    {
        phrases.Version = 1;
        return phrases;
    }

    public static UserDictionary EnsureUserDict(UserDictionary dict)
    {
        dict.Version = 1;
        return dict;
    }

    public static HistoryDocument EnsureHistory(HistoryDocument history)
    {
        history.Version = 1;
        return history;
    }
}
