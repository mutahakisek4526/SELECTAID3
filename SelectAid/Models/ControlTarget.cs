using SelectAid.ViewModels;

namespace SelectAid.Models;

public sealed class ControlTarget
{
    public ControlTarget(string label, RelayCommand command)
    {
        Label = label;
        Command = command;
    }

    public string Label { get; }

    public RelayCommand Command { get; }
}
