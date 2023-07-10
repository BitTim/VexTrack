using System;
using System.IO;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Model;
using VexTrack.MVVM.ViewModel;

namespace VexTrack.Core.Helper;

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