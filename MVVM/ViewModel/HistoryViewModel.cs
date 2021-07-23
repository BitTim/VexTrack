using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VexTrack.Core;
using VexTrack.MVVM.Model;
using VexTrack.MVVM.View;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class HistoryViewModel : ObservableObject
	{
		public RelayCommand HistoryButtonClick { get; set; }
		private HistoryEntryPopupViewModel HEPopup { get; set; }
		private MainViewModel MainVM { get; set; }

		private ObservableCollection<HistoryEntryData> _entries = new();
		public ObservableCollection<HistoryEntryData> Entries { get => _entries;
			set
			{
				if (_entries != value)
				{
					_entries = value;
					OnPropertyChanged();
				}
			}
		}

		public HistoryViewModel()
		{
			MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];

			HEPopup = new();
			ViewModelManager.ViewModels.Add("HEPopup", HEPopup);

			HistoryButtonClick = new RelayCommand(OnHistoryButtonClick);

			Update();
		}

		public void Update()
		{
			Entries.Clear();

			foreach (HistoryEntry he in TrackingDataHelper.Data.Seasons.Last<Season>().History)
			{
				string result = HistoryDataCalc.CalcHistoryResult(he.Description);
				Entries.Insert(0, new HistoryEntryData(Entries.Count, TrackingDataHelper.CurrentSeasonIndex, he.Description, he.Time, he.Amount, he.Map, result));
			}

			if (HEPopup.IsInitialized) HEPopup.SetData(Entries[Entries.Count - HEPopup.Index - 1]);
			else HEPopup.Close();
		}

		public void OnHistoryButtonClick(object parameter)
		{
			int index = Entries.Count - (int)parameter - 1;

			HEPopup.SetData(Entries[index]);
			MainVM.QueuePopup(HEPopup);
		}
	}
}
