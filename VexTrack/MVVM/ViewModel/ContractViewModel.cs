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
		public RelayCommand OnHistoryAddClicked { get; set; }
		public RelayCommand OnContractAddClicked { get; set; }

		private EditableHistoryEntryPopupViewModel EditableHePopup { get; set; }

		private MainViewModel MainVm { get; set; }

		private ObservableCollection<Season> _seasonEntries = new();
		private ObservableCollection<Contract> _contractEntries = new();
		public ObservableCollection<Season> SeasonEntries
		{
			get => _seasonEntries;
			set
			{
				if (_seasonEntries != value)
				{
					_seasonEntries = value;
					OnPropertyChanged();
				}
			}
		}
		public ObservableCollection<Contract> ContractEntries
		{
			get => _contractEntries;
			set
			{
				if (_contractEntries != value)
				{
					_contractEntries = value;
					OnPropertyChanged();
				}
			}
		}

		public ContractViewModel()
		{
			MainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
			EditableHePopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels["EditableHEPopup"];
			
			OnHistoryAddClicked = new RelayCommand(o =>
			{
				EditableHePopup.SetParameters("Create History Entry", false);
				MainVm.QueuePopup(EditableHePopup);
			});
			OnContractAddClicked = new RelayCommand(_ =>
			{
				
			});
			
			Update(false);
		}

		public void Update(bool epilogue)
		{
			SeasonEntries.Clear();
			ContractEntries.Clear();
			
			foreach (var s in TrackingData.Seasons)
			{
				SeasonEntries.Add(s);
			}

			foreach (var contract in TrackingData.Contracts)
			{
				var ged = contract.Goals.ToList();
				ContractEntries.Add(new Contract(contract.Uuid, contract.Name, contract.Color, contract.Paused, ged));
			}
		}
	}
}
