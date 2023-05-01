using VexTrack.Core.Model.WPF;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class BasePopupViewModel : ObservableObject
	{
		public bool CanCancel { get; set; }
		public bool IsOpen { get; set; }
		public bool IsInitialized { get; protected set; }

		protected MainViewModel MainVm { get; }

		protected BasePopupViewModel()
		{
			IsInitialized = false;
			MainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
		}

		public virtual void Close()
		{
			if (IsOpen && CanCancel) MainVm.DequeuePopup(this);
		}
	}
}
