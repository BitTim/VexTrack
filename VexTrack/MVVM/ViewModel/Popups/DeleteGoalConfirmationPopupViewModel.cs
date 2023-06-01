using VexTrack.Core.Model.WPF;

namespace VexTrack.MVVM.ViewModel.Popups;

class DeleteGoalConfirmationPopupViewModel : BasePopupViewModel
{
	public RelayCommand OnYesClicked { get; }
	public RelayCommand OnNoClicked { get; }

	public DeleteGoalConfirmationPopupViewModel()
	{
		CanCancel = true;

		OnYesClicked = new RelayCommand(_ =>
		{
			// Delete here
			Close();
		});
		OnNoClicked = new RelayCommand(_ => Close());
	}
}