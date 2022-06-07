using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class UpdateFailedPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnOkClicked { get; set; }
		private string _errorMessage;

		public string ErrorMessage
		{
			get { return _errorMessage; }
			set
			{
				_errorMessage = value;
				OnPropertyChanged();
			}
		}

		public UpdateFailedPopupViewModel()
		{
			CanCancel = true;
			OnOkClicked = new RelayCommand(o => { Close(); });
		}

		public void SetData(string errorMessage)
		{
			ErrorMessage = errorMessage;
			IsInitialized = true;
		}
	}
}
