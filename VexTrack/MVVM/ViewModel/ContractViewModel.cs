using System.Collections.ObjectModel;
using System.Linq;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class ContractViewModel : ObservableObject
	{
		public RelayCommand OnHistoryAddClicked { get; }
		public RelayCommand OnContractAddClicked { get; }

		private EditableHistoryEntryPopupViewModel EditableHePopup { get; }

		private MainViewModel MainVm { get; }

		public ObservableCollection<Season> SeasonEntries { get; } = new();
		public ObservableCollection<Contract> ContractEntries { get; } = new();

		public ContractViewModel()
		{
			MainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
			EditableHePopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels[nameof(EditableHistoryEntryPopupViewModel)];
			
			OnHistoryAddClicked = new RelayCommand(_ =>
			{
				EditableHePopup.SetParameters("Create History Entry", false);
				MainVm.QueuePopup(EditableHePopup);
			});
			OnContractAddClicked = new RelayCommand(_ =>
			{
				
			});
			
			Update();
		}

		public void Update()
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
