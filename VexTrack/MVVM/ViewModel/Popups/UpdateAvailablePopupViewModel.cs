using System.Collections.Generic;
using System.Collections.ObjectModel;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class UpdateAvailablePopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnCancelClicked { get; set; }
		public RelayCommand OnUpdateClicked { get; set; }
		public bool ChangelogVisible { get => Changelog.Count != 0; }
		public bool WarningsVisible { get => Warnings.Count != 0; }

		private ObservableCollection<string> _changelog = new();
		public ObservableCollection<string> Changelog
		{
			get => _changelog;
			set
			{
				if (_changelog != value)
				{
					_changelog = value;
					OnPropertyChanged();
				}
			}
		}

		private ObservableCollection<string> _warnings = new();
		public ObservableCollection<string> Warnings
		{
			get => _warnings;
			set
			{
				if (_warnings != value)
				{
					_warnings = value;
					OnPropertyChanged();
				}
			}
		}

		private string _currentVersion;
		private string _newVersion;

		public string CurrentVersion
		{
			get { return _currentVersion; }
			set
			{
				_currentVersion = value;
				OnPropertyChanged();
			}
		}

		public string NewVersion
		{
			get { return _newVersion; }
			set
			{
				_newVersion = value;
				OnPropertyChanged();
			}
		}

		public UpdateAvailablePopupViewModel()
		{
			CanCancel = true;
			OnCancelClicked = new RelayCommand(o => { Close(); });
			OnUpdateClicked = new RelayCommand(o =>
			{
				UpdateHelper.GetUpdate();
				Close();
			});
		}

		public void SetData(List<string> changelog, List<string> warnings, string newVersion)
		{
			Changelog.Clear();
			Warnings.Clear();

			CurrentVersion = Constants.Version;
			NewVersion = newVersion;

			foreach (string c in changelog) Changelog.Add(c);
			foreach (string w in warnings) Warnings.Add(w);

			IsInitialized = true;
		}
	}
}
