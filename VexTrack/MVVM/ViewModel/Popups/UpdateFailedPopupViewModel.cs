using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class UpdateFailedPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnOkClicked { get; }
		private string _errorMessage;

		public string ErrorMessage
		{
			get => _errorMessage;
			private set
			{
				_errorMessage = value;
				OnPropertyChanged();
			}
		}

		public UpdateFailedPopupViewModel()
		{
			CanCancel = true;
			OnOkClicked = new RelayCommand(_ => { Close(); });
		}

		public void SetData(string errorMessage)
		{
			ErrorMessage = errorMessage;
			IsInitialized = true;
		}
	}
}
