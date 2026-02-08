using SelectAid.Services;

namespace SelectAid.ViewModels;

public sealed class SettingsViewModel : ObservableObject
{
    private bool _clearAfterSpeak;
    private int _dwellMs;
    private int _scanIntervalMs;
    private int _confirmGuardMs;
    private int _undoWindowMs;
    private int _autoStopAfterCycles;
    private int _mouseGridSplit;

    public SettingsViewModel()
    {
        var settings = AppServices.Persistence.Settings;
        _clearAfterSpeak = settings.ClearAfterSpeak;
        _dwellMs = settings.DwellMs;
        _scanIntervalMs = settings.ScanIntervalMs;
        _confirmGuardMs = settings.ConfirmGuardMs;
        _undoWindowMs = settings.UndoWindowMs;
        _autoStopAfterCycles = settings.AutoStopAfterCycles;
        _mouseGridSplit = settings.MouseGridSplit;

        SaveCommand = new RelayCommand(_ => Save());
    }

    public bool ClearAfterSpeak
    {
        get => _clearAfterSpeak;
        set => SetProperty(ref _clearAfterSpeak, value);
    }

    public int DwellMs
    {
        get => _dwellMs;
        set => SetProperty(ref _dwellMs, value);
    }

    public int ScanIntervalMs
    {
        get => _scanIntervalMs;
        set => SetProperty(ref _scanIntervalMs, value);
    }

    public int ConfirmGuardMs
    {
        get => _confirmGuardMs;
        set => SetProperty(ref _confirmGuardMs, value);
    }

    public int UndoWindowMs
    {
        get => _undoWindowMs;
        set => SetProperty(ref _undoWindowMs, value);
    }

    public int AutoStopAfterCycles
    {
        get => _autoStopAfterCycles;
        set => SetProperty(ref _autoStopAfterCycles, value);
    }

    public int MouseGridSplit
    {
        get => _mouseGridSplit;
        set => SetProperty(ref _mouseGridSplit, value);
    }

    public RelayCommand SaveCommand { get; }

    private void Save()
    {
        var settings = AppServices.Persistence.Settings;
        settings.ClearAfterSpeak = ClearAfterSpeak;
        settings.DwellMs = DwellMs;
        settings.ScanIntervalMs = ScanIntervalMs;
        settings.ConfirmGuardMs = ConfirmGuardMs;
        settings.UndoWindowMs = UndoWindowMs;
        settings.AutoStopAfterCycles = AutoStopAfterCycles;
        settings.MouseGridSplit = MouseGridSplit;
        AppServices.Persistence.SaveSettings();
        AppServices.TimingController.UpdateSettings(DwellMs, ScanIntervalMs);
    }
}
