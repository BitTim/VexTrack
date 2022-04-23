using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class SeasonEndConfirmationPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnYesClicked { get; set; }
		public RelayCommand OnNoClicked { get; set; }

		private string UUID = "";
		public string Name {get; set; }
		public string EndDate { get; set; }


		public SeasonEndConfirmationPopupViewModel()
		{
			CanCancel = true;

			OnYesClicked = new RelayCommand(o =>
			{
				TrackingDataHelper.EndSeason(UUID);
				Close();
			});
			OnNoClicked = new RelayCommand(o => Close());
		}

		public void SetData(string seasonUUID)
		{
			UUID = seasonUUID;
			Name = TrackingDataHelper.GetSeason(UUID).Name;
			EndDate = TrackingDataHelper.GetSeason(UUID).EndDate;
		}
	}
}
