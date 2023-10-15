using System;
using VexTrack.Core.Helper;
using VexTrack.Core.Model.WPF;
using VexTrack.MVVM.ViewModel.Popups;

namespace VexTrack.MVVM.ViewModel;

class SettingsViewModel : ObservableObject
{
	private readonly bool _doUpdate;

	private ResetDataConfirmationPopupViewModel ResetDataConfirmationPopup { get; }
	private AboutPopupViewModel AboutPopup { get; }
	private MainViewModel MainVm { get; }
	public RelayCommand ThemeButtonCommand { get; }
	public RelayCommand AccentButtonCommand { get; }
	public RelayCommand OnAboutClicked { get; }
	public RelayCommand OnResetClicked { get; }
	public RelayCommand OnDefaultsClicked { get; }

	private string _theme;
	private string _accent;

	private string _username;
	private double _bufferPercentage;
	private bool _ignoreInactive;
	private bool _ignoreInit;
	private bool _ignorePreReleases;
	private bool _forceEpilogue;
	private bool _singleSeasonHistory;

	public string Theme
	{
		get => _theme;
		private set
		{
			_theme = value;
			OnPropertyChanged();
		}
	}
	public string Accent
	{
		get => _accent;
		private set
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
			if (_doUpdate) SettingsHelper.CallMainUpdate();
		}
	}
	public double BufferPercentage
	{
		get => _bufferPercentage;
		set
		{
			if (Math.Abs(_bufferPercentage - value) < 0.001) return;

			_bufferPercentage = value;
			SettingsHelper.Data.BufferPercentage = value;
			OnPropertyChanged();
			if (_doUpdate) SettingsHelper.CallMainUpdate();
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
			if (_doUpdate) SettingsHelper.CallMainUpdate();
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
			if (_doUpdate) SettingsHelper.CallMainUpdate();
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
			if (_doUpdate) SettingsHelper.CallMainUpdate();
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
			if (_doUpdate) SettingsHelper.CallMainUpdate();
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
			if (_doUpdate) SettingsHelper.CallMainUpdate();
		}
	}

	public SettingsViewModel()
	{
		MainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
		ResetDataConfirmationPopup = (ResetDataConfirmationPopupViewModel)ViewModelManager.ViewModels[nameof(ResetDataConfirmationPopupViewModel)];
		AboutPopup = (AboutPopupViewModel)ViewModelManager.ViewModels[nameof(AboutPopupViewModel)];

		ThemeButtonCommand = new RelayCommand(theme => SetTheme((string)theme));
		AccentButtonCommand = new RelayCommand(accent => SetAccent((string)accent));

		OnDefaultsClicked = new RelayCommand(_ =>
		{
			SettingsHelper.Data.Reset();
			SettingsHelper.Data.UpdateTheme();
			SettingsHelper.CallMainUpdate();
		});
		OnResetClicked = new RelayCommand(_ =>
		{
			MainVm.QueuePopup(ResetDataConfirmationPopup);
		});
		OnAboutClicked = new RelayCommand(_ =>
		{
			MainVm.QueuePopup(AboutPopup);
		});

		Update();
		_doUpdate = true;
	}

	public void Update()
	{
		Theme = SettingsHelper.Data.ThemeString;
		Accent = SettingsHelper.Data.AccentString;

		Username = SettingsHelper.Data.Username;
		BufferPercentage = SettingsHelper.Data.BufferPercentage;
		IgnoreInactive = SettingsHelper.Data.IgnoreInactiveDays;
		IgnoreInit = SettingsHelper.Data.IgnoreInit;
		IgnorePreReleases = SettingsHelper.Data.IgnorePreReleases;
		ForceEpilogue = SettingsHelper.Data.ForceEpilogue;
		SingleSeasonHistory = SettingsHelper.Data.SingleSeasonHistory;
	}

	private static void SetTheme(string theme)
	{
		SettingsHelper.Data.ThemeString = theme;
		SettingsHelper.Data.UpdateTheme();
		SettingsHelper.CallMainUpdate();
	}

	private static void SetAccent(string accent)
	{
		SettingsHelper.Data.AccentString = accent;
		SettingsHelper.Data.UpdateTheme();
		SettingsHelper.CallMainUpdate();
	}
}