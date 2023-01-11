using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class SeasonEndConfirmationPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnYesClicked { get; set; }
		public RelayCommand OnNoClicked { get; set; }

		private string _uuid = "";
		public string Name {get; set; }
		public string EndDate { get; set; }


		public SeasonEndConfirmationPopupViewModel()
		{
			CanCancel = true;

			OnYesClicked = new RelayCommand(o =>
			{
				TrackingDataHelper.EndSeason(_uuid);
				Close();
			});
			OnNoClicked = new RelayCommand(o => Close());
		}

		public void SetData(string seasonUuid)
		{
			_uuid = seasonUuid;
			Name = TrackingDataHelper.GetSeason(_uuid).Name;
			EndDate = TrackingDataHelper.GetSeason(_uuid).EndDate;
		}
	}
}
