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
		public RelayCommand OnApplyClicked { get; set; }

		public string Title { get; set; }
		public int Index { get; set; }

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

		public EditableHistoryEntryPopupViewModel(string Title)
		{
			CanCancel = true;
			SetTile(Title);

			OnBackClicked = new RelayCommand(o => { if (CanCancel) Close(); });
			OnApplyClicked = new RelayCommand(o => {
				TrackingDataHelper.EditHistoryEntry(TrackingDataHelper.CurrentSeasonIndex, Index, new HistoryEntry(Time, Description, Amount, Map));
				Close();
			});
		}
		
		public void SetTile(string title)
		{
			Title = title;
		}

		public void SetData(HistoryEntryData data)
		{
			Index = data.Index;

			Description = data.Description;
			Time = data.Time;
			Amount = data.Amount;
			Map = data.Map;

			IsInitialized = true;
		}
    }
}