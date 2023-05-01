using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Model;
using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.Core.Util;


public static class UpdateHelper
{
	private static readonly HttpClient Client = new();
	private static string _latestVersionTag = "";

	private static readonly string UpdaterFile = Constants.UpdateFolder + "/Updater.exe";
	private static readonly string SourceFile = Constants.UpdateFolder + "/UpdatePackage.zip";
	private static readonly string ExtractTarget = Constants.UpdateFolder + "/ExtractedPackage";

	public static async void CheckUpdateAsync(bool forceUpdate = false)
	{
		if (Directory.Exists(Constants.UpdateFolder)) Directory.Delete(Constants.UpdateFolder, true);

		var request = new HttpRequestMessage() { RequestUri = new Uri(Constants.ReleasesUrl), Method = HttpMethod.Get };
		request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		request.Headers.UserAgent.Add(new ProductInfoHeaderValue(Constants.AppName, Constants.Version));

		var response = await Client.SendAsync(request);
		var res = await response.Content.ReadAsStringAsync();

		List<string> changelog = new();
		List<string> warnings = new();
		var collectChangelog = false;
		var showDialog = false;
		var forceUpdateSkippedOnce = false;

		var ja = JArray.Parse(res);
		foreach (var jToken in ja)
		{
			var release = (JObject)jToken;
			var tokenizedName = release["name"]!.ToString().Split().ToList();
			if (tokenizedName[0] != Constants.AppName) continue;

			var newestVersion = float.Parse(tokenizedName[1].Split("v")[1]);
			var currentVersion = float.Parse(Constants.Version.Split("v")[1]);

			if (forceUpdateSkippedOnce) break;
			if (currentVersion >= newestVersion)
            {
				if (!forceUpdate) break;
				forceUpdateSkippedOnce = true;
            }

			if ((bool)release["prerelease"] && SettingsHelper.Data.IgnorePreReleases) continue;

			_latestVersionTag = (string)release["tag_name"];

			var rawDesc = (string)release["body"];
			var desc = rawDesc!.Split("##").ToList();

			List<string> requiredVersion = new();

			foreach (var d in desc)
			{
				var splitDesc = d.Split("\r\n").ToList();
				splitDesc.RemoveAll(string.IsNullOrWhiteSpace);

				for (var i = 0; i < splitDesc.Count; i++)
				{
					if (splitDesc[i].StartsWith("-")) splitDesc[i] = splitDesc[i].Substring(1);
					if (splitDesc[i].StartsWith(" ")) splitDesc[i] = splitDesc[i].Substring(1);
				}

				if (splitDesc.Count < 2) continue;

				if (splitDesc[0].Contains("Changelog")) changelog.AddRange(splitDesc.GetRange(1, splitDesc.Count - 1).ToList());
				if (splitDesc[0].Contains("Warning")) warnings.AddRange(splitDesc.GetRange(1, splitDesc.Count - 1).ToList());
				if (splitDesc[0].Contains("Required Version")) requiredVersion = splitDesc.GetRange(1, splitDesc.Count - 1).ToList();
			}

			if (!requiredVersion.Contains(Constants.Version) && !collectChangelog) continue;

			if ((bool)release["prerelease"] && !collectChangelog) warnings.Add("This release is a pre-release");
			collectChangelog = true;
			showDialog = true;
		}

		if (!showDialog) return;
		var updateAvailablePopup = (UpdateAvailablePopupViewModel)ViewModelManager.ViewModels[nameof(UpdateAvailablePopupViewModel)];
		var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];

		updateAvailablePopup.SetData(changelog, warnings, _latestVersionTag);
		mainVm.QueuePopup(updateAvailablePopup);
	}

	public static async void GetUpdate()
	{
		if (!Directory.Exists(Constants.UpdateFolder)) Directory.CreateDirectory(Constants.UpdateFolder);

		await Update.DownloadUpdate(SourceFile, UpdaterFile, _latestVersionTag, Client);
		var extractResult = Update.ExtractUpdate(SourceFile, ExtractTarget);
		
		var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
		var popup = new UpdateFailedPopupViewModel();

		if (extractResult == 1)
		{
			mainVm.PopupQueue.LastOrDefault()!.CanCancel = true;
			mainVm.PopupQueue.LastOrDefault()!.Close();

			popup.SetData("Invalid Update Package: Manifest file missing or corrupted");
			mainVm.QueuePopup(popup);
			return;
		}

		var applyResult = Update.ApplyUpdate(UpdaterFile, ExtractTarget, Environment.CurrentDirectory, _latestVersionTag);

		if (applyResult != 1) return;
		
		mainVm.PopupQueue.LastOrDefault()!.CanCancel = true;
		mainVm.PopupQueue.LastOrDefault()!.Close();

		popup.SetData("Invalid Update Package: Manifest version and Tag version mismatch");
		mainVm.QueuePopup(popup);
	}
}

