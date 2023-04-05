using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using VexTrack.MVVM.ViewModel;

namespace VexTrack.Core
{
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

	public static class SettingsHelper
	{
		public static Settings Data { get; set; }

		public static void Init()
		{
			Data = new Settings();
		}

		public static void CallUpdate()
		{
			var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
			mainVm.Update();
			SaveSettings();
		}

		private static void InitSettings()
		{
			Data.Reset();
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

			Data.Reset();

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
			else Data.ThemeString = (string)jo["theme"];

			if (jo["accent"] == null) reSave = true;
			else Data.AccentString = (string)jo["accent"];



			if (reSave) SaveSettings();
			Data.UpdateTheme();
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
				{ "theme", Data.ThemeString },
				{ "accent", Data.AccentString }
			};

			if (!File.Exists(Constants.SettingsPath))
			{
				var sep = Constants.SettingsPath.LastIndexOf("/", StringComparison.Ordinal);

				Directory.CreateDirectory(Constants.SettingsPath.Substring(0, sep));
				File.CreateText(Constants.SettingsPath).Close();
			}

			File.WriteAllText(Constants.SettingsPath, jo.ToString());
		}
	}

	public class ThemeWatcher
	{
		private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
		private const string RegistryValueName = "AppsUseLightTheme";

		private static string GetWindowsTheme()
		{
			using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
			var registryValueObject = key?.GetValue(RegistryValueName);
			if (registryValueObject == null)
			{
				return "Light";
			}

			var registryValue = (int)registryValueObject;

			return registryValue > 0 ? "Light" : "Dark";
		}

		public ThemeWatcher()
		{
			SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

			var theme = GetWindowsTheme();
			SettingsHelper.Data.SystemThemeString = theme;
			SettingsHelper.Data.UpdateTheme();
		}

		public void Destroy()
		{
			SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
		}

		private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if (e.Category != UserPreferenceCategory.General) return;
			var theme = GetWindowsTheme();

			SettingsHelper.Data.SystemThemeString = theme;
			SettingsHelper.Data.UpdateTheme();
		}
	}

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
}
