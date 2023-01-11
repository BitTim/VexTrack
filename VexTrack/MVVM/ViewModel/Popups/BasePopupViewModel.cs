using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class BasePopupViewModel : ObservableObject
	{
		public bool CanCancel { get; set; }
		public bool IsOpen { get; set; }
		public bool IsInitialized { get; set; }

		protected MainViewModel MainVm { get; set; }

		public BasePopupViewModel()
		{
			IsInitialized = false;
			MainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
		}

		public virtual void Close()
		{
			if (IsOpen && CanCancel) MainVm.DequeuePopup(this);
		}
	}
}
