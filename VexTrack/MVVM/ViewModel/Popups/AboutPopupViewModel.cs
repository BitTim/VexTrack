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
		public RelayCommand OnCheckUpdateClicked { get; set; }
		public RelayCommand OnForceUpdateClicked { get; set; }
		public RelayCommand OnCloseClicked { get; set; }

		public string Version => Constants.Version;

		public AboutPopupViewModel()
		{
			CanCancel = true;

			OnCloseClicked = new RelayCommand(o => Close());
			OnCheckUpdateClicked = new RelayCommand(o => UpdateHelper.CheckUpdateAsync());
			OnForceUpdateClicked = new RelayCommand(o => UpdateHelper.CheckUpdateAsync(true));
		}
	}
}
