using System;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class EditableHistoryEntryPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnDoneClicked { get; set; }

		public string Title { get; set; }
		public string SUUID { get; set; }
		public string HUUID { get; set; }
		public bool EditMode { get; set; }

		private string _description;
		private long _time;
		private int _amount;
		private string _map;

		public string Description
		{
			get => _description;
			set
			{
				_description = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Result));
			}
		}
		public long Time
		{
			get => _time;
			set
			{
				_time = value;
				OnPropertyChanged();
			}
		}
		public int Amount
		{
			get => _amount;
			set
			{
				_amount = value;
				OnPropertyChanged();
			}
		}
		public string Map
		{
			get => _map;
			set
			{
				_map = value;
				OnPropertyChanged();
			}
		}

		public string Result => HistoryDataCalc.CalcHistoryResult(Description);

		public EditableHistoryEntryPopupViewModel()
		{
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { if (CanCancel) Close(); });
			OnDoneClicked = new RelayCommand(o =>
			{
				if (EditMode) TrackingDataHelper.EditHistoryEntry(SUUID, HUUID, new HistoryEntry(HUUID, Time, Description, Amount, Map));
				else TrackingDataHelper.AddHistoryEntry(SUUID, new HistoryEntry(HUUID, Time, Description, Amount, Map));
				Close();
			});
		}

		public void SetParameters(string title, bool editMode)
		{
			Title = title;
			EditMode = editMode;

			if (!EditMode)
				InitData();
		}

		public void InitData()
		{
			Time = DateTimeOffset.Now.ToUnixTimeSeconds();

			SUUID = TrackingDataHelper.CurrentSeasonUUID;
			HUUID = Guid.NewGuid().ToString();
			Description = "";
			Amount = 0;
			Map = "";

			IsInitialized = true;
		}

		public void SetData(HistoryEntryData data)
		{
			SUUID = data.SUUID;
			HUUID = data.HUUID;
			Description = data.Description;
			Time = data.Time;
			Amount = data.Amount;
			Map = data.Map;

			IsInitialized = true;
		}
	}
}