using System;
using System.IO;

namespace SelectAid.Services;

public static class LoggingService
{
    private static readonly object SyncRoot = new();

    public static void LogInfo(string message)
    {
        Write("INFO", message, null);
    }

    public static void LogWarning(string message)
    {
        Write("WARN", message, null);
    }

    public static void LogError(string message, Exception exception)
    {
        Write("ERROR", message, exception);
    }

    private static void Write(string level, string message, Exception? exception)
    {
        try
        {
            Directory.CreateDirectory(AppPaths.AppDataDirectory);
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level} {message}";
            if (exception != null)
            {
                line += Environment.NewLine + exception;
            }

            lock (SyncRoot)
            {
                File.AppendAllText(AppPaths.LogFile, line + Environment.NewLine);
            }
        }
        catch
        {
            // avoid logging failures breaking app
        }
    }
}
