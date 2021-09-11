using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class MainViewModel : ObservableObject
	{
		public ThemeWatcher Watcher { get; set; }

		public RelayCommand DashboardViewCommand { get; set; }
		public RelayCommand GoalViewCommand { get; set; }
		public RelayCommand SeasonViewCommand { get; set; }
		public RelayCommand HistoryViewCommand { get; set; }
		public RelayCommand SettingsViewCommand { get; set; }

		public DashboardViewModel DashboardVM { get; set; }
		public GoalViewModel GoalVM { get; set; }
		public SeasonViewModel SeasonVM { get; set; }
		public HistoryViewModel HistoryVM { get; set; }
		public SettingsViewModel SettingsVM { get; set; }

		private HistoryEntryPopupViewModel HEPopup { get; set; }
		private EditableHistoryEntryPopupViewModel EditableHEPopup { get; set; }
		private GoalPopupViewModel GoalPopup { get; set; }
		private EditableGoalPopupViewModel EditableGoalPopup { get; set; }
		private SeasonPopupViewModel SeasonPopup { get; set; }
		private EditableSeasonPopupViewModel EditableSeasonPopup { get; set; }
		private DataInitPopupViewModel DataInitPopup { get; set; }
		private ResetDataConfirmationPopupViewModel ResetDataConfirmationPopup { get; set; }
		private ProgressActivityPopupViewModel PAPopupVM { get; set; }
		private AboutPopupViewModel AboutPopup { get; set; }
		private UpdateAvailablePopupViewModel UpdateAvailablePopup { get; set; }
		private UpdateDownloadPopupViewModel UpdateDownloadPopup { get; set; }
		private EditableGoalGroupPopupViewModel EditableGoalGroupPopup { get; set; }

		private object _currentView;
		private bool _epilogue;
		private bool _epilogueButtonEnabled;

		private Timer updateTimer;
		private BasePopupViewModel _currentPopup = null;
		private List<BasePopupViewModel> _popupQueue = new();

		public bool ViewModelsInitialized = false;
		public bool InterruptUpdate = false;

		public object CurrentView
		{
			get { return _currentView; }
			set {
				_currentView = value;
				OnPropertyChanged();
			}
		}

		public BasePopupViewModel CurrentPopup
		{
			get { return _currentPopup; }
			set
			{
				_currentPopup = value;
				OnPropertyChanged();
			}
		}

		public List<BasePopupViewModel> PopupQueue
		{
			get { return _popupQueue; }
			set
			{
				_popupQueue = value;
				OnPropertyChanged();
			}
		}

		public bool Epilogue
		{
			get => _epilogue;
			set
			{
				_epilogue = value;
				OnPropertyChanged();
				Update(true);
			}
		}

		public bool EpilogueButtonEnabled
		{
			get => _epilogueButtonEnabled;
			set
			{
				_epilogueButtonEnabled = value;
				OnPropertyChanged();
			}
		}

		public MainViewModel()
		{
			if(Directory.Exists(Constants.LegacyDataFolder))
			{
				DirectoryInfo targetDir = new DirectoryInfo(Constants.DataFolder);
				if(!targetDir.Exists) Directory.CreateDirectory(Constants.DataFolder);

				foreach(string f in Directory.GetFiles(Constants.LegacyDataFolder))
				{
					FileInfo file = new FileInfo(f);
					file.MoveTo(targetDir + "\\" + file.Name);
				}

				Directory.Delete(Constants.LegacyDataFolder);
			}

			SettingsHelper.Init();
			Watcher = new();

			ViewModelManager.ViewModels.Add("Main", this);
			InitPopupViewModels();

			DashboardViewCommand = new RelayCommand(o => SetView(DashboardVM)); ;
			GoalViewCommand = new RelayCommand(o => SetView(GoalVM));
			SeasonViewCommand = new RelayCommand(o => SetView(SeasonVM));
			HistoryViewCommand = new RelayCommand(o => SetView(HistoryVM));
			SettingsViewCommand = new RelayCommand(o => SetView(SettingsVM));

			TrackingDataHelper.LoadData();
			SettingsHelper.LoadSettings();

			UpdateHelper.CheckUpdateAsync();

			updateTimer = new(UpdateTimerCallback);
			DateTime now = DateTime.Now.ToLocalTime();
			DateTime midnight = DateTime.Today.ToLocalTime();

			if(now > midnight) midnight = midnight.AddDays(1).ToLocalTime();
			int msUntilMidnight = (int)(midnight - now).TotalMilliseconds;
			updateTimer.Change(msUntilMidnight, Timeout.Infinite);

			Update();
		}

		private void InitViewModels()
		{
			if (InterruptUpdate) return;

			DashboardVM = new DashboardViewModel();
			GoalVM = new GoalViewModel();
			SeasonVM = new SeasonViewModel();
			HistoryVM = new HistoryViewModel();
			SettingsVM = new SettingsViewModel();

			ViewModelManager.ViewModels.Add("Dashboard", DashboardVM);
			ViewModelManager.ViewModels.Add("Goal", GoalVM);
			ViewModelManager.ViewModels.Add("Season", SeasonVM);
			ViewModelManager.ViewModels.Add("History", HistoryVM);
			ViewModelManager.ViewModels.Add("Settings", SettingsVM);

			CurrentView = DashboardVM;
			ViewModelsInitialized = true;
		}

		private void InitPopupViewModels()
		{
			DataInitPopup = new();
			ViewModelManager.ViewModels.Add("DataInitPopup", DataInitPopup);

			EditableHEPopup = new();
			ViewModelManager.ViewModels.Add("EditableHEPopup", EditableHEPopup);

			HEPopup = new();
			ViewModelManager.ViewModels.Add("HEPopup", HEPopup);

			EditableGoalPopup = new();
			ViewModelManager.ViewModels.Add("EditableGoalPopup", EditableGoalPopup);

			GoalPopup = new();
			ViewModelManager.ViewModels.Add("GoalPopup", GoalPopup);

			EditableSeasonPopup = new();
			ViewModelManager.ViewModels.Add("EditableSeasonPopup", EditableSeasonPopup);

			SeasonPopup = new();
			ViewModelManager.ViewModels.Add("SeasonPopup", SeasonPopup);

			ResetDataConfirmationPopup = new ResetDataConfirmationPopupViewModel();
			ViewModelManager.ViewModels.Add("ResetDataConfirmationPopup", ResetDataConfirmationPopup);

			PAPopupVM = new ProgressActivityPopupViewModel();
			ViewModelManager.ViewModels.Add("PAPopup", PAPopupVM);

			AboutPopup = new AboutPopupViewModel();
			ViewModelManager.ViewModels.Add("AboutPopup", AboutPopup);

			UpdateAvailablePopup = new UpdateAvailablePopupViewModel();
			ViewModelManager.ViewModels.Add("UpdateAvailablePopup", UpdateAvailablePopup);

			UpdateDownloadPopup = new UpdateDownloadPopupViewModel();
			ViewModelManager.ViewModels.Add("UpdateDownloadPopup", UpdateDownloadPopup);

			EditableGoalGroupPopup = new EditableGoalGroupPopupViewModel();
			ViewModelManager.ViewModels.Add("EditableGoalGroupPopup", EditableGoalGroupPopup);
		}

		public void SetView(object view)
		{
			CurrentView = view;
		}

		public void Update(bool epilogueOnly = false)
		{
			if (InterruptUpdate) return;
			if (!ViewModelsInitialized) InitViewModels();

			if(TrackingDataHelper.CurrentSeasonData.ActiveBPLevel > Constants.BattlepassLevels && SettingsHelper.Data.ForceEpilogue)
			{
				if(!Epilogue) Epilogue = true;
				EpilogueButtonEnabled = false;
			}
			else EpilogueButtonEnabled = true;

			DashboardVM.Update(Epilogue);
			GoalVM.Update(Epilogue);
			SeasonVM.Update(Epilogue);

			if (!epilogueOnly)
			{
				HistoryVM.Update();
				SettingsVM.Update();
			}
		}

		public void QueuePopup(BasePopupViewModel popup)
		{
			PopupQueue.Add(popup);
			CurrentPopup = PopupQueue.Last();
			popup.IsOpen = true;
		}

		public void DequeuePopup(BasePopupViewModel popup)
		{
			PopupQueue.Remove(popup);

			if (PopupQueue.Count == 0) CurrentPopup = null;
			else CurrentPopup = PopupQueue.Last();

			popup.IsOpen = false;
		}

		public void OnPopupBorderClick()
		{
			if (PopupQueue.Count != 0 && PopupQueue.Last().CanCancel == false) return;
			DequeuePopup(PopupQueue.Last());
		}



		public void UpdateTimerCallback(object state)
		{
			Application.Current.Dispatcher.Invoke((Action)delegate { Update(); });
		}



		public void Destroy()
		{
			Watcher.Destroy();
		}
	}
}
