using System.Collections.Generic;
using System.Collections.ObjectModel;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class UpdateAvailablePopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnCancelClicked { get; }
		public RelayCommand OnUpdateClicked { get; }
		public bool ChangelogVisible => Changelog.Count != 0;
		public bool WarningsVisible => Warnings.Count != 0;

		public ObservableCollection<string> Changelog { get; } = new();
		public ObservableCollection<string> Warnings { get; } = new();

		private string _currentVersion;
		private string _newVersion;

		public string CurrentVersion
		{
			get => _currentVersion;
			private set
			{
				_currentVersion = value;
				OnPropertyChanged();
			}
		}

		public string NewVersion
		{
			get => _newVersion;
			private set
			{
				_newVersion = value;
				OnPropertyChanged();
			}
		}

		public UpdateAvailablePopupViewModel()
		{
			CanCancel = true;
			OnCancelClicked = new RelayCommand(_ => { Close(); });
			OnUpdateClicked = new RelayCommand(_ =>
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

			foreach (var c in changelog) Changelog.Add(c);
			foreach (var w in warnings) Warnings.Add(w);

			IsInitialized = true;
		}
	}
}
