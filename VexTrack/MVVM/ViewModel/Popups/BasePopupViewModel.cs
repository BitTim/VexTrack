using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class BasePopupViewModel : ObservableObject
	{
		public bool CanCancel { get; set; }
		public bool IsOpen { get; set; }
		public bool IsInitialized { get; set; }

		protected MainViewModel MainVM { get; set; }

		public BasePopupViewModel()
		{
			IsInitialized = false;
			MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
		}

		public virtual void Close()
		{
			if (IsOpen && CanCancel) MainVM.DequeuePopup(this);
		}
	}
}
