using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VexTrack.Core
{
	static class Constants
	{
		public static readonly string AppName = "VexTrack";
		public static readonly string Version = "v1.7";

		public static readonly string DataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/VexTrack";
		public static readonly string LegacyDataFolder = @"dat";
		public static readonly string UpdateFolder = DataFolder + @"/Update";

		public static readonly string SettingsPath = DataFolder + @"/settings.json";
		public static readonly string DataPath = DataFolder + @"/data.json";
		public static readonly string LegacyDataPath = DataFolder + @"/config.json";

		public static readonly int BattlepassLevels = 50;
		public static readonly int EpilogueLevels = 5;

		public static readonly int XPPerLevel = 750;
		public static readonly int XPPerEpilogueLevel = 36500;
		public static readonly int Level2Offset = 500;

		public static Dictionary<string, string> ThemeURIs = new Dictionary<string, string>
		{
			["Light"] = "Theme/AppTheme/LightTheme.xaml",
			["Dark"] = "Theme/AppTheme/DarkTheme.xaml"
		};

		public static Dictionary<string, string> AccentURIs = new Dictionary<string, string>
		{
			["Blue"] = "Theme/AccentColors/Blue.xaml",
			["Teal"] = "Theme/AccentColors/Teal.xaml",
			["Green"] = "Theme/AccentColors/Green.xaml",
			["Yellow"] = "Theme/AccentColors/Yellow.xaml",
			["Orange"] = "Theme/AccentColors/Orange.xaml",
			["Red"] = "Theme/AccentColors/Red.xaml",
			["Purple"] = "Theme/AccentColors/Purple.xaml",
			["Mono"] = "Theme/AccentColors/Mono.xaml"
		};

		public static readonly string ReleasesURL = "https://api.github.com/repos/BitTim/VexTrack/releases";
		public static readonly string BaseDownloadURL = "https://github.com/BitTim/VexTrack/releases/download";
	}
}
