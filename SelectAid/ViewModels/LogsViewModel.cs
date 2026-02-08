using System.IO;
using SelectAid.Services;

namespace SelectAid.ViewModels;

public sealed class LogsViewModel : ObservableObject
{
    private string _logText = string.Empty;

    public LogsViewModel()
    {
        RefreshCommand = new RelayCommand(_ => Refresh());
        Refresh();
    }

    public string LogText
    {
        get => _logText;
        set => SetProperty(ref _logText, value);
    }

    public RelayCommand RefreshCommand { get; }

    private void Refresh()
    {
        if (File.Exists(AppPaths.LogFile))
        {
            LogText = File.ReadAllText(AppPaths.LogFile);
        }
        else
        {
            LogText = "No logs yet.";
        }
    }
}
