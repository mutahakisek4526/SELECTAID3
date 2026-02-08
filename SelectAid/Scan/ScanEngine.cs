using System;
using System.Collections.Generic;
using SelectAid.Services;

namespace SelectAid.Scan;

public sealed class ScanEngine
{
    private readonly List<IScanTarget> _targets = new();
    private int _index;
    private int _cycles;
    private bool _hold;

    public int AutoStopAfterCycles { get; set; } = 2;

    public void RegisterTargets(IEnumerable<IScanTarget> targets)
    {
        _targets.Clear();
        _targets.AddRange(targets);
        ResetHighlights();
    }

    public void Next()
    {
        if (_hold || _targets.Count == 0)
        {
            return;
        }

        ResetHighlights();
        _targets[_index].IsHighlighted = true;
        _index++;
        if (_index >= _targets.Count)
        {
            _index = 0;
            _cycles++;
            if (AutoStopAfterCycles > 0 && _cycles >= AutoStopAfterCycles)
            {
                _hold = true;
            }
        }
    }

    public void SelectCurrent()
    {
        var currentIndex = _index - 1;
        if (currentIndex < 0)
        {
            currentIndex = _targets.Count - 1;
        }

        if (currentIndex >= 0 && currentIndex < _targets.Count)
        {
            _targets[currentIndex].Activate();
        }
    }

    public void ResetCycle()
    {
        _index = 0;
        _cycles = 0;
        _hold = false;
        ResetHighlights();
    }

    public void SetHold(bool hold)
    {
        _hold = hold;
    }

    private void ResetHighlights()
    {
        foreach (var target in _targets)
        {
            target.IsHighlighted = false;
        }
    }
}
