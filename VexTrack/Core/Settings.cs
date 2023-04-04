using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using VexTrack.MVVM.ViewModel;

namespace VexTrack.Core
{
	public class Settings
	{
		public string Username;
		public double BufferPercentage;
		public bool IgnoreInactiveDays;
		public bool IgnoreInit;
		public bool IgnorePreReleases;
		public bool ForceEpilogue;
		public bool SingleSeasonHistory;
		
		public string Theme;
		public string SystemTheme; //Not part of saved settings file
		public string Accent;

		public Settings() { }
		public Settings(string username, double bufferPercentage, bool ignoreInactiveDays, bool ignoreInit, bool ignorePreReleases, bool forceEpilogue, bool singleSeasonHistory, string theme, string systemTheme, string accent)
		{
			Username = username;
			BufferPercentage = bufferPercentage;
			IgnoreInactiveDays = ignoreInactiveDays;
			IgnoreInit = ignoreInit;
			IgnorePreReleases = ignorePreReleases;
			ForceEpilogue = forceEpilogue;
			SingleSeasonHistory = singleSeasonHistory;

			Theme = theme;
			SystemTheme = systemTheme;
			Accent = accent;
		}
		public void SetDefault()
		{
			Username = "";
			BufferPercentage = 7.5;
			IgnoreInactiveDays = true;
			IgnoreInit = true;
			IgnorePreReleases = true;
			ForceEpilogue = true;
			SingleSeasonHistory = true;

			Theme = "Auto";
			Accent = "Blue";
		}
	}

	public static class SettingsHelper
	{
		public static Settings Data { get; private set; }
		private static Settings Default { get; set; }

		public static void Init()
		{
			Data = new Settings();
			Default = new Settings();
			Default.SetDefault();
		}

		public static void CallUpdate()
		{
			var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
			mainVm.Update();
			SaveSettings();
		}

		private static void InitSettings()
		{
			Data.SetDefault();
			SaveSettings();
			LoadSettings();
		}

		public static void LoadSettings()
		{
			if (!File.Exists(Constants.SettingsPath) || File.ReadAllText(Constants.SettingsPath) == "")
			{
				InitSettings();
				return;
			}

			var rawJson = File.ReadAllText(Constants.SettingsPath);
			var jo = JObject.Parse(rawJson);

			var reSave = false;

			Data.SetDefault();

			if (jo["username"] == null) reSave = true;
			else Data.Username = (string)jo["username"];

			if (jo["bufferPercentage"] == null) reSave = true;
			else Data.BufferPercentage = (double)jo["bufferPercentage"];

			if (jo["ignoreInactiveDays"] == null) reSave = true;
			else Data.IgnoreInactiveDays = (bool)jo["ignoreInactiveDays"];

			if (jo["ignoreInit"] == null) reSave = true;
			else Data.IgnoreInit = (bool)jo["ignoreInit"];

			if (jo["ignorePreReleases"] == null) reSave = true;
			else Data.IgnorePreReleases = (bool)jo["ignorePreReleases"];

			if (jo["forceEpilogue"] == null) reSave = true;
			else Data.ForceEpilogue = (bool)jo["forceEpilogue"];

			if (jo["singleSeasonHistory"] == null) reSave = true;
			else Data.SingleSeasonHistory = (bool)jo["singleSeasonHistory"];



			if (jo["theme"] == null) reSave = true;
			else Data.Theme = (string)jo["theme"];

			if (jo["accent"] == null) reSave = true;
			else Data.Accent = (string)jo["accent"];



			if (reSave) SaveSettings();
			ApplyVisualSettings();
		}

		private static void SaveSettings()
		{
			JObject jo = new()
			{
				{ "username", Data.Username },
				{ "bufferPercentage", Data.BufferPercentage },
				{ "ignoreInactiveDays", Data.IgnoreInactiveDays },
				{ "ignoreInit", Data.IgnoreInit },
				{ "ignorePreReleases", Data.IgnorePreReleases },
				{ "forceEpilogue", Data.ForceEpilogue },
				{ "singleSeasonHistory", Data.SingleSeasonHistory },
				{ "theme", Data.Theme },
				{ "accent", Data.Accent }
			};

			if (!File.Exists(Constants.SettingsPath))
			{
				var sep = Constants.SettingsPath.LastIndexOf("/", StringComparison.Ordinal);

				Directory.CreateDirectory(Constants.SettingsPath.Substring(0, sep));
				File.CreateText(Constants.SettingsPath).Close();
			}

			File.WriteAllText(Constants.SettingsPath, jo.ToString());
		}

		public static void ApplyVisualSettings()
		{
			if (!Constants.ThemeUrIs.Keys.Contains(Data.Theme)) Data.Theme = "Auto";
			if (!Constants.AccentUrIs.Keys.Contains(Data.Accent)) Data.Accent = "Blue";

			var accentString = Data.Accent;
			var themeString = Data.Theme;
			if (themeString == "Auto") themeString = Data.SystemTheme;

			Application.Current.Resources.MergedDictionaries[1].Source = new Uri(Constants.ThemeUrIs[themeString], UriKind.Relative);
			Application.Current.Resources.MergedDictionaries[2].Source = new Uri(Constants.AccentUrIs[accentString], UriKind.Relative);
		}
	}

	public class ThemeWatcher
	{
		private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
		private const string RegistryValueName = "AppsUseLightTheme";

		private enum WindowsTheme
		{
			Light,
			Dark
		}

		private static WindowsTheme GetWindowsTheme()
		{
			using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
			var registryValueObject = key?.GetValue(RegistryValueName);
			if (registryValueObject == null)
			{
				return WindowsTheme.Light;
			}

			var registryValue = (int)registryValueObject;

			return registryValue > 0 ? WindowsTheme.Light : WindowsTheme.Dark;
		}

		public ThemeWatcher()
		{
			SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

			var theme = GetWindowsTheme().ToString();
			SettingsHelper.Data.SystemTheme = theme;
		}

		public static void Destroy()
		{
			SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
		}

		private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if (e.Category != UserPreferenceCategory.General) return;
			var theme = GetWindowsTheme().ToString();

			SettingsHelper.Data.SystemTheme = theme;
			SettingsHelper.ApplyVisualSettings();
		}
	}
}
