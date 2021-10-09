using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
			byte[] buffer = new byte[1024 * 1024];
			int i = 0;
			DateTimeOffset startTime;

			MainViewModel MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
			UpdateDownloadPopupViewModel UpdateDownloadPopup = (UpdateDownloadPopupViewModel)ViewModelManager.ViewModels["UpdateDownloadPopup"];
			MainVM.QueuePopup(UpdateDownloadPopup);

			string packageURL = Constants.BaseDownloadURL + "/" + tag + "/UpdatePackage.zip";
			string updaterURL = Constants.BaseDownloadURL + "/" + tag + "/Updater.exe";

			HttpResponseMessage packageResponse = await client.GetAsync(packageURL, HttpCompletionOption.ResponseHeadersRead);
			HttpResponseMessage updaterResponse = await client.GetAsync(updaterURL, HttpCompletionOption.ResponseHeadersRead);

			long packageContentLength = (long)packageResponse.Content.Headers.ContentLength;
			long updaterContentLength = (long)updaterResponse.Content.Headers.ContentLength;

			UpdateDownloadPopup.SetPackageData(packageContentLength, 0, 0);
			UpdateDownloadPopup.SetUpdaterData(updaterContentLength, 0, 0);



			// Downlaod Update Package

			Stream packageStream = await packageResponse.Content.ReadAsStreamAsync();
			FileStream packageFileStream = File.Create(packageFile);
			long packageTotalBytesRead = 0;
			startTime = DateTimeOffset.Now;

			i = 0;
			for(int len = packageStream.Read(buffer, 0, 1024 * 1024); len != 0; len = packageStream.Read(buffer, 0, 1024 * 1024))
			{

				packageTotalBytesRead += len;
				await packageFileStream.WriteAsync(buffer, 0, len);

				double packageProgress = CalcUtil.CalcProgress(packageContentLength, packageTotalBytesRead);
				UpdateDownloadPopup.SetPackageData(packageContentLength, packageTotalBytesRead, packageProgress);

				if (i % 15 == 0)
				{
					double downloadSpeed = packageTotalBytesRead / (DateTimeOffset.Now - startTime).TotalSeconds;
					UpdateDownloadPopup.SetDownloadSpeed(downloadSpeed);
				}
				i++;
			}

			packageStream.Dispose();
			packageFileStream.Dispose();



			// Download Updater

			Stream updaterStream = await updaterResponse.Content.ReadAsStreamAsync();
			FileStream updaterFileStream = File.Create(updaterFile);
			long updaterTotalBytesRead = 0;
			startTime = DateTimeOffset.Now;

			i = 0;
			for (int len = updaterStream.Read(buffer, 0, 1024 * 1024); len != 0; len = updaterStream.Read(buffer, 0, 1024 * 1024))
			{

				updaterTotalBytesRead += len;
				await updaterFileStream.WriteAsync(buffer, 0, len);

				double updaterProgress = CalcUtil.CalcProgress(updaterContentLength, updaterTotalBytesRead);
				UpdateDownloadPopup.SetUpdaterData(updaterContentLength, updaterTotalBytesRead, updaterProgress);

				if(i % 15  == 0)
				{
					double downloadSpeed = updaterTotalBytesRead / (DateTimeOffset.Now - startTime).TotalSeconds;
					UpdateDownloadPopup.SetDownloadSpeed(downloadSpeed);
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

			string rawJSON = File.ReadAllText(extractTarget + Constants.ManifestFile);
			JObject jo = JObject.Parse(rawJSON);

			string NewVersionString = (string)jo["version"];
			if (NewVersionString != newVersionString) return 1;

			string fileList = "";
			foreach(JValue file in jo["files"])
			{
				fileList += file.ToString() + "\n";
			}

			if(!File.Exists(extractTarget + Constants.FileListFile)) File.CreateText(extractTarget + Constants.FileListFile).Close();
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
			double size = rawSize;
			string unit = " B";

			if (isSpeed) unit = " B/s";

			int divisions = 0;
			while(size > 1)
			{
				size /= 1024;
				divisions++;

				if (divisions > 3) break;
			}

			if(divisions > 0)
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
		private static readonly HttpClient client = new HttpClient();
		private static string latestVersionTag = "";

		private static string UpdaterFile = Constants.UpdateFolder + "/Updater.exe";
		private static string SourceFile = Constants.UpdateFolder + "/UpdatePackage.zip";
		private static string ExtractTarget = Constants.UpdateFolder + "/ExtractedPackage";

		public static async void CheckUpdateAsync(bool forceUpdate = false)
		{
			if(Directory.Exists(Constants.UpdateFolder)) Directory.Delete(Constants.UpdateFolder, true);

			HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(Constants.ReleasesURL), Method = HttpMethod.Get };
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			request.Headers.UserAgent.Add(new ProductInfoHeaderValue(Constants.AppName, Constants.Version));

			HttpResponseMessage response = await client.SendAsync(request);
			string res = await response.Content.ReadAsStringAsync();

			JArray ja = JArray.Parse(res);
			foreach(JObject release in ja)
			{
				List<string> tokenizedName = release["name"].ToString().Split().ToList();
				if (tokenizedName[0] != Constants.AppName) continue;

				float newestVersion = float.Parse(tokenizedName[1].Split("v")[1]);
				float currentVersion = float.Parse(Constants.Version.Split("v")[1]);

				if (currentVersion >= newestVersion && !forceUpdate) break;
				if ((bool)release["prerelease"] && SettingsHelper.Data.IgnorePreReleases) continue;

				latestVersionTag = (string)release["tag_name"];

				string rawDesc = (string)release["body"];
				List<string> desc = rawDesc.Split("##").ToList();

				List<string> changelog = new();
				List<string> warnings = new();

				foreach (string d in  desc)
				{
					List<string> splitDesc = d.Split("\r\n").ToList();
					splitDesc.RemoveAll(x => string.IsNullOrWhiteSpace(x));

					for(int i = 0; i < splitDesc.Count; i++)
					{
						if(splitDesc[i].StartsWith("-")) splitDesc[i] = splitDesc[i].Substring(1);
						if(splitDesc[i].StartsWith(" ")) splitDesc[i] = splitDesc[i].Substring(1);
					}

					if (splitDesc.Count < 2) continue;

					if(splitDesc[0].Contains("Changelog")) changelog = splitDesc.GetRange(1, splitDesc.Count - 1).ToList();
					if(splitDesc[0].Contains("Warning")) warnings = splitDesc.GetRange(1, splitDesc.Count - 1).ToList();
				}

				if ((bool)release["prerelease"]) warnings.Add("This release is a pre-release");

				UpdateAvailablePopupViewModel UpdateAvailablePopup = (UpdateAvailablePopupViewModel)ViewModelManager.ViewModels["UpdateAvailablePopup"];
				MainViewModel MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];

				UpdateAvailablePopup.SetData(changelog, warnings);
				MainVM.QueuePopup(UpdateAvailablePopup);

				break;
			}
		}

		public static async void GetUpdate()
		{
			if(!Directory.Exists(Constants.UpdateFolder)) Directory.CreateDirectory(Constants.UpdateFolder);

			await UpdateUtil.DownloadUpdate(SourceFile, UpdaterFile, latestVersionTag, client);
			int extractResult = UpdateUtil.ExtractUpdate(SourceFile, ExtractTarget);

			if (extractResult == 1)
			{
				MainViewModel MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
				UpdateFailedPopupViewModel popup = new UpdateFailedPopupViewModel();

				MainVM.PopupQueue.LastOrDefault().CanCancel = true;
				MainVM.PopupQueue.LastOrDefault().Close();

				popup.SetData("Invalid Update Package: Manifest file missing or corrupted");
				MainVM.QueuePopup(popup);
				return;
			}

			int applyResult = UpdateUtil.ApplyUpdate(UpdaterFile, ExtractTarget, Environment.CurrentDirectory, latestVersionTag);

			if (applyResult == 1)
			{
				MainViewModel MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
				UpdateFailedPopupViewModel popup = new UpdateFailedPopupViewModel();

				MainVM.PopupQueue.LastOrDefault().CanCancel = true;
				MainVM.PopupQueue.LastOrDefault().Close();

				popup.SetData("Invalid Update Package: Manifest version and Tag version mismatch");
				MainVM.QueuePopup(popup);
				return;
			}
		}
	}
}
