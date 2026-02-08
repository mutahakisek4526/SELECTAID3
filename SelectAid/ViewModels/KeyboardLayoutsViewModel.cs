using System;
using System.Text.Json;
using SelectAid.Models;
using SelectAid.Services;

namespace SelectAid.ViewModels;

public sealed class KeyboardLayoutsViewModel : ObservableObject
{
    private string _layoutJson = string.Empty;

    public KeyboardLayoutsViewModel()
    {
        LayoutJson = JsonSerializer.Serialize(AppServices.Persistence.KeyboardLayouts, new JsonSerializerOptions { WriteIndented = true });
        SaveCommand = new RelayCommand(_ => Save());
        ReloadCommand = new RelayCommand(_ => Reload());
    }

    public string LayoutJson
    {
        get => _layoutJson;
        set => SetProperty(ref _layoutJson, value);
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand ReloadCommand { get; }

    private void Save()
    {
        try
        {
            var layouts = JsonSerializer.Deserialize<KeyboardLayoutsDocument>(LayoutJson);
            if (layouts == null)
            {
                return;
            }

            AppServices.Persistence.KeyboardLayouts = layouts;
            AppServices.Persistence.SaveLayouts();
        }
        catch (Exception ex)
        {
            LoggingService.LogError("Keyboard layout save failed", ex);
        }
    }

    private void Reload()
    {
        LayoutJson = JsonSerializer.Serialize(AppServices.Persistence.KeyboardLayouts, new JsonSerializerOptions { WriteIndented = true });
    }
}
