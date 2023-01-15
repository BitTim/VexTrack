using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class HistoryEntryPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnEditClicked { get; set; }
		public RelayCommand OnDeleteClicked { get; set; }
		private EditableHistoryEntryPopupViewModel EditableHePopup { get; set; }

		private HistoryEntry RawData { get; set; }
		public string SeasonUuid { get; set; }
		public string Uuid { get; set; }
		public string Title { get; set; }
		public long Time { get; set; }
		public int Amount { get; set; }
		public string Map { get; set; }
		public string Result { get; set; }
		public bool Deletable { get; set; }

		public HistoryEntryPopupViewModel()
		{
			EditableHePopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels["EditableHEPopup"];
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { Close(); });
			OnEditClicked = new RelayCommand(o =>
			{
				EditableHePopup.SetParameters("Edit History Entry", true);
				EditableHePopup.SetData(RawData);
				MainVm.QueuePopup(EditableHePopup);
			});
			OnDeleteClicked = new RelayCommand(o =>
			{
				IsInitialized = false;
				TrackingData.RemoveHistoryEntry(SeasonUuid, Uuid);
			});
		}

		public void SetData(HistoryEntry data, string initUuid)
		{
			RawData = data;
			Deletable = true;

			SeasonUuid = data.SeasonUuid;
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
}
