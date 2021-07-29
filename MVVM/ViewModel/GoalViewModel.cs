using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class GoalViewModel : ObservableObject
	{
		public RelayCommand BuiltinGoalButtonClick { get; set; }
		public RelayCommand UserGoalButtonClick { get; set; }
		public RelayCommand OnAddClicked { get; set; }

		public GoalPopupViewModel GoalPopup { get; set; }
		public EditableGoalPopupViewModel EditableGoalPopup { get; set; }

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

		private ObservableCollection<GoalEntryData> _userEntries = new();
		public ObservableCollection<GoalEntryData> UserEntries
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

			BuiltinGoalButtonClick = new RelayCommand(OnBuiltinGoalButtonClick);
			UserGoalButtonClick = new RelayCommand(OnUserGoalButtonClick);
			OnAddClicked = new RelayCommand(o => {
				EditableGoalPopup.SetParameters("Create Goal", false);
				MainVM.QueuePopup(EditableGoalPopup);
			});

			Update();
		}

		public void Update()
		{
			BuiltinEntries.Clear();
			UserEntries.Clear();

			BuiltinEntries.Add(GoalDataCalc.CalcTotalGoal(TotalGoalUUID, TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP));
			BuiltinEntries.Add(GoalDataCalc.CalcBattlepassGoal(BattlepassGoalUUID, TrackingDataHelper.CurrentSeasonData.ActiveBPLevel));
			BuiltinEntries.Add(GoalDataCalc.CalcLevelGoal(LevelGoalUUID, TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP));

			foreach (Goal g in TrackingDataHelper.Data.Goals)
			{
				UserEntries.Add(GoalDataCalc.CalcUserGoal(g, TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP));
			}

			if (GoalPopup.IsInitialized)
			{
				GoalEntryData ged;

				if (BuiltinEntries.Where(e => e.UUID == GoalPopup.UUID).Count() > 0) ged = BuiltinEntries.Where(e => e.UUID == GoalPopup.UUID).FirstOrDefault();
				else ged = UserEntries.Where(e => e.UUID == GoalPopup.UUID).FirstOrDefault();

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
			GoalPopup.SetData(UserEntries.Where(e => e.UUID == uuid).First());
			MainVM.QueuePopup(GoalPopup);
		}
	}
}
