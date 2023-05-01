using Microsoft.Win32;
using VexTrack.Core.Helper;

namespace VexTrack.Core.Model.WPF;

public class ThemeWatcher
{
    private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string RegistryValueName = "AppsUseLightTheme";

    private static string GetWindowsTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
        var registryValueObject = key?.GetValue(RegistryValueName);
        if (registryValueObject == null)
        {
            return "Light";
        }

        var registryValue = (int)registryValueObject;

        return registryValue > 0 ? "Light" : "Dark";
    }

    public ThemeWatcher()
    {
        SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

        var theme = GetWindowsTheme();
        SettingsHelper.Data.SystemThemeString = theme;
        SettingsHelper.Data.UpdateTheme();
    }

    public void Destroy()
    {
        SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
    }

    private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category != UserPreferenceCategory.General) return;
        var theme = GetWindowsTheme();

        SettingsHelper.Data.SystemThemeString = theme;
        SettingsHelper.Data.UpdateTheme();
    }
}