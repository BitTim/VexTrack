using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using VexTrack.Core.Helper;
using VexTrack.MVVM.ViewModel;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.Core.IO;

public static class Updater
{
	public static async Task DownloadUpdate(string packageFile, string updaterFile, string tag, HttpClient client)
	{
		var buffer = new byte[1024 * 1024];

		var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
		var updateDownloadPopup = (UpdateDownloadPopupViewModel)ViewModelManager.ViewModels[nameof(UpdateDownloadPopupViewModel)];
		mainVm.QueuePopup(updateDownloadPopup);

		var packageUrl = Constants.BaseDownloadUrl + "/" + tag + "/UpdatePackage.zip";
		var updaterUrl = Constants.BaseDownloadUrl + "/" + tag + "/Updater.exe";

		var packageResponse = await client.GetAsync(packageUrl, HttpCompletionOption.ResponseHeadersRead);
		var updaterResponse = await client.GetAsync(updaterUrl, HttpCompletionOption.ResponseHeadersRead);

		var packageContentLengthNullable = packageResponse.Content.Headers.ContentLength;
		var updaterContentLengthNullable = updaterResponse.Content.Headers.ContentLength;

		if (packageContentLengthNullable == null || updaterContentLengthNullable == null) return;
		
		var packageContentLength = (long)packageContentLengthNullable;
		var updaterContentLength = (long)updaterContentLengthNullable;

		updateDownloadPopup.SetPackageData(packageContentLength, 0, 0);
		updateDownloadPopup.SetUpdaterData(updaterContentLength, 0, 0);



		// Download Update Package

		var packageStream = await packageResponse.Content.ReadAsStreamAsync();
		var packageFileStream = File.Create(packageFile);
		long packageTotalBytesRead = 0;
		var startTime = TimeHelper.NowTime;

		var i = 0;
		for (var len = await packageStream.ReadAsync(buffer.AsMemory(0, 1024 * 1024)); len != 0; len = await packageStream.ReadAsync(buffer.AsMemory(0, 1024 * 1024)))
		{

			packageTotalBytesRead += len;
			await packageFileStream.WriteAsync(buffer, 0, len);

			var packageProgress = CalcHelper.CalcProgress(packageContentLength, packageTotalBytesRead);
			updateDownloadPopup.SetPackageData(packageContentLength, packageTotalBytesRead, packageProgress);

			if (i % 15 == 0)
			{
				var downloadSpeed = packageTotalBytesRead / (TimeHelper.NowTime - startTime).TotalSeconds;
				updateDownloadPopup.SetDownloadSpeed(downloadSpeed);
			}
			i++;
		}

		await packageStream.DisposeAsync();
		await packageFileStream.DisposeAsync();



		// Download Updater

		var updaterStream = await updaterResponse.Content.ReadAsStreamAsync();
		var updaterFileStream = File.Create(updaterFile);
		long updaterTotalBytesRead = 0;
		startTime = TimeHelper.NowTime;

		i = 0;
		for (var len = await updaterStream.ReadAsync(buffer.AsMemory(0, 1024 * 1024)); len != 0; len = await updaterStream.ReadAsync(buffer.AsMemory(0, 1024 * 1024)))
		{

			updaterTotalBytesRead += len;
			await updaterFileStream.WriteAsync(buffer, 0, len);

			var updaterProgress = CalcHelper.CalcProgress(updaterContentLength, updaterTotalBytesRead);
			updateDownloadPopup.SetUpdaterData(updaterContentLength, updaterTotalBytesRead, updaterProgress);

			if (i % 15 == 0)
			{
				var downloadSpeed = updaterTotalBytesRead / (TimeHelper.NowTime - startTime).TotalSeconds;
				updateDownloadPopup.SetDownloadSpeed(downloadSpeed);
			}
			i++;
		}

		await updaterStream.DisposeAsync();
		await updaterFileStream.DisposeAsync();
	}

	public static int ExtractUpdate(string sourceFile, string extractTarget)
	{
		ZipFile.ExtractToDirectory(sourceFile, extractTarget);

		return !File.Exists(extractTarget + Constants.ManifestFile) ? 1 : 0;
	}

	public static int ApplyUpdate(string applierFile, string extractTarget, string installPath, string newVersionString)
	{
		Application.Current.Shutdown();

		var rawJson = File.ReadAllText(extractTarget + Constants.ManifestFile);
		var jo = JObject.Parse(rawJson);

		var fetchedVersionString = (string)jo["version"];
		if (fetchedVersionString != newVersionString) return 1;

		var fileList = jo["files"]!.Cast<JValue>().Aggregate("", (current, file) => current + (file + "\n"));

		if (!File.Exists(extractTarget + Constants.FileListFile)) File.CreateText(extractTarget + Constants.FileListFile).Close();
		File.WriteAllText(extractTarget + Constants.FileListFile, fileList);

		ProcessStartInfo startInfo = new()
		{
			FileName = applierFile,
			Arguments = "\"" + extractTarget + "\" \"" + installPath + "\" \"" + extractTarget + Constants.FileListFile + "\" \"" + Constants.AppName + ".exe\"",
			WorkingDirectory = Constants.UpdateFolder,
			UseShellExecute = true,
			Verb = "runas"
		};

		Process.Start(startInfo);
		return 0;
	}
}