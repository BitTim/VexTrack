using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
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
		
		public string Theme;
		public string SystemTheme; //Not part of saved settings file
		public string Accent;
		
		public Settings() {}
		public Settings(string username, int bufferDays, bool ignoreInactiveDays, bool ignoreInit, bool ignorePreReleases, string theme, string systemTheme, string accent)
		{
			(Username, BufferDays, IgnoreInactiveDays, IgnoreInit, IgnorePreReleases, Theme, SystemTheme, Accent) = (username, bufferDays, ignoreInactiveDays, ignoreInit, ignorePreReleases, theme, systemTheme, accent);
		}
	}

	public static class SettingsHelper
	{
		public static Settings Data { get; set; }
		
		public static void Update()
		{
			//NOTE: Maybe replace with call to MainVM.Update();
			SettingsViewModel SettingsVM = (SettingsViewModel)ViewModelManager.ViewModels["Settings"];
			SettingsVM.Update();
		}

		public static void InitSettings()
		{

		}

		public static void ConvertSettings()
		{

		}

		public static void LoadSettings()
		{
			//TODO: Implement this
		}

		public static void SaveSettings()
		{

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
			SettingsHelper.Update();
		}
	}
}
