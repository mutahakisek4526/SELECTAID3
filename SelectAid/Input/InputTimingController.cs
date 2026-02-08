using System;
using System.Windows;
using System.Windows.Threading;
using SelectAid.Services;
using UITimer = System.Timers.Timer;

namespace SelectAid.Input;

public sealed class InputTimingController
{
    private readonly UITimer _dwellTimer;
    private readonly UITimer _scanTimer;
    private int _dwellElapsed;
    private int _dwellTarget;
    private int _scanInterval;

    public event EventHandler<int>? DwellProgress;
    public event EventHandler? DwellCompleted;
    public event EventHandler? ScanTick;

    public InputTimingController(InputRouter router)
    {
        _ = router;
        _dwellTimer = new UITimer(50) { AutoReset = true };
        _scanTimer = new UITimer(500) { AutoReset = true };

        _dwellTimer.Elapsed += (_, _) => OnDwellTick();
        _scanTimer.Elapsed += (_, _) => OnScanTick();

        _dwellTarget = AppServices.Persistence.Settings.DwellMs;
        _scanInterval = AppServices.Persistence.Settings.ScanIntervalMs;
        _scanTimer.Interval = _scanInterval;
    }

    public void StartDwell()
    {
        _dwellElapsed = 0;
        _dwellTimer.Start();
    }

    public void StopDwell()
    {
        _dwellTimer.Stop();
        UpdateDwell(0);
    }

    public void UpdateSettings(int dwellMs, int scanIntervalMs)
    {
        _dwellTarget = dwellMs;
        _scanInterval = scanIntervalMs;
        _scanTimer.Interval = _scanInterval;
    }

    public void StartScan()
    {
        _scanTimer.Interval = _scanInterval;
        _scanTimer.Start();
    }

    public void StopScan()
    {
        _scanTimer.Stop();
    }

    private void OnDwellTick()
    {
        _dwellElapsed += 50;
        var progress = Math.Min(100, (int)Math.Round(_dwellElapsed * 100.0 / Math.Max(1, _dwellTarget)));
        UpdateDwell(progress);
        if (_dwellElapsed >= _dwellTarget)
        {
            _dwellTimer.Stop();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                DwellCompleted?.Invoke(this, EventArgs.Empty);
            }));
        }
    }

    private void UpdateDwell(int progress)
    {
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
        {
            DwellProgress?.Invoke(this, progress);
        }));
    }

    private void OnScanTick()
    {
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
        {
            ScanTick?.Invoke(this, EventArgs.Empty);
        }));
    }
}
