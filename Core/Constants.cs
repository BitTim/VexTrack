using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VexTrack.Core
{
	static class Constants
	{
		public static readonly string SettingsPath = @"dat/settings.json";
		public static readonly string DataPath = @"dat/data.json";
		public static readonly string LegacyDataPath = @"dat/config.json";

		public static readonly int BattlepassLevels = 50;
		public static readonly int EpilogueLevels = 5;

		public static readonly int XPPerLevel = 750;
		public static readonly int XPPerEpilogueLevel = 36500;
		public static readonly int Level2Offset = 500;

		//TODO: Move this to Settings
		public static readonly int BufferDays = 8;
		public static readonly bool IgnoreInitDay = true;
		public static readonly bool IgnoreInactiveDays = true;
		// --------------------------

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
	}
}
