using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SelectAid.Scan;

public abstract class ScanTargetBase : IScanTarget
{
    private bool _isHighlighted;

    public abstract string Label { get; }

    public bool IsHighlighted
    {
        get => _isHighlighted;
        set
        {
            if (_isHighlighted == value)
            {
                return;
            }

            _isHighlighted = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public abstract void Activate();

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
