using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class DeleteGoalConfirmationPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnYesClicked { get; set; }
		public RelayCommand OnNoClicked { get; set; }

		private string _groupUuid = "";
		private string _uuid = "";

		public DeleteGoalConfirmationPopupViewModel()
		{
			CanCancel = true;

			OnYesClicked = new RelayCommand(o =>
			{
				// Delete here
				Close();
			});
			OnNoClicked = new RelayCommand(o => Close());
		}

		public void SetData(string groupUuid, string uuid)
		{
			_groupUuid = groupUuid;
			_uuid = uuid;
		}
	}
}
