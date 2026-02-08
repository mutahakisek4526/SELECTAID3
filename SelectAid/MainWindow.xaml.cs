using System.Windows;
using System.Windows.Input;
using SelectAid.Input;
using SelectAid.ViewModels;

namespace SelectAid;

public partial class MainWindow : Window
{
    private readonly SwitchInput _switchInput;

    public MainWindow()
    {
        InitializeComponent();
        var viewModel = new MainViewModel();
        DataContext = viewModel;
        _switchInput = new SwitchInput(Services.AppServices.InputRouter);
        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        _switchInput.HandleKeyDown(e.Key);
    }
}
