using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class ContractViewModel : ObservableObject
	{
		//public RelayCommand BuiltinGoalButtonClick { get; set; }
		public RelayCommand ContractButtonClick { get; set; }
		public RelayCommand OnAddClicked { get; set; }
		//public RelayCommand OnGroupAddClicked { get; set; }
		//public RelayCommand OnGroupEditClicked { get; set; }
		//public RelayCommand OnGroupDeleteClicked { get; set; }

		// TODO: Replace Popup with expanding panel
		public GoalPopupViewModel GoalPopup { get; set; }
		public EditableGoalPopupViewModel EditableGoalPopup { get; set; }
		public EditableGoalGroupPopupViewModel EditableGoalGroupPopup { get; set; }
		public DeleteGoalGroupConfirmationPopupViewModel DeleteGoalGroupConfirmationPopup { get; set; }

		private string _totalGoalUuid = Guid.NewGuid().ToString();
		private string _battlepassGoalUuid = Guid.NewGuid().ToString();
		private string _levelGoalUuid = Guid.NewGuid().ToString();

		private MainViewModel MainVm { get; set; }

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

		private ObservableCollection<ContractData> _userEntries = new();
		public ObservableCollection<ContractData> UserEntries
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

		public ContractViewModel()
		{
			MainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
			GoalPopup = (GoalPopupViewModel)ViewModelManager.ViewModels["GoalPopup"];
			EditableGoalPopup = (EditableGoalPopupViewModel)ViewModelManager.ViewModels["EditableGoalPopup"];
			EditableGoalGroupPopup = (EditableGoalGroupPopupViewModel)ViewModelManager.ViewModels["EditableGoalGroupPopup"];
			DeleteGoalGroupConfirmationPopup = (DeleteGoalGroupConfirmationPopupViewModel)ViewModelManager.ViewModels["DeleteGoalGroupConfirmationPopup"];

			BuiltinGoalButtonClick = new RelayCommand(OnBuiltinGoalButtonClick);
			ContractButtonClick = new RelayCommand(OnUserGoalButtonClick);
			OnAddClicked = new RelayCommand(o =>
			{
				EditableGoalPopup.SetParameters("Create Goal", false);
				MainVm.QueuePopup(EditableGoalPopup);
			});
			OnGroupAddClicked = new RelayCommand(o =>
			{
				EditableGoalGroupPopup.SetParameters("Create Group", false);
				MainVm.QueuePopup(EditableGoalGroupPopup);
			});
			OnGroupEditClicked = new RelayCommand(o =>
			{
				EditableGoalGroupPopup.SetParameters("Edit Group", true);
				EditableGoalGroupPopup.SetData(UserEntries.Where(x => x.Uuid == (string)o).FirstOrDefault());
				MainVm.QueuePopup(EditableGoalGroupPopup);
			});
			OnGroupDeleteClicked = new RelayCommand(o =>
			{
				DeleteGoalGroupConfirmationPopup.SetData((string)o);
				MainVm.QueuePopup(DeleteGoalGroupConfirmationPopup);
			});

			Update(false);
		}

		public void Update(bool epilogue)
		{
			BuiltinEntries.Clear();
			UserEntries.Clear();

			BuiltinEntries.Add(GoalDataCalc.CalcTotalGoal(_totalGoalUuid, TrackingDataHelper.CurrentSeasonData.ActiveBpLevel, TrackingDataHelper.CurrentSeasonData.Cxp, epilogue));
			BuiltinEntries.Add(GoalDataCalc.CalcBattlepassGoal(_battlepassGoalUuid, TrackingDataHelper.CurrentSeasonData.ActiveBpLevel, epilogue));
			BuiltinEntries.Add(GoalDataCalc.CalcLevelGoal(_levelGoalUuid, TrackingDataHelper.CurrentSeasonData.ActiveBpLevel, TrackingDataHelper.CurrentSeasonData.Cxp));

			foreach (var gg in TrackingDataHelper.Data.Contracts)
			{
				List<GoalEntryData> ged = new();
				foreach (var g in gg.Goals)
					ged.Add(GoalDataCalc.CalcGoal(gg.Uuid, g));

				UserEntries.Add(new ContractData(gg.Uuid, gg.Name, ged));
			}

			if (GoalPopup.IsInitialized)
			{
				GoalEntryData ged;

				if (BuiltinEntries.Where(e => e.Uuid == GoalPopup.Uuid).Count() > 0) ged = BuiltinEntries.Where(e => e.Uuid == GoalPopup.Uuid).FirstOrDefault();
				else ged = (from gg in UserEntries
							from g in gg.Goals
							where g.Uuid == GoalPopup.Uuid
							select g).FirstOrDefault();

				GoalPopup.SetData(ged);
			}
			else GoalPopup.Close();
		}

		public void OnBuiltinGoalButtonClick(object parameter)
		{
			var uuid = (string)parameter;

			GoalPopup.SetFlags(false, false);
			GoalPopup.SetData(BuiltinEntries.Where(e => e.Uuid == uuid).First(), uuid == _battlepassGoalUuid ? "" : " XP");
			MainVm.QueuePopup(GoalPopup);
		}

		public void OnUserGoalButtonClick(object parameter)
		{
			var uuid = (string)parameter;

			GoalPopup.SetFlags(true, true);
			GoalPopup.SetData((from gg in UserEntries
							   from g in gg.Goals
							   where g.Uuid == uuid
							   select g).FirstOrDefault());
			MainVm.QueuePopup(GoalPopup);
		}
	}
}
