using VexTrack.Core;
using VexTrack.Core.Helper;
using VexTrack.Core.Model.WPF;

namespace VexTrack.MVVM.ViewModel.Popups;

class AboutPopupViewModel : BasePopupViewModel
{
	public RelayCommand OnCheckUpdateClicked { get; }
	public RelayCommand OnForceUpdateClicked { get; }
	public RelayCommand OnCloseClicked { get; }

	public string Version => Constants.Version;

	public AboutPopupViewModel()
	{
		CanCancel = true;

		OnCloseClicked = new RelayCommand(_ => Close());
		OnCheckUpdateClicked = new RelayCommand(_ => UpdateHelper.CheckUpdateAsync());
		OnForceUpdateClicked = new RelayCommand(_ => UpdateHelper.CheckUpdateAsync(true));
	}
}