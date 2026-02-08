using System;
using System.Globalization;
using System.IO;

namespace SelectAid.Services;

public sealed class MetricsService
{
    private int _confirmCount;
    private int _undoCount;
    private int _mistakeCount;
    private int _totalConfirmMs;

    public void RecordConfirm(int durationMs)
    {
        _confirmCount++;
        _totalConfirmMs += durationMs;
        Save();
    }

    public void RecordUndo()
    {
        _undoCount++;
        Save();
    }

    public void RecordMistake()
    {
        _mistakeCount++;
        Save();
    }

    public MetricsSnapshot GetSnapshot()
    {
        var average = _confirmCount == 0 ? 0 : _totalConfirmMs / _confirmCount;
        return new MetricsSnapshot(_confirmCount, _undoCount, _mistakeCount, average);
    }

    private void Save()
    {
        try
        {
            Directory.CreateDirectory(AppPaths.AppDataDirectory);
            var average = _confirmCount == 0 ? 0 : _totalConfirmMs / _confirmCount;
            var line = string.Join(",", DateTime.Now.ToString("s", CultureInfo.InvariantCulture), _confirmCount, _undoCount, _mistakeCount, average);
            File.AppendAllLines(AppPaths.MetricsFile, new[] { line });
        }
        catch (Exception ex)
        {
            LoggingService.LogError("Metrics save failed", ex);
        }
    }
}

public readonly record struct MetricsSnapshot(int ConfirmCount, int UndoCount, int MistakeCount, int AverageConfirmMs);
