using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VexTrack.MVVM.ViewModel;

namespace VexTrack.Core
{
	public class Settings
	{
		public string Username;
		public int BufferDays;
		public bool IgnoreInactiveDays;
		public bool IgnoreInit;
		public bool IgnorePreReleases;
		public bool ForceEpilogue;
		
		public string Theme;
		public string SystemTheme; //Not part of saved settings file
		public string Accent;
		
		public Settings() {}
		public Settings(string username, int bufferDays, bool ignoreInactiveDays, bool ignoreInit, bool ignorePreReleases, bool forceEpilogue, string theme, string systemTheme, string accent)
		{
			(Username, BufferDays, IgnoreInactiveDays, IgnoreInit, IgnorePreReleases, ForceEpilogue, Theme, SystemTheme, Accent) = (username, bufferDays, ignoreInactiveDays, ignoreInit, ignorePreReleases, forceEpilogue, theme, systemTheme, accent);
		}
		public void SetDefault()
		{
			Username = "";
			BufferDays = 8;
			IgnoreInactiveDays = true;
			IgnoreInit = true;
			IgnorePreReleases = true;
			ForceEpilogue = true;

			Theme = "Auto";
			Accent = "Blue";
		}
	}

	public static class SettingsHelper
	{
		public static Settings Data { get; set; }
		public static Settings Default { get; set; }
		
		public static void Init()
		{
			Data = new Settings();
			Default = new Settings();
			Default.SetDefault();
		}

		public static void CallUpdate()
		{
			MainViewModel MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
			MainVM.Update();
			SaveSettings();
		}

		public static void InitSettings()
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
				CreateSettingsInitPopup();
				return;
			}

			string rawJSON = File.ReadAllText(Constants.SettingsPath);
			JObject jo = JObject.Parse(rawJSON);

			bool showInitPopup = false;

			Data.SetDefault();

			if (jo["username"] == null) showInitPopup = true;
			else Data.Username = (string)jo["username"];

			if (jo["bufferDays"] == null) showInitPopup = true;
			else Data.BufferDays = (int)jo["bufferDays"];

			if (jo["ignoreInactiveDays"] == null) showInitPopup = true;
			else Data.IgnoreInactiveDays = (bool)jo["ignoreInactiveDays"];

			if (jo["ignoreInit"] == null) showInitPopup = true;
			else Data.IgnoreInit = (bool)jo["ignoreInit"];

			if (jo["ignorePreReleases"] == null) showInitPopup = true;
			else Data.IgnorePreReleases = (bool)jo["ignorePreReleases"];

			if (jo["forceEpilogue"] == null) showInitPopup = true;
			else Data.ForceEpilogue = (bool)jo["forceEpilogue"];



			if (jo["theme"] == null) showInitPopup = true;
			else Data.Theme = (string)jo["theme"];

			if (jo["accent"] == null) showInitPopup = true;
			else Data.Accent = (string)jo["accent"];

			

			if (showInitPopup)
			{
				SaveSettings();
				CreateSettingsInitPopup();
			}
			ApplyVisualSettings();
		}

		public static void SaveSettings()
		{
			JObject jo = new();

			jo.Add("username", Data.Username);
			jo.Add("bufferDays", Data.BufferDays);
			jo.Add("ignoreInactiveDays", Data.IgnoreInactiveDays);
			jo.Add("ignoreInit", Data.IgnoreInit);
			jo.Add("ignorePreReleases", Data.IgnorePreReleases);
			jo.Add("forceEpilogue", Data.ForceEpilogue);

			jo.Add("theme", Data.Theme);
			jo.Add("accent", Data.Accent);

			if (!File.Exists(Constants.SettingsPath))
			{
				int sep = Constants.SettingsPath.LastIndexOf("/");

				Directory.CreateDirectory(Constants.SettingsPath.Substring(0, sep));
				File.CreateText(Constants.SettingsPath).Close();
			}

			File.WriteAllText(Constants.SettingsPath, jo.ToString());
		}

		public static void CreateSettingsInitPopup()
		{
			//TODO: Create popup
		}

		public static void ApplyVisualSettings()
		{
			if (!Constants.ThemeURIs.Keys.Contains(Data.Theme)) Data.Theme = "Auto";
			if (!Constants.AccentURIs.Keys.Contains(Data.Accent)) Data.Accent = "Blue";

			string accentString = Data.Accent;
			string themeString = Data.Theme;
			if (themeString == "Auto") themeString = Data.SystemTheme;

			Application.Current.Resources.MergedDictionaries[1].Source = new Uri(Constants.ThemeURIs[themeString], UriKind.Relative);
			Application.Current.Resources.MergedDictionaries[2].Source = new Uri(Constants.AccentURIs[accentString], UriKind.Relative);
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
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
			{
				object registryValueObject = key?.GetValue(RegistryValueName);
				if (registryValueObject == null)
				{
					return WindowsTheme.Light;
				}

				int registryValue = (int)registryValueObject;

				return registryValue > 0 ? WindowsTheme.Light : WindowsTheme.Dark;
			}
		}

		public ThemeWatcher()
		{
			SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

			string theme = GetWindowsTheme().ToString();
			SettingsHelper.Data.SystemTheme = theme;
		}

		public void Destroy()
		{
			SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
		}

		private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if (e.Category != UserPreferenceCategory.General) return;
			string theme = GetWindowsTheme().ToString();

			SettingsHelper.Data.SystemTheme = theme;
			SettingsHelper.ApplyVisualSettings();
		}
	}
}
