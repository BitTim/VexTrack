using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class DeleteGoalGroupConfirmationPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnYesClicked { get; set; }
		public RelayCommand OnNoClicked { get; set; }

		private bool _keepGoals = false;
		public bool KeepGoals
		{
			get { return _keepGoals; }
			set
			{
				_keepGoals = value;
				OnPropertyChanged();
			}
		}

		private string UUID = "";

		public DeleteGoalGroupConfirmationPopupViewModel()
		{
			CanCancel = true;

			OnYesClicked = new RelayCommand(o =>
			{
				if (KeepGoals)
				{
					foreach (Goal goal in TrackingDataHelper.Data.Goals.Find(gg => gg.UUID == UUID).Goals)
					{
						TrackingDataHelper.MoveGoal(UUID, Constants.DefaultGroupUUID, goal.UUID);
					}

					KeepGoals = false;
				}

				TrackingDataHelper.RemoveGoalGroup(UUID);
				Close();
			});
			OnNoClicked = new RelayCommand(o => Close());
		}

		public void SetData(string uuid)
		{
			UUID = uuid;
		}
	}
}
