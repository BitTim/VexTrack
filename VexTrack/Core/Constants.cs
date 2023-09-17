using System;
using System.Collections.Generic;

namespace VexTrack.Core;

internal static class Constants
{
	// General Data
	public const string AppName = "VexTrack";
	public const string Version = "v2.0";
	public const string DataVersion = "v2";

	
	
	// User Data
	public static readonly string DataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/VexTrack - Preview";
	public const string LegacyDataFolder = @"dat";
	public static readonly string SettingsPath = DataFolder + @"/settings.json";
	public static readonly string DataPath = DataFolder + @"/data.json";
	
	
	
	// Api Data
	public static readonly string CachePath = DataFolder + @"/cache.json";
	public static readonly string AssetFolder = DataFolder + @"/Assets";
	public static readonly string AssetBackupFolder = DataFolder + @"/AssetsBackup";
	
	public static readonly string MapListViewImageFolder = AssetFolder + @"/Maps/ListViewImages/";
	public static readonly string MapSplashImageFolder = AssetFolder + @"/Maps/SplashImages/";
	
	public static readonly string GameModeIconFolder = AssetFolder + @"/GameModes/Icons/";
	
	public static readonly string AgentIconFolder = AssetFolder + @"/Agents/Icons/";
	public static readonly string AgentPortraitFolder = AssetFolder + @"/Agents/Portraits/";
	public static readonly string AgentKillFeedPortraitFolder = AssetFolder + @"/Agents/KillFeedPortraits/";
	public static readonly string AgentBackgroundFolder = AssetFolder + @"/Agents/Backgournds/";
	
	public static readonly string AgentRoleIconFolder = AssetFolder + @"/Agents/Icons/Roles/";
	public static readonly string AgentAbilityIconFolder = AssetFolder + @"/Agents/Icons/Abilities/";

	public static readonly string BuddyIconFolder = AssetFolder + @"/Buddies/Icons/";
	
	public static readonly string CurrencyIconFolder = AssetFolder + @"/Currencies/Icons/";
	public static readonly string CurrencyLargeIconFolder = AssetFolder + @"/Currencies/LargeIcons/";

	public static readonly string PlayerCardIconFolder = AssetFolder + @"/PlayerCards/Icons/";
	public static readonly string PlayerCardSmallArtFolder = AssetFolder + @"/PlayerCards/SmallArt/";
	public static readonly string PlayerCardWideArtFolder = AssetFolder + @"/PlayerCards/WideArt/";
	public static readonly string PlayerCardLargeArtFolder = AssetFolder + @"/PlayerCards/LargeArt/";

	public static readonly string SprayIconFolder = AssetFolder + @"/Sprays/Icons/";
	public static readonly string SprayFullIconFolder = AssetFolder + @"/Sprays/FullIcons/";
	public static readonly string SprayAnimationFolder = AssetFolder + @"/Sprays/Animations/";

	public static readonly string WeaponIconPath = AssetFolder + @"/Weapons/Icons/";
	public static readonly string WeaponKillStreamIconPath = AssetFolder + @"/Weapons/KillStreamIcons/";
	public static readonly string WeaponSkinIconPath = AssetFolder + @"/Weapons/Skins/Icons/";
	public static readonly string WeaponSkinWallpaperPath = AssetFolder + @"/Weapons/Skins/Wallpapers/";
	public static readonly string WeaponSkinChromaIconPath = AssetFolder + @"/Weapons/Skins/Chromas/Icons/";
	public static readonly string WeaponSkinChromaFullRenderPath = AssetFolder + @"/Weapons/Skins/Chromas/FullRenders/";
	public static readonly string WeaponSkinChromaSwatchPath = AssetFolder + @"/Weapons/Skins/Chromas/Swatches/";
	public static readonly string WeaponSkinLevelIconPath = AssetFolder + @"/Weapons/Skins/Levels/Icons/";
	
	
	
	// Update
	public static readonly string UpdateFolder = DataFolder + @"/Update";
	public const string ManifestFile = @"/manifest.json";
	public const string FileListFile = @"/fileList.txt";

	public const string ReleasesUrl = "https://api.github.com/repos/BitTim/VexTrack/releases";
	public const string BaseDownloadUrl = "https://github.com/BitTim/VexTrack/releases/download";

	
	
	// Misc
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
}

public enum RomanValues
{
	I = 1,
	V = 5,
	X = 10,
	L = 50,
	C = 100,
	D = 500,
	M = 1000
}