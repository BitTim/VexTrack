using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

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

		public object CurrentView
		{
			get { return _currentView; }
			set {
				_currentView = value;
				OnPropertyChanged();
			}
		}

		public MainViewModel()
		{
			DashboardVM = new DashboardViewModel();
			GoalVM = new GoalViewModel();
			SeasonVM = new SeasonViewModel();
			HistoryVM = new HistoryViewModel();
			SettingsVM = new SettingsViewModel();

			CurrentView = DashboardVM;

			DashboardViewCommand = new RelayCommand(o => { CurrentView = DashboardVM; });
			GoalViewCommand = new RelayCommand(o => { CurrentView = GoalVM; });
			SeasonViewCommand = new RelayCommand(o => { CurrentView = SeasonVM; });
			HistoryViewCommand = new RelayCommand(o => { CurrentView = HistoryVM; });
			SettingsViewCommand = new RelayCommand(o => { CurrentView = SettingsVM; });
		}
	}
}
