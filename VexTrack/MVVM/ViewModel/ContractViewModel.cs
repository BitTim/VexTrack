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
		public RelayCommand ContractButtonClick { get; set; }
		public RelayCommand OnAddClicked { get; set; }
		public RelayCommand OnGroupAddClicked { get; set; }
		public RelayCommand OnGroupEditClicked { get; set; }
		public RelayCommand OnGroupDeleteClicked { get; set; }

		// TODO: Replace Popup with expanding panel
		public GoalPopupViewModel GoalPopup { get; set; }
		public EditableGoalPopupViewModel EditableGoalPopup { get; set; }
		public EditableGoalGroupPopupViewModel EditableGoalGroupPopup { get; set; }
		public DeleteGoalGroupConfirmationPopupViewModel DeleteGoalGroupConfirmationPopup { get; set; }

		private MainViewModel MainVm { get; set; }

		private ObservableCollection<Contract> _entries = new();
		public ObservableCollection<Contract> Entries
		{
			get => _entries;
			set
			{
				if (_entries != value)
				{
					_entries = value;
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

			ContractButtonClick = new RelayCommand(OnGoalButtonClick);
			OnAddClicked = new RelayCommand(_ =>
			{
				EditableGoalPopup.SetParameters("Create Goal", false);
				MainVm.QueuePopup(EditableGoalPopup);
			});
			OnGroupAddClicked = new RelayCommand(_ =>
			{
				EditableGoalGroupPopup.SetParameters("Create Group", false);
				MainVm.QueuePopup(EditableGoalGroupPopup);
			});
			OnGroupEditClicked = new RelayCommand(o =>
			{
				EditableGoalGroupPopup.SetParameters("Edit Group", true);
				EditableGoalGroupPopup.SetData(Entries.FirstOrDefault(x => x.Uuid == (string)o));
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
			Entries.Clear();

			foreach (var contract in TrackingData.Contracts)
			{
				var ged = contract.Goals.ToList();
				Entries.Add(new Contract(contract.Uuid, contract.Name, contract.Color, contract.Paused, ged));
			}

			if (GoalPopup.IsInitialized)
			{
				var goal = (from contract in Entries from g in contract.Goals where g.Uuid == GoalPopup.Uuid select g).FirstOrDefault();
				GoalPopup.SetData(goal);
			}
			else GoalPopup.Close();
		}

		public void OnGoalButtonClick(object parameter)
		{
			var uuid = (string)parameter;

			GoalPopup.SetFlags(true, true);
			GoalPopup.SetData((from gg in Entries
							   from g in gg.Goals
							   where g.Uuid == uuid
							   select g).FirstOrDefault());
			MainVm.QueuePopup(GoalPopup);
		}
	}
}
