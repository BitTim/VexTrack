using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class GoalPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnEditClicked { get; set; }
		public RelayCommand OnDeleteClicked { get; set; }
		private EditableGoalPopupViewModel EditableGoalPopup { get; set; }
		private DeleteGoalConfirmationPopupViewModel DeleteGoalConfirmationPopup { get; set; }

		private GoalEntryData RawData { get; set; }
		public string Uuid { get; set; }
		public string GroupUuid { get; set; }
		public string DepUuid { get; set; }
		public string Title { get; set; }
		public string Unit { get; set; }
		public int Collected { get; set; }
		public int Remaining { get; set; }
		public int Total { get; set; }
		public double Progress { get; set; }
		public int Active { get; set; }
		public string Color { get; set; }

		public bool CanDelete { get; set; }
		public bool CanEdit { get; set; }

		private bool PausedState { get; set; }
		public bool Paused
		{
			get => PausedState;
			set
			{
				if (value == PausedState) return;

				PausedState = value;
				RawData.Paused = PausedState;
				TrackingDataHelper.EditGoal(GroupUuid, Uuid, new Goal(Uuid, Title, Total, Collected, Color, DepUuid, PausedState));

				OnPropertyChanged();
			}
		}

		public GoalPopupViewModel()
		{
			EditableGoalPopup = (EditableGoalPopupViewModel)ViewModelManager.ViewModels["EditableGoalPopup"];
			DeleteGoalConfirmationPopup = (DeleteGoalConfirmationPopupViewModel)ViewModelManager.ViewModels["DeleteGoalConfirmationPopup"];
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { Close(); });
			OnEditClicked = new RelayCommand(o =>
			{
				EditableGoalPopup.SetParameters("Edit Goal", true);
				EditableGoalPopup.SetData(RawData);
				MainVm.QueuePopup(EditableGoalPopup);
			});
			OnDeleteClicked = new RelayCommand(o =>
			{
				IsInitialized = false;
				DeleteGoalConfirmationPopup.SetData(GroupUuid, Uuid);
				MainVm.QueuePopup(DeleteGoalConfirmationPopup);
			});
		}

		public void SetFlags(bool canDelete, bool canEdit)
		{
			CanDelete = canDelete;
			CanEdit = canEdit;
		}

		public void SetData(GoalEntryData data, string unit = " XP")
		{
			if (data == null) return;
			RawData = data;

			Uuid = data.Uuid;
			GroupUuid = data.GroupUuid;
			DepUuid = data.DepUuid;
			Title = data.Title;
			Collected = data.Collected;
			Remaining = data.Remaining;
			Total = data.Total;
			Progress = data.Progress;
			Active = data.Active;
			Color = data.Color;
			Unit = unit;
			Paused = data.Paused;

			IsInitialized = true;
		}

		public override void Close()
		{
			EditableGoalPopup.Close();
			base.Close();
		}
	}
}
