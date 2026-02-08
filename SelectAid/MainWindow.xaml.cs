using System.Windows;
using SelectAid.ViewModels;

namespace SelectAid;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new AacViewModel();
    }
}
