using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VexTrack.Core;

namespace VexTrack.MVVM.ViewModel.Popups
{
	class HistoryEntryPopupViewModel : BasePopupViewModel
	{
		public RelayCommand OnBackClicked { get; set; }
		public RelayCommand OnEditClicked { get; set; }
		public RelayCommand OnDeleteClicked { get; set; }
		private EditableHistoryEntryPopupViewModel EditableHEPopup { get; set; }

		private HistoryEntryData RawData { get; set; }
		public int Index { get; set; }
		public string UUID { get; set; }
		public string Description { get; set; }
		public long Time { get; set; }
		public int Amount { get; set; }
		public string Map { get; set; }
		public string Result { get; set; }

		public HistoryEntryPopupViewModel()
		{
			EditableHEPopup = (EditableHistoryEntryPopupViewModel)ViewModelManager.ViewModels["EditableHEPopup"];
			CanCancel = true;

			OnBackClicked = new RelayCommand(o => { Close(); });
			OnEditClicked = new RelayCommand(o => {
				EditableHEPopup.SetParameters("Edit History Entry", true);
				EditableHEPopup.SetData(RawData);
				MainVM.QueuePopup(EditableHEPopup);
			});
			OnDeleteClicked = new RelayCommand(o => {
				IsInitialized = false;
				TrackingDataHelper.RemoveHistoryEntry(TrackingDataHelper.CurrentSeasonIndex, Index);
			});
		}

		public void SetData(HistoryEntryData data)
		{
			RawData = data;

			Index = data.Index;
			UUID = data.UUID;
			Description = data.Description;
			Time = data.Time;
			Amount = data.Amount;
			Map = data.Map;
			Result = data.Result;

			if (Result == "") Result = "-";
			if (Map == "" || Map == null) Map = "-";

			IsInitialized = true;
		}

		public override void Close()
		{
			EditableHEPopup.Close();
			base.Close();
		}
	}
}
