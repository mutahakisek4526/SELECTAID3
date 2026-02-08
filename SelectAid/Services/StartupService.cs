using System;
using Microsoft.Win32;

namespace SelectAid.Services;

public sealed class StartupService
{
    private const string RunKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
    private const string AppName = "SelectAid";

    public bool IsAutoStartEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, false);
        return key?.GetValue(AppName) != null;
    }

    public void SetAutoStart(bool enabled)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
            if (key == null)
            {
                return;
            }

            if (enabled)
            {
                var exePath = Environment.ProcessPath ?? string.Empty;
                key.SetValue(AppName, exePath);
            }
            else
            {
                key.DeleteValue(AppName, false);
            }
        }
        catch (Exception ex)
        {
            LoggingService.LogError("Auto start update failed", ex);
        }
    }
}
