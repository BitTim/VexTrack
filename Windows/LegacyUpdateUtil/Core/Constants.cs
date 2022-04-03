using System;

namespace LegacyUpdateUtil.Core
{
	static class Constants
	{
		public static readonly string AppName = "VexTrack";
		public static readonly string Version = "v1.6";
		public static readonly string UpdateVersion = "v1.7";

		public static readonly string DataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/VexTrack";
		public static readonly string LegacyDataFolder = @"dat";
		public static readonly string UpdateFolder = DataFolder + @"/Update";

		public static readonly string ReleasesURL = "https://api.github.com/repos/BitTim/VexTrack/releases";
		public static readonly string BaseDownloadURL = "https://github.com/BitTim/VexTrack/releases/download";
	}
}
