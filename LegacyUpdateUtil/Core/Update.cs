using LegacyUpdateUtil.MVVM.ViewModel;
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

namespace LegacyUpdateUtil.Core
{
	public static class UpdateUtil
	{
		public static async Task DownloadUpdate(string packageFile, string updaterFile, string tag, HttpClient client)
		{
			var buffer = new byte[1024 * 1024];
			var i = 0;
			DateTimeOffset startTime;

			var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];

			var packageUrl = Constants.BaseDownloadUrl + "/" + tag + "/UpdatePackage.zip";
			var updaterUrl = Constants.BaseDownloadUrl + "/" + tag + "/Updater.exe";

			var packageResponse = await client.GetAsync(packageUrl, HttpCompletionOption.ResponseHeadersRead);
			var updaterResponse = await client.GetAsync(updaterUrl, HttpCompletionOption.ResponseHeadersRead);

			var packageContentLength = (long)packageResponse.Content.Headers.ContentLength;
			var updaterContentLength = (long)updaterResponse.Content.Headers.ContentLength;

			mainVm.SetPackageData(packageContentLength, 0, 0);
			mainVm.SetUpdaterData(updaterContentLength, 0, 0);



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
				mainVm.SetPackageData(packageContentLength, packageTotalBytesRead, packageProgress);

				if (i % 15 == 0)
				{
					var downloadSpeed = packageTotalBytesRead / (DateTimeOffset.Now - startTime).TotalSeconds;
					mainVm.SetDownloadSpeed(downloadSpeed);
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
				mainVm.SetUpdaterData(updaterContentLength, updaterTotalBytesRead, updaterProgress);

				if (i % 15 == 0)
				{
					var downloadSpeed = updaterTotalBytesRead / (DateTimeOffset.Now - startTime).TotalSeconds;
					mainVm.SetDownloadSpeed(downloadSpeed);
				}
				i++;
			}

			updaterStream.Dispose();
			updaterFileStream.Dispose();
		}

		public static void ExtractUpdate(string sourceFile, string extractTarget)
		{
			ZipFile.ExtractToDirectory(sourceFile, extractTarget);
		}

		public static void ApplyUpdate(string applierFile, string extractTarget, string installPath)
		{
			Application.Current.Shutdown();

			ProcessStartInfo startInfo = new();
			startInfo.FileName = applierFile;
			startInfo.Arguments = "\"" + extractTarget + "\" \"" + installPath + "\" \"" + Constants.AppName + ".exe\"";
			startInfo.WorkingDirectory = Constants.UpdateFolder;

			Process.Start(startInfo);
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

		private static string _updaterFile = Constants.UpdateFolder + "/Updater.exe";
		private static string _sourceFile = Constants.UpdateFolder + "/UpdatePackage.zip";
		private static string _extractTarget = Constants.UpdateFolder + "/ExtractedPackage";

		public static async void CheckUpdateAsync(bool forceUpdate = false)
		{
			if (Directory.Exists(Constants.UpdateFolder)) Directory.Delete(Constants.UpdateFolder, true);

			var request = new HttpRequestMessage() { RequestUri = new Uri(Constants.ReleasesUrl), Method = HttpMethod.Get };
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			request.Headers.UserAgent.Add(new ProductInfoHeaderValue(Constants.AppName, Constants.Version));

			var response = await Client.SendAsync(request);
			var res = await response.Content.ReadAsStringAsync();

			var ja = JArray.Parse(res);
			foreach (JObject release in ja)
			{
				List<string> tokenizedName = release["name"].ToString().Split().ToList();
				if (tokenizedName[0] != Constants.AppName) continue;
				if (tokenizedName[1] != Constants.UpdateVersion) continue;

				var newestVersion = float.Parse(tokenizedName[1].Split("v")[1]);
				var currentVersion = float.Parse(Constants.Version.Split("v")[1]);

				if (currentVersion >= newestVersion && !forceUpdate) break;
				//if ((bool)release["prerelease"]) continue;

				_latestVersionTag = (string)release["tag_name"];
				GetUpdate();
				break;
			}
		}

		public static async void GetUpdate()
		{
			if (!Directory.Exists(Constants.UpdateFolder)) Directory.CreateDirectory(Constants.UpdateFolder);

			await UpdateUtil.DownloadUpdate(_sourceFile, _updaterFile, _latestVersionTag, Client);
			UpdateUtil.ExtractUpdate(_sourceFile, _extractTarget);
			UpdateUtil.ApplyUpdate(_updaterFile, _extractTarget, Environment.CurrentDirectory);
		}
	}
}
