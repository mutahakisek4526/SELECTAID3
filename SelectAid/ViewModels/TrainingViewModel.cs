using SelectAid.Services;

namespace SelectAid.ViewModels;

public sealed class TrainingViewModel : ObservableObject
{
    private int _dwellProgress;
    private MetricsSnapshot _snapshot;

    public TrainingViewModel()
    {
        StartDwellCommand = new RelayCommand(_ => StartDwell());
        StopDwellCommand = new RelayCommand(_ => StopDwell());
        RefreshMetricsCommand = new RelayCommand(_ => RefreshMetrics());
        AppServices.TimingController.DwellProgress += (_, progress) => DwellProgress = progress;
        RefreshMetrics();
    }

    public int DwellProgress
    {
        get => _dwellProgress;
        set => SetProperty(ref _dwellProgress, value);
    }

    public MetricsSnapshot Snapshot
    {
        get => _snapshot;
        set => SetProperty(ref _snapshot, value);
    }

    public RelayCommand StartDwellCommand { get; }
    public RelayCommand StopDwellCommand { get; }
    public RelayCommand RefreshMetricsCommand { get; }

    private void StartDwell()
    {
        AppServices.TimingController.StartDwell();
    }

    private void StopDwell()
    {
        AppServices.TimingController.StopDwell();
    }

    private void RefreshMetrics()
    {
        Snapshot = AppServices.Metrics.GetSnapshot();
    }
}
