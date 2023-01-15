using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.Core
{
	public static class UpdateUtil
	{
		public static async Task DownloadUpdate(string packageFile, string updaterFile, string tag, HttpClient client)
		{
			var buffer = new byte[1024 * 1024];
			var i = 0;
			DateTimeOffset startTime;

			var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
			var updateDownloadPopup = (UpdateDownloadPopupViewModel)ViewModelManager.ViewModels["UpdateDownloadPopup"];
			mainVm.QueuePopup(updateDownloadPopup);

			var packageUrl = Constants.BaseDownloadUrl + "/" + tag + "/UpdatePackage.zip";
			var updaterUrl = Constants.BaseDownloadUrl + "/" + tag + "/Updater.exe";

			var packageResponse = await client.GetAsync(packageUrl, HttpCompletionOption.ResponseHeadersRead);
			var updaterResponse = await client.GetAsync(updaterUrl, HttpCompletionOption.ResponseHeadersRead);

			var packageContentLength = (long)packageResponse.Content.Headers.ContentLength;
			var updaterContentLength = (long)updaterResponse.Content.Headers.ContentLength;

			updateDownloadPopup.SetPackageData(packageContentLength, 0, 0);
			updateDownloadPopup.SetUpdaterData(updaterContentLength, 0, 0);



			// Downlaod Update Package

			var packageStream = await packageResponse.Content.ReadAsStreamAsync();
			var packageFileStream = File.Create(packageFile);
			long packageTotalBytesRead = 0;
			startTime = DateTimeOffset.Now;

			i = 0;
			for (var len = packageStream.Read(buffer, 0, 1024 * 1024); len != 0; len = packageStream.Read(buffer, 0, 1024 * 1024))
			{

				packageTotalBytesRead += len;
				await packageFileStream.WriteAsync(buffer, 0, len);

				var packageProgress = CalcUtil.CalcProgress(packageContentLength, packageTotalBytesRead);
				updateDownloadPopup.SetPackageData(packageContentLength, packageTotalBytesRead, packageProgress);

				if (i % 15 == 0)
				{
					var downloadSpeed = packageTotalBytesRead / (DateTimeOffset.Now - startTime).TotalSeconds;
					updateDownloadPopup.SetDownloadSpeed(downloadSpeed);
				}
				i++;
			}

			packageStream.Dispose();
			packageFileStream.Dispose();



			// Download Updater

			var updaterStream = await updaterResponse.Content.ReadAsStreamAsync();
			var updaterFileStream = File.Create(updaterFile);
			long updaterTotalBytesRead = 0;
			startTime = DateTimeOffset.Now;

			i = 0;
			for (var len = updaterStream.Read(buffer, 0, 1024 * 1024); len != 0; len = updaterStream.Read(buffer, 0, 1024 * 1024))
			{

				updaterTotalBytesRead += len;
				await updaterFileStream.WriteAsync(buffer, 0, len);

				var updaterProgress = CalcUtil.CalcProgress(updaterContentLength, updaterTotalBytesRead);
				updateDownloadPopup.SetUpdaterData(updaterContentLength, updaterTotalBytesRead, updaterProgress);

				if (i % 15 == 0)
				{
					var downloadSpeed = updaterTotalBytesRead / (DateTimeOffset.Now - startTime).TotalSeconds;
					updateDownloadPopup.SetDownloadSpeed(downloadSpeed);
				}
				i++;
			}

			updaterStream.Dispose();
			updaterFileStream.Dispose();
		}

		public static int ExtractUpdate(string sourceFile, string extractTarget)
		{
			ZipFile.ExtractToDirectory(sourceFile, extractTarget);

			if (!File.Exists(extractTarget + Constants.ManifestFile)) return 1;
			return 0;
		}

		public static int ApplyUpdate(string applierFile, string extractTarget, string installPath, string newVersionString)
		{
			Application.Current.Shutdown();

			var rawJson = File.ReadAllText(extractTarget + Constants.ManifestFile);
			var jo = JObject.Parse(rawJson);

			var fetchedVersionString = (string)jo["version"];
			if (fetchedVersionString != newVersionString) return 1;

			var fileList = "";
			foreach (JValue file in jo["files"])
			{
				fileList += file.ToString() + "\n";
			}

			if (!File.Exists(extractTarget + Constants.FileListFile)) File.CreateText(extractTarget + Constants.FileListFile).Close();
			File.WriteAllText(extractTarget + Constants.FileListFile, fileList);

			ProcessStartInfo startInfo = new();
			startInfo.FileName = applierFile;
			startInfo.Arguments = "\"" + extractTarget + "\" \"" + installPath + "\" \"" + extractTarget + Constants.FileListFile + "\" \"" + Constants.AppName + ".exe\"";
			startInfo.WorkingDirectory = Constants.UpdateFolder;
			startInfo.UseShellExecute = true;
			startInfo.Verb = "runas";

			Process.Start(startInfo);
			return 0;
		}



		public static (double, string) FormatSize(double rawSize, bool isSpeed = false)
		{
			var size = rawSize;
			var unit = " B";

			if (isSpeed) unit = " B/s";

			var divisions = 0;
			while (size > 1)
			{
				size /= 1024;
				divisions++;

				if (divisions > 3) break;
			}

			if (divisions > 0)
			{
				size *= 1024;
				divisions--;
			}

			size = Math.Round(size, 2);

			if (divisions == 1) unit = unit.Insert(1, "K");
			if (divisions == 2) unit = unit.Insert(1, "M");
			if (divisions == 3) unit = unit.Insert(1, "G");

			return (size, unit);
		}
	}

	public static class UpdateHelper
	{
		private static readonly HttpClient Client = new HttpClient();
		private static string _latestVersionTag = "";

		private static readonly string _updaterFile = Constants.UpdateFolder + "/Updater.exe";
		private static readonly string _sourceFile = Constants.UpdateFolder + "/UpdatePackage.zip";
		private static readonly string _extractTarget = Constants.UpdateFolder + "/ExtractedPackage";

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
			foreach (JObject release in ja)
			{
				var tokenizedName = release["name"].ToString().Split().ToList();
				if (tokenizedName[0] != Constants.AppName) continue;

				var newestVersion = float.Parse(tokenizedName[1].Split("v")[1]);
				var currentVersion = float.Parse(Constants.Version.Split("v")[1]);

				if (forceUpdateSkippedOnce && forceUpdate) break;
				if (currentVersion >= newestVersion)
                {
					if (!forceUpdate) break;
					forceUpdateSkippedOnce = true;
                }

				if ((bool)release["prerelease"] && SettingsHelper.Data.IgnorePreReleases) continue;

				_latestVersionTag = (string)release["tag_name"];

				var rawDesc = (string)release["body"];
				var desc = rawDesc.Split("##").ToList();

				List<string> requiredVersion = new();

				foreach (var d in desc)
				{
					var splitDesc = d.Split("\r\n").ToList();
					splitDesc.RemoveAll(x => string.IsNullOrWhiteSpace(x));

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
			var updateAvailablePopup = (UpdateAvailablePopupViewModel)ViewModelManager.ViewModels["UpdateAvailablePopup"];
			var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];

			updateAvailablePopup.SetData(changelog, warnings, _latestVersionTag);
			mainVm.QueuePopup(updateAvailablePopup);
		}

		public static async void GetUpdate()
		{
			if (!Directory.Exists(Constants.UpdateFolder)) Directory.CreateDirectory(Constants.UpdateFolder);

			await UpdateUtil.DownloadUpdate(_sourceFile, _updaterFile, _latestVersionTag, Client);
			var extractResult = UpdateUtil.ExtractUpdate(_sourceFile, _extractTarget);

			if (extractResult == 1)
			{
				var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
				var popup = new UpdateFailedPopupViewModel();

				mainVm.PopupQueue.LastOrDefault().CanCancel = true;
				mainVm.PopupQueue.LastOrDefault().Close();

				popup.SetData("Invalid Update Package: Manifest file missing or corrupted");
				mainVm.QueuePopup(popup);
				return;
			}

			var applyResult = UpdateUtil.ApplyUpdate(_updaterFile, _extractTarget, Environment.CurrentDirectory, _latestVersionTag);

			if (applyResult == 1)
			{
				var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
				var popup = new UpdateFailedPopupViewModel();

				mainVm.PopupQueue.LastOrDefault().CanCancel = true;
				mainVm.PopupQueue.LastOrDefault().Close();

				popup.SetData("Invalid Update Package: Manifest version and Tag version mismatch");
				mainVm.QueuePopup(popup);
				return;
			}
		}
	}
}
