using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using VexTrack.Core;
using VexTrack.Core.Helper;
using VexTrack.Core.IO;
using VexTrack.Core.Model;
using VexTrack.Core.Model.WPF;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class MainViewModel : ObservableObject
	{
		private ThemeWatcher Watcher { get; set; }
		
		public RelayCommand DashboardViewCommand { get; }
		public RelayCommand GoalViewCommand { get; }
		public RelayCommand HistoryViewCommand { get; }
		public RelayCommand SettingsViewCommand { get; }

		private HomeViewModel HomeVm { get; set; }
		private ContractViewModel ContractVm { get; set; }
		private HistoryViewModel HistoryVm { get; set; }
		private SettingsViewModel SettingsVm { get; set; }

		private HistoryEntryPopupViewModel HePopup { get; set; }
		private EditableHistoryEntryPopupViewModel EditableHePopup { get; set; }
		private EditableGoalPopupViewModel EditableGoalPopup { get; set; }
		private DataInitPopupViewModel DataInitPopup { get; set; }
		private ResetDataConfirmationPopupViewModel ResetDataConfirmationPopup { get; set; }
		private ProgressActivityPopupViewModel ProgressActivityPopup { get; set; }
		private AboutPopupViewModel AboutPopup { get; set; }
		private UpdateAvailablePopupViewModel UpdateAvailablePopup { get; set; }
		private UpdateDownloadPopupViewModel UpdateDownloadPopup { get; set; }
		private UpdateFailedPopupViewModel UpdateFailedPopup { get; set; }
		private DeleteGoalConfirmationPopupViewModel DeleteGoalConfirmationPopup { get; set; }

		private object _currentView;
		private bool _epilogue;

		private BasePopupViewModel _currentPopup;

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

		public List<BasePopupViewModel> PopupQueue { get; } = new();

		private bool Epilogue
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
			set
			{
				_epilogue = value;
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

			ViewModelManager.ViewModels.Add(nameof(MainViewModel), this);
			InitPopupViewModels();

			DashboardViewCommand = new RelayCommand(_ => SetView(HomeVm));
			GoalViewCommand = new RelayCommand(_ => SetView(ContractVm));
			HistoryViewCommand = new RelayCommand(_ => SetView(HistoryVm));
			SettingsViewCommand = new RelayCommand(_ => SetView(SettingsVm));

			Loader.LoadUserData();
			SettingsHelper.LoadSettings();

			UpdateHelper.CheckUpdateAsync();

			Timer updateTimer = new(UpdateTimerCallback);
			var now = TimeHelper.NowTime;
			var midnight = TimeHelper.TodayDate;

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
			HistoryVm = new HistoryViewModel();
			SettingsVm = new SettingsViewModel();

			ViewModelManager.ViewModels.Add(nameof(HomeViewModel), HomeVm);
			ViewModelManager.ViewModels.Add(nameof(ContractViewModel), ContractVm);
			ViewModelManager.ViewModels.Add(nameof(HistoryViewModel), HistoryVm);
			ViewModelManager.ViewModels.Add(nameof(SettingsViewModel), SettingsVm);

			CurrentView = HomeVm;
			_viewModelsInitialized = true;
		}

		private void InitPopupViewModels()
		{
			DataInitPopup = new DataInitPopupViewModel();
			ViewModelManager.ViewModels.Add(nameof(DataInitPopupViewModel), DataInitPopup);
			
			EditableHePopup = new EditableHistoryEntryPopupViewModel();
			ViewModelManager.ViewModels.Add(nameof(EditableHistoryEntryPopupViewModel), EditableHePopup);
			
			HePopup = new HistoryEntryPopupViewModel();
			ViewModelManager.ViewModels.Add(nameof(HistoryEntryPopupViewModel), HePopup);
			
			DeleteGoalConfirmationPopup = new DeleteGoalConfirmationPopupViewModel();
			ViewModelManager.ViewModels.Add(nameof(DeleteGoalConfirmationPopupViewModel), DeleteGoalConfirmationPopup);
			
			EditableGoalPopup = new EditableGoalPopupViewModel();
			ViewModelManager.ViewModels.Add(nameof(EditableGoalPopupViewModel), EditableGoalPopup);
			
			ResetDataConfirmationPopup = new ResetDataConfirmationPopupViewModel();
			ViewModelManager.ViewModels.Add(nameof(ResetDataConfirmationPopupViewModel), ResetDataConfirmationPopup);
			
			ProgressActivityPopup = new ProgressActivityPopupViewModel();
			ViewModelManager.ViewModels.Add(nameof(ProgressActivityPopupViewModel), ProgressActivityPopup);
			
			AboutPopup = new AboutPopupViewModel();
			ViewModelManager.ViewModels.Add(nameof(AboutPopupViewModel), AboutPopup);
			
			UpdateAvailablePopup = new UpdateAvailablePopupViewModel();
			ViewModelManager.ViewModels.Add(nameof(UpdateAvailablePopupViewModel), UpdateAvailablePopup);
			
			UpdateDownloadPopup = new UpdateDownloadPopupViewModel();
			ViewModelManager.ViewModels.Add(nameof(UpdateDownloadPopupViewModel), UpdateDownloadPopup);
			
			UpdateFailedPopup = new UpdateFailedPopupViewModel();
			ViewModelManager.ViewModels.Add(nameof(UpdateFailedPopupViewModel), UpdateFailedPopup);
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

			if (Tracking.CurrentSeasonData.ActiveBpLevel > Constants.BattlepassLevels && SettingsHelper.Data.ForceEpilogue)
			{
				if (!Epilogue) Epilogue = true;
				EpilogueButtonEnabled = false;
			}
			else EpilogueButtonEnabled = true;

			HomeVm.Update();
			ContractVm.Update();

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
