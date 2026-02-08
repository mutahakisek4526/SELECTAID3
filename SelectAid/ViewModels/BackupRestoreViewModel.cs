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

    private void Backup()
    {
        try
        {
            Directory.CreateDirectory(AppPaths.BackupsDirectory);
            var name = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            var path = Path.Combine(AppPaths.BackupsDirectory, name);
            ZipFile.CreateFromDirectory(AppPaths.AppDataDirectory, path, CompressionLevel.Fastest, false);
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
            var safety = Path.Combine(AppPaths.BackupsDirectory, $"pre_restore_{DateTime.Now:yyyyMMdd_HHmmss}.zip");
            ZipFile.CreateFromDirectory(AppPaths.AppDataDirectory, safety, CompressionLevel.Fastest, false);
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
