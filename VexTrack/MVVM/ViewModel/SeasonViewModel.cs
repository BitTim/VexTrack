using System.Collections.ObjectModel;
using System.Linq;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class SeasonViewModel : ObservableObject
	{
		public RelayCommand SeasonButtonClick { get; set; }
		public RelayCommand OnAddClicked { get; set; }

		public SeasonPopupViewModel SeasonPopup { get; set; }
		private SeasonEndConfirmationPopupViewModel SeasonEndPopup { get; set; }
		private MainViewModel MainVM { get; set; }
		private bool Epilogue { get; set; }

		private ObservableCollection<SeasonEntryData> _entries = new();
		public ObservableCollection<SeasonEntryData> Entries
		{
			get => _entries;
			set
			{
				if (_entries != value)
				{
					_entries = value;
					OnPropertyChanged();
				}
			}
		}

		public SeasonViewModel()
		{
			MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
			SeasonPopup = (SeasonPopupViewModel)ViewModelManager.ViewModels["SeasonPopup"];
			SeasonEndPopup = (SeasonEndConfirmationPopupViewModel)ViewModelManager.ViewModels["SeasonEndConfirmationPopup"];

			SeasonButtonClick = new RelayCommand(OnSeasonButtonClick);
			OnAddClicked = new RelayCommand(o =>
			{
				SeasonEndPopup.SetData(TrackingDataHelper.CurrentSeasonUUID);
				MainVM.QueuePopup(SeasonEndPopup);
			});

			Update(false);
		}

		public void Update(bool epilogue)
		{
			Epilogue = epilogue;
			Entries.Clear();

			foreach (Season s in TrackingDataHelper.Data.Seasons)
			{
				Entries.Add(SeasonDataCalc.CalcSeason(s, epilogue));
			}

			if (SeasonPopup.IsInitialized) SeasonPopup.SetData(Entries.Where(e => e.UUID == SeasonPopup.UUID).First(), epilogue);
			else SeasonPopup.Close();
		}

		public void OnSeasonButtonClick(object parameter)
		{
			string uuid = (string)parameter;

			SeasonPopup.SetFlags(true, true);
			SeasonPopup.SetData(Entries.Where(e => e.UUID == uuid).First(), Epilogue);
			MainVM.QueuePopup(SeasonPopup);
		}
	}
}
