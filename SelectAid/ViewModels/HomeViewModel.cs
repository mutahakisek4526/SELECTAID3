using System.Windows;
using SelectAid.Overlay;
using SelectAid.Services;

namespace SelectAid.ViewModels;

public sealed class HomeViewModel : ObservableObject
{
    public HomeViewModel(MainViewModel mainViewModel)
    {
        OpenOverlayCommand = new RelayCommand(_ => OpenOverlay());
    }

    public RelayCommand OpenOverlayCommand { get; }

    private void OpenOverlay()
    {
        var overlay = new OverlayWindow(AppServices.InputSend);
        overlay.Show();
    }
}
