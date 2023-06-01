using VexTrack.Core.Model;
using VexTrack.Core.Model.WPF;

namespace VexTrack.MVVM.ViewModel.Popups;

class ResetDataConfirmationPopupViewModel : BasePopupViewModel
{
	public RelayCommand OnYesClicked { get; }
	public RelayCommand OnNoClicked { get; }

	public ResetDataConfirmationPopupViewModel()
	{
		CanCancel = true;

		OnYesClicked = new RelayCommand(_ =>
		{
			UserData.ResetData();
			Close();
		});
		OnNoClicked = new RelayCommand(_ => Close());
	}
}