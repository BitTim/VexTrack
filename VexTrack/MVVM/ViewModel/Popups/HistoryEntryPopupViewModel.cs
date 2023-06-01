using VexTrack.Core.Model;
using VexTrack.Core.Model.WPF;

namespace VexTrack.MVVM.ViewModel.Popups;

class HistoryEntryPopupViewModel : BasePopupViewModel
{
	private string _result;
	public RelayCommand OnBackClicked { get; }
	public RelayCommand OnEditClicked { get; }
	public RelayCommand OnDeleteClicked { get; }
	private EditableHistoryEntryPopupViewModel EditableHePopup { get; }

	private HistoryEntry RawData { get; set; }
	private string SeasonUuid { get; set; }
	public string Uuid { get; private set; }
	public string Title { get; private set; }
	public long Time { get; private set; }
	public int Amount { get; private set; }
	public string Map { get; private set; }

	public string Result
	{
		get => _result;
		private set
		{
			if (value == _result) return;
			_result = value;
			OnPropertyChanged();
		}
	}

	public bool Deletable { get; set; }

	public HistoryEntryPopupViewModel()
	{
		EditableHePopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels[nameof(EditableHistoryEntryPopupViewModel)];
		CanCancel = true;

		OnBackClicked = new RelayCommand(_ => { Close(); });
		OnEditClicked = new RelayCommand(_ =>
		{
			EditableHePopup.SetParameters("Edit History Entry", true);
			EditableHePopup.SetData(RawData);
			MainVm.QueuePopup(EditableHePopup);
		});
		OnDeleteClicked = new RelayCommand(_ =>
		{
			IsInitialized = false;
			UserData.RemoveHistoryEntry(SeasonUuid, Uuid);
		});
	}

	public void SetData(HistoryEntry data, string initUuid)
	{
		RawData = data;
		Deletable = true;

		SeasonUuid = data.GroupUuid;
		Uuid = data.Uuid;
		Title = data.GetTitle();
		Time = data.Time;
		Amount = data.Amount;
		Map = data.Map;
		Result = data.GetResult();

		if (Result == "") Result = "-";
		if (string.IsNullOrEmpty(Map)) Map = "-";
		if (data.Uuid == initUuid) Deletable = false;

		IsInitialized = true;
	}

	public override void Close()
	{
		EditableHePopup.Close();
		base.Close();
	}
}