using System;
using System.Windows.Input;

namespace SelectAid.Scan;

public sealed class ScanTargetViewModel : ScanTargetBase, ICommand
{
    private readonly ICommand _command;
    private readonly object? _parameter;
    private readonly string _label;

    public ScanTargetViewModel(string label, ICommand command, object? parameter = null)
    {
        _label = label;
        _command = command;
        _parameter = parameter;
    }

    public override string Label => _label;

    public override void Activate()
    {
        if (_command.CanExecute(_parameter))
        {
            _command.Execute(_parameter);
        }
    }

    public bool CanExecute(object? parameter)
    {
        return _command.CanExecute(_parameter);
    }

    public void Execute(object? parameter)
    {
        Activate();
    }

    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }
}
