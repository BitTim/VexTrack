using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class ResetDataConfirmationPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnYesClicked { get; set; }
		public RelayCommand OnNoClicked { get; set; }

		public ResetDataConfirmationPopupViewModel()
		{
			CanCancel = true;

			OnYesClicked = new RelayCommand(o =>
			{
				TrackingData.ResetData();
				Close();
			});
			OnNoClicked = new RelayCommand(o => Close());
		}
	}
}
