using System.Collections.ObjectModel;
using VexTrack.Core.Model;
using VexTrack.Core.Model.WPF;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel;

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
			
		foreach (var s in UserData.Seasons)
		{
			SeasonEntries.Add(s);
		}

		foreach (var contract in UserData.Contracts)
		{
			// Dont know why this was here
			//var ged = contract.Goals.ToList();
			ContractEntries.Add(contract);
		}
	}
}