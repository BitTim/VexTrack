using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class ResetDataConfirmationPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnYesClicked { get; }
		public RelayCommand OnNoClicked { get; }

		public ResetDataConfirmationPopupViewModel()
		{
			CanCancel = true;

			OnYesClicked = new RelayCommand(_ =>
			{
				TrackingData.ResetData();
				Close();
			});
			OnNoClicked = new RelayCommand(_ => Close());
		}
	}
}
