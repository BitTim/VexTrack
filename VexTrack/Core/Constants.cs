using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace VexTrack.Core
{
	static class Constants
	{
		public static readonly string AppName = "VexTrack";
		public static readonly string Version = "v1.9";
		public static readonly string DataVersion = "v2";

		public static readonly string DataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/VexTrack - Preview";
		public static readonly string LegacyDataFolder = @"dat";
		public static readonly string UpdateFolder = DataFolder + @"/Update";
		public static readonly string LogsFolder = DataFolder + @"/logs";

		public static readonly string SettingsPath = DataFolder + @"/settings.json";
		public static readonly string DataPath = DataFolder + @"/data.json";
		public static readonly string LegacyDataPath = DataFolder + @"/config.json";
		public static readonly string ManifestFile = @"/manifest.json";
		public static readonly string FileListFile = @"/fileList.txt";

		public static readonly int BattlepassLevels = 50;
		public static readonly int EpilogueLevels = 5;
		public static readonly int AgentTiers = 10;

		public static readonly int XpPerLevel = 750;
		public static readonly int XpPerEpilogueLevel = 36500;
		public static readonly int Level2Offset = 500;

		public static readonly List<int> TierTotals = new()
		{
			20000,
			30000,
			40000,
			50000,
			60000,
			75000,
			100000,
			150000,
			200000,
			250000
		};

		public static readonly string DefaultGroupUuid = "07a7c1e7-5cfc-40b5-b7a5-7d3aaa7a6352";

		public static readonly List<string> Maps = new()
		{
			"Ascent",
			"Bind",
			"Breeze",
			"Fracture",
			"Haven",
			"Icebox",
			"Split",
			"None"
		};

		public static readonly List<string> Gamemodes = new()
		{
			"Unrated",
			"Competetive",
			"Spike Rush",
			"Deathmatch",
			"Escalation",
			"Replication",
			"Snowballfight",
			"Custom"
		};

		public static readonly Dictionary<string, string> ScoreTypes = new()
		{
			[Gamemodes[0]] = "Score",
			[Gamemodes[1]] = "Score",
			[Gamemodes[2]] = "Score",
			[Gamemodes[3]] = "Placement",
			[Gamemodes[4]] = "Score",
			[Gamemodes[5]] = "Score",
			[Gamemodes[6]] = "Score",
			[Gamemodes[7]] = "None"
		};

		public static readonly Dictionary<string, int> StreakStatusOrder = new()
		{
			["None"] = 0,
			["Daily"] = 1,
			["DailyEpilogue"] = 2
		};

		public static Dictionary<string, string> ThemeUrIs = new()
		{
			["Light"] = "Theme/AppTheme/LightTheme.xaml",
			["Dark"] = "Theme/AppTheme/DarkTheme.xaml"
		};

		public static Dictionary<string, string> AccentUrIs = new()
		{
			["Blue"] = "Theme/AccentColors/Blue.xaml",
			["Teal"] = "Theme/AccentColors/Teal.xaml",
			["Green"] = "Theme/AccentColors/Green.xaml",
			["Yellow"] = "Theme/AccentColors/Yellow.xaml",
			["Orange"] = "Theme/AccentColors/Orange.xaml",
			["Red"] = "Theme/AccentColors/Red.xaml",
			["Purple"] = "Theme/AccentColors/Purple.xaml",
			["Mono"] = "Theme/AccentColors/Mono.xaml",
			["HotCold"] = "Theme/AccentColors/HotCold.xaml",
			["Cyberpunk"] = "Theme/AccentColors/Cyberpunk.xaml",
			["Lavender"] = "Theme/AccentColors/Lavender.xaml",
			["Aqua"] = "Theme/AccentColors/Aqua.xaml",
			["Nature"] = "Theme/AccentColors/Nature.xaml",
			["Emerald"] = "Theme/AccentColors/Emerald.xaml",
			["Chocolate"] = "Theme/AccentColors/Chocolate.xaml",
			["Cyberpunk2"] = "Theme/AccentColors/Cyberpunk2.xaml",
		};

		public static readonly string ReleasesUrl = "https://api.github.com/repos/BitTim/VexTrack/releases";
		public static readonly string BaseDownloadUrl = "https://github.com/BitTim/VexTrack/releases/download";
	}
}
