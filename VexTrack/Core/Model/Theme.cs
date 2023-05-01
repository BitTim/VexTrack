using System.Windows;
using System.Windows.Media;

namespace VexTrack.Core.Model;

public class Theme
{
    public Brush BackgroundBrush { get; }
    public Brush ShadeBrush { get; }
    public Brush ForegroundBrush { get; }
    public Brush AccentBrush { get; }
    public Brush MonoBrush { get; }

    public Theme(string themeString, string systemThemeString, string accentString)
    {
        if (string.IsNullOrEmpty(themeString)) themeString = "Light";
        if (string.IsNullOrEmpty(systemThemeString)) systemThemeString = "Light";
        if (string.IsNullOrEmpty(accentString)) accentString = "Blue";
			
        if (themeString == "Auto") themeString = systemThemeString;
        if (accentString == "Mono") accentString += themeString;

        BackgroundBrush = (Brush)Application.Current.FindResource(themeString + "Background");
        ShadeBrush = (Brush)Application.Current.FindResource(themeString + "Shade");
        ForegroundBrush = (Brush)Application.Current.FindResource(themeString + "Foreground");
        AccentBrush = (Brush)Application.Current.FindResource(accentString);
        MonoBrush = (Brush)Application.Current.FindResource("Mono" + themeString);
    }
}