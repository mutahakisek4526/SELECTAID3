using System;
using System.Windows;

namespace SelectAid.Theme;

public sealed class ThemeService
{
    public void ApplyTheme(string themeId, bool highContrast)
    {
        var app = Application.Current;
        if (app == null)
        {
            return;
        }

        var dictionaries = app.Resources.MergedDictionaries;
        dictionaries.Clear();
        dictionaries.Add(new ResourceDictionary { Source = new Uri($"Theme/{themeId}.xaml", UriKind.Relative) });
        if (highContrast)
        {
            dictionaries.Add(new ResourceDictionary { Source = new Uri("Theme/HighContrast.xaml", UriKind.Relative) });
        }
    }
}
