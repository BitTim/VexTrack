using System.Collections.ObjectModel;
using System.Linq;
using VexTrack.Core.Helper;
using VexTrack.Core.Model;
using VexTrack.Core.Model.WPF;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel;

class HistoryViewModel : ObservableObject
{
	private string _initUuid;

	public RelayCommand HistoryButtonClick { get; }
	public RelayCommand OnAddClicked { get; }
	private HistoryEntryPopupViewModel HePopup { get; }
	private EditableHistoryEntryPopupViewModel EditableHePopup { get; }
	private MainViewModel MainVm { get; }
	public ObservableCollection<HistoryGroup> Groups { get; } = new();

	public HistoryViewModel()
	{
		MainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
		HePopup = (HistoryEntryPopupViewModel)ViewModelManager.ViewModels[nameof(HistoryEntryPopupViewModel)];
		EditableHePopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels[nameof(EditableHistoryEntryPopupViewModel)];

		HistoryButtonClick = new RelayCommand(OnHistoryButtonClick);
		OnAddClicked = new RelayCommand(_ =>
		{
			EditableHePopup.SetParameters("Create History Entry", false);
			MainVm.QueuePopup(EditableHePopup);
		});

		Update();
	}

	public void Update()
	{
		Groups.Clear();

		var groups = SettingsHelper.Data.SingleSeasonHistory ? UserData.History.Where(hg => hg.SeasonUuid == UserData.CurrentSeasonData.Uuid).ToList() : UserData.History;
		foreach (var hg in groups) { Groups.Add(hg); }

		if (Groups.Count < 1) return;
		
		_initUuid = Groups.Last().Entries.Last().Uuid;

		var entry = (from g in Groups
			from e in g.Entries
			where e.Uuid == HePopup.Uuid
			select e).FirstOrDefault();

		if (HePopup.IsInitialized && entry != null) HePopup.SetData(entry);
		else HePopup.Close();
	}

	private void OnHistoryButtonClick(object parameter)
	{
		var hUuid = (string)parameter;

		HePopup.SetData((from g in Groups
			from e in g.Entries
			where e.Uuid == hUuid
			select e).FirstOrDefault());

		MainVm.QueuePopup(HePopup);
	}
}