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
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LegacyUpdateUtil.Core
{
	public static class UpdateUtil
	{
		public static async Task DownloadUpdate(string packageFile, string updaterFile, string tag, HttpClient client)
		{
			byte[] buffer = new byte[1024 * 1024];
			int i = 0;
			DateTimeOffset startTime;

			MainViewModel MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];

			string packageURL = Constants.BaseDownloadURL + "/" + tag + "/UpdatePackage.zip";
			string updaterURL = Constants.BaseDownloadURL + "/" + tag + "/Updater.exe";

			HttpResponseMessage packageResponse = await client.GetAsync(packageURL, HttpCompletionOption.ResponseHeadersRead);
			HttpResponseMessage updaterResponse = await client.GetAsync(updaterURL, HttpCompletionOption.ResponseHeadersRead);

			long packageContentLength = (long)packageResponse.Content.Headers.ContentLength;
			long updaterContentLength = (long)updaterResponse.Content.Headers.ContentLength;

			MainVM.SetPackageData(packageContentLength, 0, 0);
			MainVM.SetUpdaterData(updaterContentLength, 0, 0);



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
				MainVM.SetPackageData(packageContentLength, packageTotalBytesRead, packageProgress);

				if (i % 15 == 0)
				{
					double downloadSpeed = packageTotalBytesRead / (DateTimeOffset.Now - startTime).TotalSeconds;
					MainVM.SetDownloadSpeed(downloadSpeed);
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
				MainVM.SetUpdaterData(updaterContentLength, updaterTotalBytesRead, updaterProgress);

				if(i % 15  == 0)
				{
					double downloadSpeed = updaterTotalBytesRead / (DateTimeOffset.Now - startTime).TotalSeconds;
					MainVM.SetDownloadSpeed(downloadSpeed);
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
				if (tokenizedName[1] != Constants.UpdateVersion) continue;

				float newestVersion = float.Parse(tokenizedName[1].Split("v")[1]);
				float currentVersion = float.Parse(Constants.Version.Split("v")[1]);

				if (currentVersion >= newestVersion && !forceUpdate) break;
				//if ((bool)release["prerelease"]) continue;

				latestVersionTag = (string)release["tag_name"];
				GetUpdate();
				break;
			}
		}

		public static async void GetUpdate()
		{
			if(!Directory.Exists(Constants.UpdateFolder)) Directory.CreateDirectory(Constants.UpdateFolder);

			await UpdateUtil.DownloadUpdate(SourceFile, UpdaterFile, latestVersionTag, client);
			UpdateUtil.ExtractUpdate(SourceFile, ExtractTarget);
			UpdateUtil.ApplyUpdate(UpdaterFile, ExtractTarget, Environment.CurrentDirectory);
		}
	}
}
