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
using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.Core
{
	public static class UpdateUtil
	{
		public static async Task DownloadUpdate(string packageFile, string updaterFile, string tag, HttpClient client)
		{
			string updaterURL = Constants.BaseDownloadURL + "/" + tag + "/Updater.exe";
			string packageURL = Constants.BaseDownloadURL + "/" + tag + "/UpdatePackage.zip";

			HttpResponseMessage packageResponse = await client.GetAsync(packageURL, HttpCompletionOption.ResponseHeadersRead);

			Stream packageStream = await packageResponse.Content.ReadAsStreamAsync();
			FileStream packageFileStream = File.Create(packageFile);
			long packageContentLength = (long)packageResponse.Content.Headers.ContentLength;
			long packageTotalBytesRead = 0;

			while (true)
			{
				DateTimeOffset startTime = DateTimeOffset.Now;

				byte[] chunk = new byte[1024 * 1024];
				await packageStream.ReadAsync(chunk);

				if (packageStream.Position >= packageContentLength) break;

				packageTotalBytesRead += Buffer.ByteLength(chunk);
				await packageFileStream.WriteAsync(chunk);


				Trace.WriteLine("Total: " + packageContentLength + "B | Read: " +  packageTotalBytesRead.ToString() + "B | Progress: " + Math.Round((double)(packageTotalBytesRead / packageContentLength)).ToString() + "% | Speed: " + Math.Round(packageTotalBytesRead / (DateTimeOffset.Now - startTime).TotalSeconds).ToString() + " B/s");

			}
			packageStream.Dispose();
			packageFileStream.Dispose();
		}

		public static void ExtractUpdate(string sourceFile, string extractTarget)
		{
			ZipFile.ExtractToDirectory(sourceFile, extractTarget);
		}

		public static void ApplyUpdate(string applierFile)
		{

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
			UpdateUtil.ExtractUpdate(SourceFile, ExtractTarget);
			UpdateUtil.ApplyUpdate(UpdaterFile);
		}
	}
}
