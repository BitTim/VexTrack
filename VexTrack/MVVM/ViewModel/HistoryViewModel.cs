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
		private string initUUID;

		public RelayCommand HistoryButtonClick { get; set; }
		public RelayCommand OnAddClicked { get; set; }
		private HistoryEntryPopupViewModel HEPopup { get; set; }
		private EditableHistoryEntryPopupViewModel EditableHEPopup { get; set; }
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
			HEPopup = (HistoryEntryPopupViewModel)ViewModelManager.ViewModels["HEPopup"];
			EditableHEPopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels["EditableHEPopup"];

			HistoryButtonClick = new RelayCommand(OnHistoryButtonClick);
			OnAddClicked = new RelayCommand(o => {
				EditableHEPopup.SetParameters("Create History Entry", false);
				MainVM.QueuePopup(EditableHEPopup);
			});

			Update();
		}

		public void Update()
		{
			Entries.Clear();

			foreach (HistoryEntry he in TrackingDataHelper.CurrentSeasonData.History)
			{
				string result = HistoryDataCalc.CalcHistoryResult(he.Description);
				Entries.Insert(0, new HistoryEntryData(TrackingDataHelper.CurrentSeasonUUID, he.UUID, he.Description, he.Time, he.Amount, he.Map, result));
			}

			initUUID = Entries.Last().HUUID;

			HistoryEntryData entry = Entries.Where(e => e.HUUID == HEPopup.HUUID).FirstOrDefault();
			if (HEPopup.IsInitialized && entry != null) HEPopup.SetData(entry, initUUID);
			else HEPopup.Close();
		}

		public void OnHistoryButtonClick(object parameter)
		{
			string hUUID = (string)parameter;

			HEPopup.SetData(Entries.Where(e => e.HUUID == hUUID).First(), initUUID);
			MainVM.QueuePopup(HEPopup);
		}
	}
}
