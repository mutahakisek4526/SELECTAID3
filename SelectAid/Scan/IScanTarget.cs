using System.ComponentModel;

namespace SelectAid.Scan;

public interface IScanTarget : INotifyPropertyChanged
{
    string Label { get; }
    bool IsHighlighted { get; set; }
    void Activate();
}
