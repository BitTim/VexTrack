using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel
{
	class GoalViewModel : ObservableObject
	{
		public RelayCommand ProgressButtonClick { get; set; }
		public RelayCommand OnAddClicked { get; set; }

		public void Update()
		{

		}
	}
}
