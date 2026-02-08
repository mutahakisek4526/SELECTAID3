using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using SelectAid.Services;
using UITimer = System.Timers.Timer;

namespace SelectAid.ViewModels;

public sealed class BackupRestoreViewModel : ObservableObject
{
    private readonly UITimer _confirmTimer;
    private string _pendingRestore = string.Empty;
    private string _status = string.Empty;

    public BackupRestoreViewModel()
    {
        BackupCommand = new RelayCommand(_ => Backup());
        RestoreCommand = new RelayCommand(_ => Restore());
        _confirmTimer = new UITimer(3000) { AutoReset = false };
        _confirmTimer.Elapsed += (_, _) => _pendingRestore = string.Empty;
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public RelayCommand BackupCommand { get; }
    public RelayCommand RestoreCommand { get; }

    private string CreateZipToBackups(string name)
    {
        Directory.CreateDirectory(AppPaths.BackupsDirectory);
        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{name}");
        var finalPath = Path.Combine(AppPaths.BackupsDirectory, name);

        try
        {
            ZipFile.CreateFromDirectory(AppPaths.AppDataDirectory, tempPath, CompressionLevel.Fastest, false);
            File.Move(tempPath, finalPath, true);
            return finalPath;
        }
        catch
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            throw;
        }
    }

    private void Backup()
    {
        try
        {
            var name = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            CreateZipToBackups(name);
            Status = $"Backup created: {name}";
        }
        catch (Exception ex)
        {
            LoggingService.LogError("Backup failed", ex);
            Status = "Backup failed.";
        }
    }

    private void Restore()
    {
        Directory.CreateDirectory(AppPaths.BackupsDirectory);
        var latest = Directory.GetFiles(AppPaths.BackupsDirectory, "backup_*.zip")
            .OrderByDescending(File.GetCreationTime)
            .FirstOrDefault();

        if (latest == null)
        {
            Status = "No backup found.";
            return;
        }

        if (_pendingRestore != latest)
        {
            _pendingRestore = latest;
            _confirmTimer.Stop();
            _confirmTimer.Start();
            MessageBox.Show("Confirm restore by pressing again within 3 seconds.");
            return;
        }

        _pendingRestore = string.Empty;
        _confirmTimer.Stop();

        try
        {
            CreateZipToBackups($"pre_restore_{DateTime.Now:yyyyMMdd_HHmmss}.zip");
            ZipFile.ExtractToDirectory(latest, AppPaths.AppDataDirectory, true);
            Status = "Restore completed. Restart the app.";
        }
        catch (Exception ex)
        {
            LoggingService.LogError("Restore failed", ex);
            Status = "Restore failed.";
        }
    }
}
