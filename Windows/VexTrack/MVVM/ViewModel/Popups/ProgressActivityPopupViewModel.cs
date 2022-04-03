using System.Collections.Generic;
using System.Collections.ObjectModel;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class ProgressActivityPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnOkClicked { get; set; }
		public bool CompletedVisible { get => Completed.Count != 0; }
		public bool LostVisible { get => Lost.Count != 0; }

		private ObservableCollection<string> _completed = new();
		public ObservableCollection<string> Completed
		{
			get => _completed;
			set
			{
				if (_completed != value)
				{
					_completed = value;
					OnPropertyChanged();
				}
			}
		}

		private ObservableCollection<string> _lost = new();
		public ObservableCollection<string> Lost
		{
			get => _lost;
			set
			{
				if (_lost != value)
				{
					_lost = value;
					OnPropertyChanged();
				}
			}
		}

		public ProgressActivityPopupViewModel()
		{
			CanCancel = true;
			OnOkClicked = new RelayCommand(o => { Close(); });
		}

		public void SetData(List<string> completed, List<string> lost)
		{
			Completed.Clear();
			Lost.Clear();

			foreach (string c in completed) Completed.Add(c);
			foreach (string l in lost) Lost.Add(l);

			IsInitialized = true;
		}
	}
}
