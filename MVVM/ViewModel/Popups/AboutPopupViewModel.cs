using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class AboutPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnChangelogClicked { get; set; }
		public RelayCommand OnCheckUpdateClicked { get; set; }
		public RelayCommand OnForceUpdateClicked { get; set; }
		public RelayCommand OnCloseClicked { get; set; }

		public AboutPopupViewModel()
		{
			CanCancel = true;

			OnCloseClicked = new RelayCommand(o => Close());
		}
	}
}
