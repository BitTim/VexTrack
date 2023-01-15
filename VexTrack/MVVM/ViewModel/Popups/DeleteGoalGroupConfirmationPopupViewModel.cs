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

		private string _uuid = "";

		public DeleteGoalGroupConfirmationPopupViewModel()
		{
			CanCancel = true;

			OnYesClicked = new RelayCommand(o =>
			{
				if (KeepGoals)
				{
					foreach (var goal in TrackingData.Contracts.Find(gg => gg.Uuid == _uuid).Goals)
					{
						TrackingData.MoveGoal(_uuid, Constants.DefaultGroupUuid, goal.Uuid);
					}

					KeepGoals = false;
				}

				TrackingData.RemoveContract(_uuid);
				Close();
			});
			OnNoClicked = new RelayCommand(o => Close());
		}

		public void SetData(string uuid)
		{
			_uuid = uuid;
		}
	}
}
