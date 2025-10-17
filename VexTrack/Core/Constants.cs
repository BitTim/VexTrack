using System;
using System.Collections.Generic;

namespace VexTrack.Core
{
	static class Constants
	{
		public static readonly string AppName = "VexTrack";
		public static readonly string Version = "v1.88";

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

		public static readonly int XPPerLevel = 750;
		public static readonly int XPPerEpilogueLevel = 36500;
		public static readonly int Level2Offset = 500;

		public static readonly string DefaultGroupUUID = "07a7c1e7-5cfc-40b5-b7a5-7d3aaa7a6352";

		public static readonly List<string> Maps = new List<string>()
		{
			"Ascent",
			"Split",
			"Fracture",
			"Bind",
			"Breeze",
			"Abyss",
			"Lotus",
			"Sunset",
			"Peral",
			"Icebox",
			"Corrode",
			"Haven",
			"Disctrict",
			"Kasbah",
			"Drift",
			"Glitch",
			"Piazza",
			"Skirmish A",
			"Skirmish B",
			"Skirmish C",
			"Custom"
		};

		public static readonly List<string> Gamemodes = new List<string>()
		{
			"Unrated",
			"Competitive",
			"Swiftplay",
			"Spike Rush",
			"Team Deathmatch",
			"Deathmatch",
			"Escalation",
			"Replication",
			"Snowball Fight",
			"Skirmish",
			"Custom"
		};

		public static readonly Dictionary<string, string> ScoreTypes = new Dictionary<string, string>()
		{
			[Gamemodes[0]] = "Score",
			[Gamemodes[1]] = "Score",
			[Gamemodes[2]] = "Score",
			[Gamemodes[3]] = "Score",
			[Gamemodes[4]] = "Score",
			[Gamemodes[5]] = "Placement",
			[Gamemodes[6]] = "Score",
			[Gamemodes[7]] = "Score",
			[Gamemodes[8]] = "Score",
			[Gamemodes[9]] = "Score",
			[Gamemodes[10]] = "None"
		};

		public static readonly Dictionary<string, int> StreakStatusOrder = new Dictionary<string, int>
		{
			["None"] = 0,
			["Daily"] = 1,
			["DailyEpilogue"] = 2
		};

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

		public static readonly string ReleasesURL = "https://api.github.com/repos/BitTim/VexTrack/releases";
		public static readonly string BaseDownloadURL = "https://github.com/BitTim/VexTrack/releases/download";
	}
}
