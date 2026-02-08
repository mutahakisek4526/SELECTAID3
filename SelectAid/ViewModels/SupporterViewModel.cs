using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using SelectAid.Models;
using SelectAid.Services;
using UITimer = System.Timers.Timer;

namespace SelectAid.ViewModels;

public sealed class SupporterViewModel : ObservableObject
{
    private readonly UITimer _confirmTimer;
    private string _pendingAction = string.Empty;
    private InputMode _selectedMode;
    private string _selectedTheme;
    private bool _highContrast;
    private bool _autoStart;
    private bool _allowPowerControls;

    public SupporterViewModel()
    {
        var settings = AppServices.Persistence.Settings;
        _selectedMode = settings.CurrentInputMode;
        _selectedTheme = AppServices.Persistence.CurrentProfile.ThemeId;
        _highContrast = settings.HighContrast;
        _autoStart = settings.AutoStartEnabled;
        _allowPowerControls = settings.AllowPowerControls;

        InputModes = Enum.GetValues<InputMode>().ToList();
        Themes = new List<string> { "Friendly", "Stylish", "Kids" };

        SaveCommand = new RelayCommand(_ => Save());
        ExecutePowerCommand = new RelayCommand(param => ExecutePower(param?.ToString() ?? string.Empty));

        _confirmTimer = new UITimer(3000) { AutoReset = false };
        _confirmTimer.Elapsed += (_, _) => _pendingAction = string.Empty;
    }

    public List<InputMode> InputModes { get; }
    public List<string> Themes { get; }

    public InputMode SelectedMode
    {
        get => _selectedMode;
        set => SetProperty(ref _selectedMode, value);
    }

    public string SelectedTheme
    {
        get => _selectedTheme;
        set => SetProperty(ref _selectedTheme, value);
    }

    public bool HighContrast
    {
        get => _highContrast;
        set => SetProperty(ref _highContrast, value);
    }

    public bool AutoStart
    {
        get => _autoStart;
        set => SetProperty(ref _autoStart, value);
    }

    public bool AllowPowerControls
    {
        get => _allowPowerControls;
        set => SetProperty(ref _allowPowerControls, value);
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand ExecutePowerCommand { get; }

    private void Save()
    {
        var settings = AppServices.Persistence.Settings;
        settings.CurrentInputMode = SelectedMode;
        settings.HighContrast = HighContrast;
        settings.AutoStartEnabled = AutoStart;
        settings.AllowPowerControls = AllowPowerControls;
        var profile = AppServices.Persistence.CurrentProfile;
        profile.ThemeId = SelectedTheme;
        AppServices.Persistence.SaveSettings();
        AppServices.Persistence.SaveAll();
        AppServices.InputRouter.SetMode(SelectedMode);
        AppServices.Theme.ApplyTheme(SelectedTheme, HighContrast);
        AppServices.Startup.SetAutoStart(AutoStart);
        if (Application.Current?.MainWindow?.DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.UpdateStatus();
        }
    }

    private void ExecutePower(string action)
    {
        if (!AllowPowerControls)
        {
            MessageBox.Show("Power controls are disabled.");
            return;
        }

        if (_pendingAction != action)
        {
            _pendingAction = action;
            _confirmTimer.Stop();
            _confirmTimer.Start();
            MessageBox.Show($"Confirm {action} by pressing again within 3 seconds.");
            return;
        }

        _pendingAction = string.Empty;
        _confirmTimer.Stop();

        try
        {
            var args = action switch
            {
                "Sleep" => "/c rundll32.exe powrprof.dll,SetSuspendState 0,1,0",
                "Shutdown" => "/c shutdown /s /t 0",
                "Restart" => "/c shutdown /r /t 0",
                "Logoff" => "/c shutdown /l",
                _ => string.Empty
            };

            if (!string.IsNullOrWhiteSpace(args))
            {
                Process.Start(new ProcessStartInfo("cmd.exe", args) { CreateNoWindow = true, UseShellExecute = false });
            }
        }
        catch (Exception ex)
        {
            LoggingService.LogError("Power action failed", ex);
        }
    }
}
