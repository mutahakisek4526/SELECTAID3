using System.Collections.Generic;

namespace SelectAid.Models;

public sealed class AppSettings
{
    public int Version { get; set; } = 1;
    public string CurrentProfileId { get; set; } = "default";
    public bool HighContrast { get; set; }
    public bool AutoStartEnabled { get; set; }
    public bool AllowPowerControls { get; set; }
    public bool ClearAfterSpeak { get; set; } = true;
    public int DwellMs { get; set; } = 800;
    public int ScanIntervalMs { get; set; } = 900;
    public int ConfirmGuardMs { get; set; } = 600;
    public int UndoWindowMs { get; set; } = 3000;
    public int AutoStopAfterCycles { get; set; } = 2;
    public int MouseGridSplit { get; set; } = 3;
    public InputMode CurrentInputMode { get; set; } = InputMode.EyeOnly;
}

public sealed class ProfilesDocument
{
    public int Version { get; set; } = 1;
    public List<Profile> Profiles { get; set; } = new();
}

public sealed class Profile
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ThemeId { get; set; } = "Friendly";
    public bool SupporterLocked { get; set; }
}

public sealed class HistoryDocument
{
    public int Version { get; set; } = 1;
    public List<HistoryItem> Items { get; set; } = new();
}

public sealed class HistoryItem
{
    public string Text { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
}
