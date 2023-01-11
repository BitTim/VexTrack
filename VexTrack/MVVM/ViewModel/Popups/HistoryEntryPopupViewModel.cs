using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class HistoryEntryPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnEditClicked { get; set; }
		public RelayCommand OnDeleteClicked { get; set; }
		private EditableHistoryEntryPopupViewModel EditableHePopup { get; set; }

		private HistoryEntryData RawData { get; set; }
		public string Suuid { get; set; }
		public string Huuid { get; set; }
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
				TrackingDataHelper.RemoveHistoryEntry(Suuid, Huuid);
			});
		}

		public void SetData(HistoryEntryData data, string initUuid)
		{
			RawData = data;
			Deletable = true;

			Suuid = data.Suuid;
			Huuid = data.Huuid;
			Title = data.Title;
			Time = data.Time;
			Amount = data.Amount;
			Map = data.Map;
			Result = data.Result;

			if (Result == "") Result = "-";
			if (Map == "" || Map == null) Map = "-";
			if (data.Huuid == initUuid) Deletable = false;

			IsInitialized = true;
		}

		public override void Close()
		{
			EditableHePopup.Close();
			base.Close();
		}
	}
}
