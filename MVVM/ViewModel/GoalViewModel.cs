using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel
{
	class GoalViewModel : ObservableObject
	{
		public RelayCommand BuiltinGoalButtonClick { get; set; }
		public RelayCommand UserGoalButtonClick { get; set; }
		public RelayCommand OnAddClicked { get; set; }

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

			BuiltinGoalButtonClick = new RelayCommand(OnBuiltinGoalButtonClick);
			UserGoalButtonClick = new RelayCommand(OnUserGoalButtonClick);
			OnAddClicked = new RelayCommand(o => { });

			Update();
		}

		public void Update()
		{
			BuiltinEntries.Clear();
			UserEntries.Clear();

			BuiltinEntries.Add(GoalDataCalc.CalcTotalGoal(BuiltinEntries.Count, TotalGoalUUID, TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP));
			BuiltinEntries.Add(GoalDataCalc.CalcBattlepassGoal(BuiltinEntries.Count, TotalGoalUUID, TrackingDataHelper.CurrentSeasonData.ActiveBPLevel));
			BuiltinEntries.Add(GoalDataCalc.CalcLevelGoal(BuiltinEntries.Count, TotalGoalUUID, TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP));

			foreach (Goal g in TrackingDataHelper.Data.Goals)
			{
				UserEntries.Add(GoalDataCalc.CalcUserGoal(UserEntries.Count, g, TrackingDataHelper.CurrentSeasonData.ActiveBPLevel, TrackingDataHelper.CurrentSeasonData.CXP));
			}
		}

		public void OnBuiltinGoalButtonClick(object parameter)
		{

		}

		public void OnUserGoalButtonClick(object parameter)
		{

		}
	}
}
