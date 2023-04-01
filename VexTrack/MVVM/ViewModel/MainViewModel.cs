using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	internal class MainViewModel : ObservableObject
	{
		private ThemeWatcher Watcher { get; set; }

		public RelayCommand DashboardViewCommand { get; set; }
		public RelayCommand GoalViewCommand { get; set; }
		public RelayCommand SeasonViewCommand { get; set; }
		public RelayCommand HistoryViewCommand { get; set; }
		public RelayCommand SettingsViewCommand { get; set; }

		private HomeViewModel HomeVm { get; set; }
		private ContractViewModel ContractVm { get; set; }
		private SeasonViewModel SeasonVm { get; set; }
		private HistoryViewModel HistoryVm { get; set; }
		private SettingsViewModel SettingsVm { get; set; }

		private HistoryEntryPopupViewModel HePopup { get; set; }
		private EditableHistoryEntryPopupViewModel EditableHePopup { get; set; }
		private GoalPopupViewModel GoalPopup { get; set; }
		private EditableGoalPopupViewModel EditableGoalPopup { get; set; }
		private SeasonPopupViewModel SeasonPopup { get; set; }
		private EditableSeasonPopupViewModel EditableSeasonPopup { get; set; }
		private DataInitPopupViewModel DataInitPopup { get; set; }
		private ResetDataConfirmationPopupViewModel ResetDataConfirmationPopup { get; set; }
		private ProgressActivityPopupViewModel PaPopupVm { get; set; }
		private AboutPopupViewModel AboutPopup { get; set; }
		private UpdateAvailablePopupViewModel UpdateAvailablePopup { get; set; }
		private UpdateDownloadPopupViewModel UpdateDownloadPopup { get; set; }
		private UpdateFailedPopupViewModel UpdateFailedPopup { get; set; }
		private EditableGoalGroupPopupViewModel EditableGoalGroupPopup { get; set; }
		private DeleteGoalConfirmationPopupViewModel DeleteGoalConfirmationPopup { get; set; }
		private DeleteGoalGroupConfirmationPopupViewModel DeleteGoalGroupConfirmationPopup { get; set; }
		private SeasonEndConfirmationPopupViewModel SeasonEndConfirmationPopup { get; set; }

		private object _currentView;
		private bool _epilogue;
		private bool _epilogueButtonEnabled;

		private BasePopupViewModel _currentPopup;
		private List<BasePopupViewModel> _popupQueue = new();

		private bool _viewModelsInitialized;
		public bool InterruptUpdate = false;

		public object CurrentView
		{
			get => _currentView;
			private set
			{
				_currentView = value;
				OnPropertyChanged();
			}
		}

		public BasePopupViewModel CurrentPopup
		{
			get => _currentPopup;
			set
			{
				_currentPopup = value;
				OnPropertyChanged();
			}
		}

		public List<BasePopupViewModel> PopupQueue
		{
			get => _popupQueue;
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
			if (Directory.Exists(Constants.LegacyDataFolder))
			{
				var targetDir = new DirectoryInfo(Constants.DataFolder);
				if (!targetDir.Exists) Directory.CreateDirectory(Constants.DataFolder);

				foreach (var f in Directory.GetFiles(Constants.LegacyDataFolder))
				{
					var file = new FileInfo(f);
					file.MoveTo(targetDir + "\\" + file.Name);
				}

				Directory.Delete(Constants.LegacyDataFolder);
			}

			SettingsHelper.Init();
			Watcher = new ThemeWatcher();

			ViewModelManager.ViewModels.Add("Main", this);
			InitPopupViewModels();

			DashboardViewCommand = new RelayCommand(_ => SetView(HomeVm));
			GoalViewCommand = new RelayCommand(_ => SetView(ContractVm));
			SeasonViewCommand = new RelayCommand(_ => SetView(SeasonVm));
			HistoryViewCommand = new RelayCommand(_ => SetView(HistoryVm));
			SettingsViewCommand = new RelayCommand(_ => SetView(SettingsVm));

			TrackingData.LoadData();
			SettingsHelper.LoadSettings();

			UpdateHelper.CheckUpdateAsync();

			Timer updateTimer = new(UpdateTimerCallback);
			var now = DateTime.Now.ToLocalTime();
			var midnight = DateTime.Today.ToLocalTime();

			if (now > midnight) midnight = midnight.AddDays(1).ToLocalTime();
			var msUntilMidnight = (int)(midnight - now).TotalMilliseconds;
			updateTimer.Change(msUntilMidnight, Timeout.Infinite);

			Update();
		}

		private void InitViewModels()
		{
			if (InterruptUpdate) return;

			HomeVm = new HomeViewModel();
			ContractVm = new ContractViewModel();
			SeasonVm = new SeasonViewModel();
			HistoryVm = new HistoryViewModel();
			SettingsVm = new SettingsViewModel();

			ViewModelManager.ViewModels.Add("Dashboard", HomeVm);
			ViewModelManager.ViewModels.Add("Goal", ContractVm);
			ViewModelManager.ViewModels.Add("Season", SeasonVm);
			ViewModelManager.ViewModels.Add("History", HistoryVm);
			ViewModelManager.ViewModels.Add("Settings", SettingsVm);

			CurrentView = HomeVm;
			_viewModelsInitialized = true;
		}

		private void InitPopupViewModels()
		{
			DataInitPopup = new DataInitPopupViewModel();
			ViewModelManager.ViewModels.Add("DataInitPopup", DataInitPopup);

			EditableHePopup = new EditableHistoryEntryPopupViewModel();
			ViewModelManager.ViewModels.Add("EditableHEPopup", EditableHePopup);

			HePopup = new HistoryEntryPopupViewModel();
			ViewModelManager.ViewModels.Add("HEPopup", HePopup);

			DeleteGoalConfirmationPopup = new DeleteGoalConfirmationPopupViewModel();
			ViewModelManager.ViewModels.Add("DeleteGoalConfirmationPopup", DeleteGoalConfirmationPopup);

			DeleteGoalGroupConfirmationPopup = new DeleteGoalGroupConfirmationPopupViewModel();
			ViewModelManager.ViewModels.Add("DeleteGoalGroupConfirmationPopup", DeleteGoalGroupConfirmationPopup);

			EditableGoalPopup = new EditableGoalPopupViewModel();
			ViewModelManager.ViewModels.Add("EditableGoalPopup", EditableGoalPopup);

			GoalPopup = new GoalPopupViewModel();
			ViewModelManager.ViewModels.Add("GoalPopup", GoalPopup);

			EditableSeasonPopup = new EditableSeasonPopupViewModel();
			ViewModelManager.ViewModels.Add("EditableSeasonPopup", EditableSeasonPopup);

			SeasonPopup = new SeasonPopupViewModel();
			ViewModelManager.ViewModels.Add("SeasonPopup", SeasonPopup);

			ResetDataConfirmationPopup = new ResetDataConfirmationPopupViewModel();
			ViewModelManager.ViewModels.Add("ResetDataConfirmationPopup", ResetDataConfirmationPopup);

			PaPopupVm = new ProgressActivityPopupViewModel();
			ViewModelManager.ViewModels.Add("PAPopup", PaPopupVm);

			AboutPopup = new AboutPopupViewModel();
			ViewModelManager.ViewModels.Add("AboutPopup", AboutPopup);

			UpdateAvailablePopup = new UpdateAvailablePopupViewModel();
			ViewModelManager.ViewModels.Add("UpdateAvailablePopup", UpdateAvailablePopup);

			UpdateDownloadPopup = new UpdateDownloadPopupViewModel();
			ViewModelManager.ViewModels.Add("UpdateDownloadPopup", UpdateDownloadPopup);

			UpdateFailedPopup = new UpdateFailedPopupViewModel();
			ViewModelManager.ViewModels.Add("UpdateFailedPopup", UpdateFailedPopup);

			EditableGoalGroupPopup = new EditableGoalGroupPopupViewModel();
			ViewModelManager.ViewModels.Add("EditableGoalGroupPopup", EditableGoalGroupPopup);

			SeasonEndConfirmationPopup = new SeasonEndConfirmationPopupViewModel();
			ViewModelManager.ViewModels.Add("SeasonEndConfirmationPopup", SeasonEndConfirmationPopup);
		}

		private void SetView(object view)
		{
			CurrentView = view;
		}

		public void Update(bool epilogueOnly = false)
		{
			if (InterruptUpdate) return;
			if (!_viewModelsInitialized) InitViewModels();

			if (InterruptUpdate) return;

			if (TrackingData.CurrentSeasonData.ActiveBpLevel > Constants.BattlepassLevels && SettingsHelper.Data.ForceEpilogue)
			{
				if (!Epilogue) Epilogue = true;
				EpilogueButtonEnabled = false;
			}
			else EpilogueButtonEnabled = true;

			HomeVm.Update(Epilogue);
			ContractVm.Update(Epilogue);
			SeasonVm.Update(Epilogue);

			if (epilogueOnly) return;
			HistoryVm.Update();
			SettingsVm.Update();
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

			CurrentPopup = PopupQueue.Count == 0 ? null : PopupQueue.Last();

			popup.IsOpen = false;
		}

		public void OnPopupBorderClick()
		{
			if (PopupQueue.Count != 0 && PopupQueue.Last().CanCancel == false) return;
			DequeuePopup(PopupQueue.Last());
		}


		private void UpdateTimerCallback(object state)
		{
			Application.Current.Dispatcher.Invoke(delegate { Update(); });
		}



		public void Destroy()
		{
			Watcher.Destroy();
		}
	}
}
