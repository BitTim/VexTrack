using System.Collections.Generic;
using System.Collections.ObjectModel;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class ProgressActivityPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnOkClicked { get; }
		public bool CompletedVisible => Completed.Count != 0;
		public bool LostVisible => Lost.Count != 0;

		public ObservableCollection<string> Completed { get; } = new();
		public ObservableCollection<string> Lost { get; } = new();

		public ProgressActivityPopupViewModel()
		{
			CanCancel = true;
			OnOkClicked = new RelayCommand(_ => { Close(); });
		}

		public void SetData(List<string> completed, List<string> lost)
		{
			Completed.Clear();
			Lost.Clear();

			foreach (var c in completed) Completed.Add(c);
			foreach (var l in lost) Lost.Add(l);

			IsInitialized = true;
		}
	}
}
