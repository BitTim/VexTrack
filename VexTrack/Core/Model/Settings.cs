using VexTrack.Core.Model.WPF;

namespace VexTrack.Core.Model;

public class Settings : ObservableObject
{
    public string Username;
    public double BufferPercentage;
    public bool IgnoreInactiveDays;
    public bool IgnoreInit;
    public bool IgnorePreReleases;
    public bool ForceEpilogue;
    public bool SingleSeasonHistory;
    
    public string ThemeString;
    public string SystemThemeString; //Not part of saved settings file
    public string AccentString;
    private Theme _theme;

    public Theme Theme
    {
    	get => _theme;
    	private set
    	{
    		if (Equals(value, _theme)) return;
    		_theme = value;
    		OnPropertyChanged();
    	}
    }

    public Settings()
    {
    	Reset();
    }

    public void Reset()
    {
    	Username = "";
    	BufferPercentage = 7.5;
    	IgnoreInactiveDays = true;
    	IgnoreInit = true;
    	IgnorePreReleases = true;
    	ForceEpilogue = true;
    	SingleSeasonHistory = true;

    	ThemeString = "Auto";
    	AccentString = "Blue";

    	UpdateTheme();
    }

    public void UpdateTheme()
    {
    	Theme = GetTheme();
    }
    
    private Theme GetTheme()
    {
    	return new Theme
    	(
    		ThemeString,
    		SystemThemeString,
    		AccentString
    	);
    }
}
    	