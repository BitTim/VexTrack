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
		private MainViewModel MainVm { get; set; }
		private bool Epilogue { get; set; }

		private ObservableCollection<Season> _entries = new();
		public ObservableCollection<Season> Entries
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
			MainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
			SeasonPopup = (SeasonPopupViewModel)ViewModelManager.ViewModels["SeasonPopup"];
			SeasonEndPopup = (SeasonEndConfirmationPopupViewModel)ViewModelManager.ViewModels["SeasonEndConfirmationPopup"];

			SeasonButtonClick = new RelayCommand(OnSeasonButtonClick);
			OnAddClicked = new RelayCommand(o =>
			{
				SeasonEndPopup.SetData(TrackingData.CurrentSeasonData.Uuid);
				MainVm.QueuePopup(SeasonEndPopup);
			});

			Update(false);
		}

		public void Update(bool epilogue)
		{
			Epilogue = epilogue;
			Entries.Clear();

			foreach (var s in TrackingData.Seasons)
			{
				Entries.Add(s);
			}

			if (SeasonPopup.IsInitialized) SeasonPopup.SetData(Entries.First(e => e.Uuid == SeasonPopup.Uuid), epilogue);
			else SeasonPopup.Close();
		}

		private static void OnSeasonButtonClick(object parameter)
		{
			var uuid = (string)parameter;

			TrackingData.Seasons.Find(s => s.Uuid == uuid)?.UpdateGraph();
		}
	}
}
