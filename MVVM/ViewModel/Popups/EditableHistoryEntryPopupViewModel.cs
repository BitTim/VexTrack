using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
    class EditableHistoryEntryPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnDoneClicked { get; set; }

		public string Title { get; set; }
		public int Index { get; set; }
		public string UUID { get; set; }
		public bool EditMode { get; set; }

		private string _description;
		private long _time;
		private int _amount;
		private string _map;

		public string Description { get => _description;
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
			OnDoneClicked = new RelayCommand(o => {
				if(EditMode) TrackingDataHelper.EditHistoryEntry(TrackingDataHelper.CurrentSeasonIndex, Index, new HistoryEntry(UUID, Time, Description, Amount, Map));
				else TrackingDataHelper.AddHistoryEntry(TrackingDataHelper.CurrentSeasonIndex, new HistoryEntry(UUID, Time, Description, Amount, Map));
				Close();
			});
		}
		
		public void SetParameters(string title, bool editMode)
		{
			Title = title;
			EditMode = editMode;

			if (!EditMode) InitData();
		}

		public void InitData()
		{
			Time = DateTimeOffset.Now.ToUnixTimeSeconds();

			Index = -1;
			UUID = Guid.NewGuid().ToString();
			Description = "";
			Amount = 0;
			Map = "";

			IsInitialized = true;
		}

		public void SetData(HistoryEntryData data)
		{
			Index = data.Index;

			UUID = data.UUID;
			Description = data.Description;
			Time = data.Time;
			Amount = data.Amount;
			Map = data.Map;

			IsInitialized = true;
		}
    }
}