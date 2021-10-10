using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class GoalViewModel : ObservableObject
	{
		public RelayCommand BuiltinGoalButtonClick { get; set; }
		public RelayCommand UserGoalButtonClick { get; set; }
		public RelayCommand OnAddClicked { get; set; }
		public RelayCommand OnGroupAddClicked { get; set; }
		public RelayCommand OnGroupEditClicked { get; set; }
		public RelayCommand OnGroupDeleteClicked { get; set; }

		public GoalPopupViewModel GoalPopup { get; set; }
		public EditableGoalPopupViewModel EditableGoalPopup { get; set; }
		public EditableGoalGroupPopupViewModel EditableGoalGroupPopup { get; set; }
		public DeleteGoalGroupConfirmationPopupViewModel DeleteGoalGroupConfirmationPopup { get; set; }

		private string TotalGoalUUID = Guid.NewGuid().ToString();
		private string BattlepassGoalUUID = Guid.NewGuid().ToString();
		private string LevelGoalUUID = Guid.NewGuid().ToString();

		private MainViewModel MainVM { get; set; }

		private ObservableCollection<GoalEntryData> _builtinEntries = new();
		public ObservableCollection<GoalEntryData> BuiltinEntries
		{
			get => _builtinEntries;
			set
			{
				if (_builtinEntries != value)
				{
					_builtinEntries = value;
					OnPropertyChanged();
				}
			}
		}

		private ObservableCollection<GoalGroupData> _userEntries = new();
		public ObservableCollection<GoalGroupData> UserEntries
		{
			get => _userEntries;
			set
			{
				if (_userEntries != value)
				{
					_userEntries = value;
					OnPropertyChanged();
				}
			}
		}

		public GoalViewModel()
		{
			MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
			GoalPopup = (GoalPopupViewModel)ViewModelManager.ViewModels["GoalPopup"];
			EditableGoalPopup = (EditableGoalPopupViewModel)ViewModelManager.ViewModels["EditableGoalPopup"];
			EditableGoalGroupPopup = (EditableGoalGroupPopupViewModel)ViewModelManager.ViewModels["EditableGoalGroupPopup"];
			DeleteGoalGroupConfirmationPopup = (DeleteGoalGroupConfirmationPopupViewModel)ViewModelManager.ViewModels["DeleteGoalGroupConfirmationPopup"];

			BuiltinGoalButtonClick = new RelayCommand(OnBuiltinGoalButtonClick);
			UserGoalButtonClick = new RelayCommand(OnUserGoalButtonClick);
			OnAddClicked = new RelayCommand(o =>
			{
				EditableGoalPopup.SetParameters("Create Goal", false);
				MainVM.QueuePopup(EditableGoalPopup);
			});
			OnGroupAddClicked = new RelayCommand(o =>
			{
				EditableGoalGroupPopup.SetParameters("Create Group", false);
				MainVM.QueuePopup(EditableGoalGroupPopup);
			});
			OnGroupEditClicked = new RelayCommand(o =>
			{
				EditableGoalGroupPopup.SetParameters("Edit Group", true);
				EditableGoalGroupPopup.SetData(UserEntries.Where(x => x.UUID == (string)o).FirstOrDefault());
				MainVM.QueuePopup(EditableGoalGroupPopup);
			});
			OnGroupDeleteClicked = new RelayCommand(o =>
			{
				DeleteGoalGroupConfirmationPopup.SetData((string)o);
				MainVM.QueuePopup(DeleteGoalGroupConfirmationPopup);
			});

			Update(false);
		}

		public void Update(bool epilogue)
		{
			BuiltinEntries.Clear();
			UserEntries.Clear();

			BuiltinEntries.Add(GoalDataCalc.CalcTotalGoal(TotalGoalUUID, TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP, epilogue));
			BuiltinEntries.Add(GoalDataCalc.CalcBattlepassGoal(BattlepassGoalUUID, TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, epilogue));
			BuiltinEntries.Add(GoalDataCalc.CalcLevelGoal(LevelGoalUUID, TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP));

			foreach (GoalGroup gg in TrackingDataHelper.Data.Goals)
			{
				List<GoalEntryData> ged = new();
				foreach (Goal g in gg.Goals)
					ged.Add(GoalDataCalc.CalcUserGoal(gg.UUID, g));

				UserEntries.Add(new GoalGroupData(gg.UUID, gg.Name, ged));
			}

			if (GoalPopup.IsInitialized)
			{
				GoalEntryData ged;

				if (BuiltinEntries.Where(e => e.UUID == GoalPopup.UUID).Count() > 0) ged = BuiltinEntries.Where(e => e.UUID == GoalPopup.UUID).FirstOrDefault();
				else ged = (from gg in UserEntries
							from g in gg.Goals
							where g.UUID == GoalPopup.UUID
							select g).FirstOrDefault();

				GoalPopup.SetData(ged);
			}
			else GoalPopup.Close();
		}

		public void OnBuiltinGoalButtonClick(object parameter)
		{
			string uuid = (string)parameter;

			GoalPopup.SetFlags(false, false);
			GoalPopup.SetData(BuiltinEntries.Where(e => e.UUID == uuid).First(), uuid == BattlepassGoalUUID ? "" : " XP");
			MainVM.QueuePopup(GoalPopup);
		}

		public void OnUserGoalButtonClick(object parameter)
		{
			string uuid = (string)parameter;

			GoalPopup.SetFlags(true, true);
			GoalPopup.SetData((from gg in UserEntries
							   from g in gg.Goals
							   where g.UUID == uuid
							   select g).FirstOrDefault());
			MainVM.QueuePopup(GoalPopup);
		}
	}
}
