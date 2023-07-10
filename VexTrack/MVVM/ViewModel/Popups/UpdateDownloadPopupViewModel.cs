using VexTrack.Core.Helper;

namespace VexTrack.MVVM.ViewModel.Popups;

class UpdateDownloadPopupViewModel : BasePopupViewModel
{
	private double _packageProgress;
	private double _updaterProgress;

	private double _packageSize;
	private double _updaterSize;

	private double _packageTotalSize;
	private double _updaterTotalSize;

	private double _downloadSpeed;

	private string _packageSizeUnit;
	private string _updaterSizeUnit;
	private string _packageTotalSizeUnit;
	private string _updaterTotalSizeUnit;
	private string _downloadSpeedUnit;

	public double PackageProgress
	{
		get => _packageProgress;
		set
		{
			_packageProgress = value;
			OnPropertyChanged();
		}
	}
	public double UpdaterProgress
	{
		get => _updaterProgress;
		set
		{
			_updaterProgress = value;
			OnPropertyChanged();
		}
	}

	public double PackageSize
	{
		get => _packageSize;
		set
		{
			_packageSize = value;
			OnPropertyChanged();
		}
	}
	public double UpdaterSize
	{
		get => _updaterSize;
		set
		{
			_updaterSize = value;
			OnPropertyChanged();
		}
	}

	public double PackageTotalSize
	{
		get => _packageTotalSize;
		set
		{
			_packageTotalSize = value;
			OnPropertyChanged();
		}
	}
	public double UpdaterTotalSize
	{
		get => _updaterTotalSize;
		set
		{
			_updaterTotalSize = value;
			OnPropertyChanged();
		}
	}

	public double DownloadSpeed
	{
		get => _downloadSpeed;
		set
		{
			_downloadSpeed = value;
			OnPropertyChanged();
		}
	}

	public string PackageSizeUnit
	{
		get => _packageSizeUnit;
		set
		{
			_packageSizeUnit = value;
			OnPropertyChanged();
		}
	}
	public string UpdaterSizeUnit
	{
		get => _updaterSizeUnit;
		set
		{
			_updaterSizeUnit = value;
			OnPropertyChanged();
		}
	}
	public string PackageTotalSizeUnit
	{
		get => _packageTotalSizeUnit;
		set
		{
			_packageTotalSizeUnit = value;
			OnPropertyChanged();
		}
	}
	public string UpdaterTotalSizeUnit
	{
		get => _updaterTotalSizeUnit;
		set
		{
			_updaterTotalSizeUnit = value;
			OnPropertyChanged();
		}
	}
	public string DownloadSpeedUnit
	{
		get => _downloadSpeedUnit;
		set
		{
			_downloadSpeedUnit = value;
			OnPropertyChanged();
		}
	}

	public void SetDownloadSpeed(double speed)
	{
		(DownloadSpeed, DownloadSpeedUnit) = FormatHelper.FormatSize(speed, true);
	}

	public void SetPackageData(long total, long size, double progress)
	{
		(PackageTotalSize, PackageTotalSizeUnit) = FormatHelper.FormatSize(total);
		(PackageSize, PackageSizeUnit) = FormatHelper.FormatSize(size);
		PackageProgress = progress;
	}

	public void SetUpdaterData(long total, long size, double progress)
	{
		(UpdaterTotalSize, UpdaterTotalSizeUnit) = FormatHelper.FormatSize(total);
		(UpdaterSize, UpdaterSizeUnit) = FormatHelper.FormatSize(size);
		UpdaterProgress = progress;
	}
}