using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class MainViewModel : ObservableObject
	{
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

		private object _currentView;

		private BasePopupViewModel _currentPopup = null;
		private List<BasePopupViewModel> _popupQueue = new();

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

		public MainViewModel()
		{
			TrackingDataHelper.LoadData();

			ViewModelManager.ViewModels.Add("Main", this);

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

			DashboardViewCommand = new RelayCommand(o => { CurrentView = DashboardVM; });
			GoalViewCommand = new RelayCommand(o => { CurrentView = GoalVM; });
			SeasonViewCommand = new RelayCommand(o => { CurrentView = SeasonVM; });
			HistoryViewCommand = new RelayCommand(o => { CurrentView = HistoryVM; });
			SettingsViewCommand = new RelayCommand(o => { CurrentView = SettingsVM; });
		}

		public void Update()
		{
			HistoryVM.Update();
			GoalVM.Update();
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
	}
}
