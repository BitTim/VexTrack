using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class HistoryEntryPopupViewModel : ObservableObject
	{
		public RelayCommand OnBackClicked { get; set; }

		public string Description { get; set; }
		public long Time { get; set; }
		public int Amount { get; set; }
		public string Map { get; set; }
		public string Result { get; set; }

		private MainViewModel MainVM { get; set; }

		public HistoryEntryPopupViewModel()
		{
			MainVM = (MainViewModel)ViewModelManager.ViewModels["Main"];
			OnBackClicked = new RelayCommand(o => { MainVM.CurrentPopup = null; });
		}

		public void SetData(HistoryEntryData data)
		{
			Description = data.Description;
			Time = data.Time;
			Amount = data.Amount;
			Map = data.Map;
			Result = data.Result;

			if (Result == "") Result = "-";
			if (Map == "" || Map == null) Map = "-";

			OnPropertyChanged();
		}
	}
}
