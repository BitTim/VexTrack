using VexTrack.Core;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel
{
	class SettingsViewModel : ObservableObject
	{
		private bool NoUpdate = true;

		private ResetDataConfirmationPopupViewModel ResetDataConfirmationPopup { get; set; }
		private AboutPopupViewModel AboutPopup { get; set; }
		private MainViewModel MainVM { get; set; }
		public RelayCommand ThemeButtonCommand { get; set; }
		public RelayCommand AccentButtonCommand { get; set; }
		public RelayCommand OnAboutClicked { get; set; }
		public RelayCommand OnResetClicked { get; set; }
		public RelayCommand OnDefaultsClicked { get; set; }

		private string _theme;
		private string _accent;

		private string _username;
		private int _bufferDays;
		private bool _ignoreInactive;
		private bool _ignoreInit;
		private bool _ignorePreReleases;
		private bool _forceEpilogue;
		private bool _singleSeasonHistory;

		public string Theme
		{
			get => _theme;
			set
			{
				_theme = value;
				OnPropertyChanged();
			}
		}
		public string Accent
		{
			get => _accent;
			set
			{
				_accent = value;
				OnPropertyChanged();
			}
		}

		public string Username
		{
			get => _username;
			set
			{
				if (_username == value) return;

				_username = value;
				SettingsHelper.Data.Username = value;
				OnPropertyChanged();
				if (!NoUpdate) SettingsHelper.CallUpdate();
			}
		}
		public int BufferDays
		{
			get => _bufferDays;
			set
			{
				if (_bufferDays == value) return;

				_bufferDays = value;
				SettingsHelper.Data.BufferDays = value;
				OnPropertyChanged();
				if (!NoUpdate) SettingsHelper.CallUpdate();
			}
		}
		public bool IgnoreInactive
		{
			get => _ignoreInactive;
			set
			{
				if (_ignoreInactive == value) return;

				_ignoreInactive = value;
				SettingsHelper.Data.IgnoreInactiveDays = value;
				OnPropertyChanged();
				if (!NoUpdate) SettingsHelper.CallUpdate();
			}
		}
		public bool IgnoreInit
		{
			get => _ignoreInit;
			set
			{
				if (_ignoreInit == value) return;

				_ignoreInit = value;
				SettingsHelper.Data.IgnoreInit = value;
				OnPropertyChanged();
				if (!NoUpdate) SettingsHelper.CallUpdate();
			}
		}
		public bool IgnorePreReleases
		{
			get => _ignorePreReleases;
			set
			{
				if (_ignorePreReleases == value) return;

				_ignorePreReleases = value;
				SettingsHelper.Data.IgnorePreReleases = value;
				OnPropertyChanged();
				if (!NoUpdate) SettingsHelper.CallUpdate();
			}
		}
		public bool ForceEpilogue
		{
			get => _forceEpilogue;
			set
			{
				if (_forceEpilogue == value) return;

				_forceEpilogue = value;
				SettingsHelper.Data.ForceEpilogue = value;
				OnPropertyChanged();
				if (!NoUpdate) SettingsHelper.CallUpdate();
			}
		}
		public bool SingleSeasonHistory
		{
			get => _singleSeasonHistory;
			set
			{
				if (_singleSeasonHistory == value) return;

				_singleSeasonHistory = value;
				SettingsHelper.Data.SingleSeasonHistory = value;
				OnPropertyChanged();
				if (!NoUpdate) SettingsHelper.CallUpdate();
			}
		}

		public SettingsViewModel()
		{
			MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
			ResetDataConfirmationPopup = (ResetDataConfirmationPopupViewModel)ViewModelManager.ViewModels["ResetDataConfirmationPopup"];
			AboutPopup = (AboutPopupViewModel)ViewModelManager.ViewModels["AboutPopup"];

			ThemeButtonCommand = new RelayCommand(theme => SetTheme((string)theme));
			AccentButtonCommand = new RelayCommand(accent => SetAccent((string)accent));

			OnDefaultsClicked = new RelayCommand(o =>
			{
				SettingsHelper.Data.SetDefault();
				SettingsHelper.ApplyVisualSettings();
				SettingsHelper.CallUpdate();
			});
			OnResetClicked = new RelayCommand(o =>
			{
				MainVM.QueuePopup(ResetDataConfirmationPopup);
			});
			OnAboutClicked = new RelayCommand(o =>
			{
				MainVM.QueuePopup(AboutPopup);
			});

			Update();
			NoUpdate = false;
		}

		public void Update()
		{
			Theme = SettingsHelper.Data.Theme;
			Accent = SettingsHelper.Data.Accent;

			Username = SettingsHelper.Data.Username;
			BufferDays = SettingsHelper.Data.BufferDays;
			IgnoreInactive = SettingsHelper.Data.IgnoreInactiveDays;
			IgnoreInit = SettingsHelper.Data.IgnoreInit;
			IgnorePreReleases = SettingsHelper.Data.IgnorePreReleases;
			ForceEpilogue = SettingsHelper.Data.ForceEpilogue;
			SingleSeasonHistory = SettingsHelper.Data.SingleSeasonHistory;
		}

		public void SetTheme(string theme)
		{
			SettingsHelper.Data.Theme = theme;
			SettingsHelper.ApplyVisualSettings();
			SettingsHelper.CallUpdate();
		}

		public void SetAccent(string accent)
		{
			SettingsHelper.Data.Accent = accent;
			SettingsHelper.ApplyVisualSettings();
			SettingsHelper.CallUpdate();
		}
	}
}
