using System;
using SelectAid.Input;
using SelectAid.Models;
using SelectAid.Services;

namespace SelectAid.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private ObservableObject _currentViewModel = null!;
    private string _statusText = "";
    private int _dwellProgress;
    private readonly Scan.ScanEngine _scanEngine = new();

    public MainViewModel()
    {
        NavigateCommand = new RelayCommand(param => Navigate(param?.ToString() ?? "Home"));
        HomeCommand = new RelayCommand(_ => Navigate("Home"));
        BackCommand = new RelayCommand(_ => Navigate("Home"));
        UndoCommand = new RelayCommand(_ => AacViewModel.UndoCommand.Execute(null));
        PauseCommand = new RelayCommand(_ => AppServices.InputRouter.Trigger(InputAction.PauseToggle));
        BuzzerCommand = new RelayCommand(_ => AacViewModel.BuzzerCommand.Execute(null));
        OverlayCommand = new RelayCommand(_ => HomeViewModel.OpenOverlayCommand.Execute(null));
        HomeViewModel = new HomeViewModel(this);
        AacViewModel = new AacViewModel();
        PhrasesViewModel = new PhrasesViewModel(AacViewModel);
        KeyboardLayoutsViewModel = new KeyboardLayoutsViewModel();
        SettingsViewModel = new SettingsViewModel();
        SupporterViewModel = new SupporterViewModel();
        TrainingViewModel = new TrainingViewModel();
        LogsViewModel = new LogsViewModel();
        BackupRestoreViewModel = new BackupRestoreViewModel();

        CurrentViewModel = HomeViewModel;

        AppServices.InputRouter.ActionTriggered += (_, action) => OnInputAction(action);
        AppServices.TimingController.DwellProgress += (_, progress) => DwellProgress = progress;
        AppServices.TimingController.ScanTick += (_, _) => OnScanTick();
        UpdateStatus();
    }

    public RelayCommand NavigateCommand { get; }
    public RelayCommand HomeCommand { get; }
    public RelayCommand BackCommand { get; }
    public RelayCommand UndoCommand { get; }
    public RelayCommand PauseCommand { get; }
    public RelayCommand BuzzerCommand { get; }
    public RelayCommand OverlayCommand { get; }
    public HomeViewModel HomeViewModel { get; }
    public AacViewModel AacViewModel { get; }
    public PhrasesViewModel PhrasesViewModel { get; }
    public KeyboardLayoutsViewModel KeyboardLayoutsViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }
    public SupporterViewModel SupporterViewModel { get; }
    public TrainingViewModel TrainingViewModel { get; }
    public LogsViewModel LogsViewModel { get; }
    public BackupRestoreViewModel BackupRestoreViewModel { get; }

    public ObservableObject CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            SetProperty(ref _currentViewModel, value);
            RegisterScanTargets();
        }
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public int DwellProgress
    {
        get => _dwellProgress;
        set => SetProperty(ref _dwellProgress, value);
    }

    public void Navigate(string target)
    {
        CurrentViewModel = target switch
        {
            "AAC" => AacViewModel,
            "Phrases" => PhrasesViewModel,
            "KeyboardLayouts" => KeyboardLayoutsViewModel,
            "Settings" => SettingsViewModel,
            "Supporter" => SupporterViewModel,
            "Training" => TrainingViewModel,
            "Logs" => LogsViewModel,
            "BackupRestore" => BackupRestoreViewModel,
            _ => HomeViewModel
        };
        UpdateStatus();
    }

    private void OnInputAction(InputAction action)
    {
        if (action == InputAction.EmergencyStop)
        {
            StatusText = "EMERGENCY STOP";
        }
        else if (action == InputAction.PauseToggle)
        {
            UpdateStatus();
        }

        if (AppServices.Persistence.Settings.CurrentInputMode == InputMode.SwitchScan)
        {
            if (action == InputAction.Confirm)
            {
                _scanEngine.SelectCurrent();
            }
            else if (action == InputAction.Cancel)
            {
                _scanEngine.ResetCycle();
            }
        }
    }

    private void OnScanTick()
    {
        if (AppServices.Persistence.Settings.CurrentInputMode != InputMode.SwitchScan || AppServices.InputRouter.IsPaused)
        {
            return;
        }

        _scanEngine.Next();
    }

    private void RegisterScanTargets()
    {
        if (CurrentViewModel is Scan.IScanContext scanContext)
        {
            _scanEngine.AutoStopAfterCycles = AppServices.Persistence.Settings.AutoStopAfterCycles;
            _scanEngine.RegisterTargets(scanContext.GetScanTargets());
        }
    }

    public void UpdateStatus()
    {
        var settings = AppServices.Persistence.Settings;
        StatusText = $"Mode: {settings.CurrentInputMode} | Pause: {AppServices.InputRouter.IsPaused} | Theme: {AppServices.Persistence.CurrentProfile.ThemeId}";
        if (settings.CurrentInputMode == InputMode.SwitchScan)
        {
            AppServices.TimingController.StartScan();
        }
        else
        {
            AppServices.TimingController.StopScan();
        }
    }
}
