using AltoHttp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VexTrack.Core
{
	public static class UpdateUtil
	{
		public static void DownloadUpdate(string packageFile, string updaterFile, string tag)
		{
			HttpDownloader packageDownloader = new(Constants.BaseDownloadURL + "/" + tag + "/UpdatePackage.zip", packageFile);
			HttpDownloader updaterDownloader = new(Constants.BaseDownloadURL + "/" + tag + "/Updater.exe", packageFile);

			//TODO: Add Event Handlers for prorgess changed and finished

			//TODO: Start Update
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

		public static async void CheckUpdateAsync()
		{
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

				if (currentVersion >= newestVersion) break;
				if ((bool)release["prerelease"] && SettingsHelper.Data.IgnorePreReleases) continue;

				//TODO: Display Update Confirmation Dialog
			}
		}

		public static void GetUpdate(string versionTag)
		{
			string UpdaterFile = Constants.DataFolder + "/Updater.exe";
			string SourceFile = Constants.DataFolder + "/UpdatePackage.zip";
			string ExtractTarget = Constants.DataFolder + "/ExtractedPackage";

			UpdateUtil.DownloadUpdate(SourceFile, UpdaterFile, versionTag);
			UpdateUtil.ExtractUpdate(SourceFile, ExtractTarget);
			UpdateUtil.ApplyUpdate(UpdaterFile);
		}
	}
}
