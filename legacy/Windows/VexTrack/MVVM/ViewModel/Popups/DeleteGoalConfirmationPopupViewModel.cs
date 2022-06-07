using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class DeleteGoalConfirmationPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnYesClicked { get; set; }
		public RelayCommand OnNoClicked { get; set; }

		private string GroupUUID = "";
		private string UUID = "";

		public DeleteGoalConfirmationPopupViewModel()
		{
			CanCancel = true;

			OnYesClicked = new RelayCommand(o =>
			{
				TrackingDataHelper.RemoveGoal(GroupUUID, UUID);
				Close();
			});
			OnNoClicked = new RelayCommand(o => Close());
		}

		public void SetData(string groupUUID, string uuid)
		{
			GroupUUID = groupUUID;
			UUID = uuid;
		}
	}
}
