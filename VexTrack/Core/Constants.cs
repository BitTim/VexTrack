using System;
using System.Collections.Generic;

namespace VexTrack.Core;

internal static class Constants
{
	public const string AppName = "VexTrack";
	public const string Version = "v2.0";
	public const string DataVersion = "v2";

	public static readonly string DataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/VexTrack - Preview";
	public const string LegacyDataFolder = @"dat";
	public static readonly string SettingsPath = DataFolder + @"/settings.json";
	public static readonly string DataPath = DataFolder + @"/data.json";
	
	public static readonly string CachePath = DataFolder + @"/cache.json";
	public static readonly string AssetFolder = DataFolder + @"/Assets";
	public static readonly string MapListViewImageFolder = AssetFolder + @"/Maps/ListViewImages/";
	public static readonly string MapSplashImageFolder = AssetFolder + @"/Maps/SplashImages/";
	public static readonly string GameModeIconFolder = AssetFolder + @"/GameModes/Icons/";

	public static readonly string UpdateFolder = DataFolder + @"/Update";
	public const string ManifestFile = @"/manifest.json";
	public const string FileListFile = @"/fileList.txt";

	public const int BattlepassLevels = 50;
	public const int EpilogueLevels = 5;

	public const int XpPerLevel = 750;
	public const int XpPerEpilogueLevel = 36500;
	public const int Level2Offset = 500;

	public static readonly string CompetitiveGameModeUuid = "0e82e886-f92c-44d4-a40e-9f849798c000";
	
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

	public const string ReleasesUrl = "https://api.github.com/repos/BitTim/VexTrack/releases";
	public const string BaseDownloadUrl = "https://github.com/BitTim/VexTrack/releases/download";
}